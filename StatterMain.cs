using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using Advanced_Combat_Tracker;
using ACT_Plugin.UI;
using ACT_Plugin.Model;

namespace ACT_Plugin
{
    /*
    * See https://github.com/EQAditu/AdvancedCombatTracker/wiki/Plugin-Creation-Tips
    */
    public class StatterMain : IActPluginV1
    {
#if DEBUG
        public static bool DEBUG = true;
#else
        public static bool DEBUG = false;
#endif

        public static int PLUGIN_ID = 999999;

        public const string MACRO_FILENAME = "statter.txt";
        public const string DYNAMICDATA_NOP = "dynamicdata Start";
        public const string DYNAMICDATA_NOT_FOUND = "DynamicData not found";
        public const string DYNAMICDATA_COMMAND_PREFIX = "dynamicdata Stats.";

        public const string ACTFORM_ENCOUNTER_TREEVIEW_NAME = "tvDG";
        public const string ACTFORM_ENCOUNTER_TREEVIEW_VALID_ENCOUNTER_TAG = "EncounterData";
        public const string ACTFORM_ENCOUNTER_TREEVIEW_MENU_OPTION = "View Encounter Stats";

        // This enum describes whether we're currently parsing a collection of stat lines
        private enum ParseState
        {
            None,
            ReadingStats
        }

        // These elements are given to us by the plugin engine
        private TabPage _pluginScreenSpace = null;
        private Label _pluginStatusText = null;

        // Keep references to some elements from the main ACT form
        private TreeView _oFrmMainEncounterTree = null;
        private ListView _oFrmMainAttackList = null;
        private ContextMenuStrip _menuEncounter = null;
        private ToolStripItem _tsEncounterShowStats = null;

        // Since we're searching the main ACT form object hierarchy for object references,
        // use a delay timer to give it time to load
        private Timer _timerDelayedAttach = new Timer();
        private const int DELAY_ATTACH_SECONDS = 5 * 1000;

        // Our parsing-sepecifc state vars
        private List<Func<LogLineEventArgs, bool>> _readHandlers = new List<Func<LogLineEventArgs, bool>>();
        private Regex _regexStatPackStart = new Regex(string.Format(@"^{0}\.$", DYNAMICDATA_NOT_FOUND), RegexOptions.Compiled);
        private ParseState _parseState = ParseState.None;
        private int _statIdx = 0;
        private StatterPluginTab _ui = null;
        private StatterSettings _settings = new StatterSettings();

        public StatterMain()
        {
            // Wire up other events
            _timerDelayedAttach.Tick += new EventHandler(timerDelayedAttach_Tick);
        }

        // The entry-point from the ACT plugin engine, called when the plugin is loaded
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            try
            {
                _pluginScreenSpace = pluginScreenSpace;
                _pluginStatusText = pluginStatusText;

                _settings.Load();

                _ui = new StatterPluginTab(_settings);
                _ui.SelectedStatsChanged += new Action(selectedStatsChanged);
                _ui.Dock = DockStyle.Fill;
                _pluginScreenSpace.Controls.Add(_ui);
                _pluginScreenSpace.Text = "Statter";

                CreateMacroFile();

                // Add the stat read handler to the list of currently active handlers
                EnableHandler(HandleStatRead, true);

                // Kick off a timer to do additional initialization
                _timerDelayedAttach.Interval = DELAY_ATTACH_SECONDS;
                _timerDelayedAttach.Start();

                ShowPluginStatus(true);
            }
            catch {}
        }

        // The exit-point from the ACT plugin engine, called when the plugin is unloaded
        public void DeInitPlugin()
        {
            try
            {
                DettachFromActForm();

                ShowPluginStatus(false);
            }
            catch { }
        }

        private void AttachToActForm()
        {
            // Wire-up the ACT plugin engine to pass us log line read events
            ActGlobals.oFormActMain.OnLogLineRead -= oFormActMain_OnLogLineRead;
            ActGlobals.oFormActMain.OnLogLineRead += oFormActMain_OnLogLineRead;

            // Get a reference to the left-side encounter treeview, and attach a command to 
            // the right-click context menu to view stats during each encounter
            _oFrmMainEncounterTree = FindControl(ActGlobals.oFormActMain, ACTFORM_ENCOUNTER_TREEVIEW_NAME) as TreeView;
            if (_oFrmMainEncounterTree != null)
            {
                _menuEncounter = _oFrmMainEncounterTree.ContextMenuStrip;
                if (_menuEncounter != null)
                {
                    _menuEncounter.Opening -= menuEncounter_Opening;
                    _menuEncounter.Opening += menuEncounter_Opening;

                    _tsEncounterShowStats = _menuEncounter.Items.Add(ACTFORM_ENCOUNTER_TREEVIEW_MENU_OPTION);
                    _tsEncounterShowStats.Click += new EventHandler(tsEncounterShowStats_Click);
                }
            }
        }

        // Unhook all the events we attached to the ACT plugin engine, and its forms
        private void DettachFromActForm()
        {
            ActGlobals.oFormActMain.OnLogLineRead -= oFormActMain_OnLogLineRead;

            if (_menuEncounter != null)
            {
                _menuEncounter.Opening -= menuEncounter_Opening;

                if (_tsEncounterShowStats != null)
                {
                    _tsEncounterShowStats.Click -= tsEncounterShowStats_Click;
                    _menuEncounter.Items.Remove(_tsEncounterShowStats);
                }
            }
        }

        private void EnableHandler(Func<LogLineEventArgs, bool> handler, bool enable)
        {
            if (enable)
            {
                if (!_readHandlers.Contains(handler))
                {
                    _parseState = ParseState.None;
                    _readHandlers.Add(handler);
                }
            }
            else
            {
                _readHandlers.Remove(handler);
            }
        }

        // The main parsing logic lives here
        private bool HandleStatRead(LogLineEventArgs logInfo)
        {
            // Return an indication of whether we handled the parse
            bool parsed = true;

            // Extract the actual text of the log line
            string logLine = logInfo.logLine;
            int timeEndPos = logLine.IndexOf("] ");
            if (timeEndPos >= 0)
                logLine = logLine.Substring(timeEndPos + 2).Trim();

            switch (_parseState)
            {
                // ParseState.None implies that we haven't detected the start of a stat block,
                // so try to find one
                case ParseState.None:
                    if (_regexStatPackStart.IsMatch(logLine))
                    {
                        _parseState = ParseState.ReadingStats;
                        _statIdx = 0;
                    }
                    break;

                // ParseState.ReadingStats implies that we are currently parsing stats, so keep
                // reading them until we hit the limit
                case ParseState.ReadingStats:
                    var stat = _settings.Stats[_statIdx];
                    stat.ParseReading(logLine, logInfo.detectedTime);
                    _statIdx++;

                    if (_statIdx >= _settings.Stats.Count) // We're done with this stat pack
                    {
                        _parseState = ParseState.None;
                    }

                    break;
            }

            return parsed;
        }

        // This is called each time ACT detects a new log line
        private void oFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            if (isImport && !_settings.ParseOnImport) return;

            try
            {
                // Iterate over each handler, giving the chain a chance to override
                // subsequent handlers
                foreach (Func<LogLineEventArgs, bool> handler in _readHandlers)
                    if (handler(logInfo))
                        break;
            }
            catch { } // Black hole...
        }

        private void timerDelayedAttach_Tick(object sender, EventArgs e)
        {
            // Kill the timer, this is a singular event
            _timerDelayedAttach.Stop();

            // Now complete the init
            AttachToActForm();
        }

        // Enable or disable the option to view encounter stats based
        // on whether the selected control contains encounter data
        private void menuEncounter_Opening(object sender, CancelEventArgs e)
        {
            _tsEncounterShowStats.Enabled = GetSelectedEncounter() != null;
        }

        // Launch a dialog box with enounter stats
        private void tsEncounterShowStats_Click(object sender, EventArgs e)
        {
            EncounterData encounterData = GetSelectedEncounter();
            if (encounterData != null)
                ShowStatDialog();
        }

        // Launch a dialog box with enounter stats
        private void tsAttackShowStats_Click(object sender, EventArgs e)
        {
            ListViewItem attackItem = _oFrmMainAttackList.FocusedItem;
            if (attackItem != null)
            {
                DateTime attackTime = DateTime.MinValue;

                // Iterate through the list items, trying to find a valid encounter start time
                for (int i = 0; i < attackItem.SubItems.Count; i++)
                    if (DateTime.TryParse(attackItem.SubItems[i].Text, out attackTime))
                        break;

                if (attackTime != DateTime.MinValue)
                    ShowStatDialog();
            }
        }

        // Event is fired whenever the user has changed their selection of tracked stats
        private void selectedStatsChanged()
        {
            // Re-write the macro file to use the new stats
            CreateMacroFile();

            ShowPluginStatus(true);
        }

        // Helper funtion to search a control tree and return the control with the given name
        private Control FindControl(Control parent, string name)
        {
            if (parent.Name.Equals(name)) return parent;

            foreach (Control c in parent.Controls)
            {
                Control found = FindControl(c, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        // Create a macro file that the client can call to dump stats to the EQ2 log
        private void CreateMacroFile()
        {
            if (_settings.Stats.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                // The first line in the macro is to trigger our parsing state.
                // It is meaningless to EQ2, but will result in the string DYNAMICDATA_NOT_FOUND
                // being dumped into the log.  We will key on this to begin parsing stats.
                sb.AppendLine(DYNAMICDATA_NOP);

                // Now append a directive for each stat being tracked
                foreach (var stat in _settings.Stats)
                    sb.AppendLine(string.Format("{0}{1}", DYNAMICDATA_COMMAND_PREFIX, stat.ClientAttribute));

                try
                {
                    ActGlobals.oFormActMain.SendToMacroFile(MACRO_FILENAME, sb.ToString(), "");
                }
                catch { }
            }
        }

        // Attempt to return EncounterData for the currently selected node from the
        // left-side encounter treeview
        private EncounterData GetSelectedEncounter()
        {
            EncounterData encounterData = null;

            TreeNode tn = _oFrmMainEncounterTree.SelectedNode;
            if (tn != null)
            {
                while (!tn.Tag.Equals(ACTFORM_ENCOUNTER_TREEVIEW_VALID_ENCOUNTER_TAG) && tn.Parent != null)
                    tn = tn.Parent;

                if (tn.Tag.Equals(ACTFORM_ENCOUNTER_TREEVIEW_VALID_ENCOUNTER_TAG))
                {
                    int zoneId = tn.Parent.Index;
                    int encounterId = tn.Index;

                    try
                    {
                        encounterData = ActGlobals.oFormActMain.ZoneList[zoneId].Items[encounterId];
                    }
                    catch { }
                }
            }

            return encounterData;
        }

        private void ShowPluginStatus(bool enabled)
        {
            if (enabled)
                _pluginStatusText.Text = string.Format("Tracking {0} stat{1}", _settings.Stats.Count, _settings.Stats.Count == 1 ? "" : "s");
            else
                _pluginStatusText.Text = "Disabled";
        }

        private void ShowStatDialog()
        {
            EncounterData encounterData = GetSelectedEncounter();
            if (encounterData != null)
            {
                var dlgViewStats = new StatterViewStatsForm(_settings);
                dlgViewStats.ShowStats(_settings.Stats, encounterData);
            }
        }
    }
}



