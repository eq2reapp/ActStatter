using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using Advanced_Combat_Tracker;
using ActStatter.UI;
using ActStatter.Model;
using System.Reflection;

namespace ActStatter
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

        public static int PLUGIN_ID = 90;

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

        // Keep a log handy for debugging
        private List<String> Logs = new List<string>();

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
        private Regex _regexDarqStatMonLine = new Regex("^You tell [^\"]+\"DarqUI_StatMon:", RegexOptions.Compiled);
        private ParseState _parseState = ParseState.None;
        private StatterPluginTab _ui = null;
        private StatterSettings _settings = new StatterSettings();
        private StatterStatCollection _statCollection = null;

        // Also detect when DarqUI is logging stats so we can show extra help, etc.
        private bool _usingDarqUI = false;
        public bool DarqUIDetected {  get { return _usingDarqUI; } }

        public StatterMain()
        {
            // Wire up other events
            _timerDelayedAttach.Tick += new EventHandler(timerDelayedAttach_Tick);

            _statCollection = new StatterStatCollection(_settings);
        }

        public void Log(string message)
        {
            Logs.Add(String.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
        }

        public String[] GetLogs()
        {
            return Logs.ToArray();
        }

        // The entry-point from the ACT plugin engine, called when the plugin is loaded
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            try
            {
                Log("Initializing plugin");
                Log("Version = " + GetType().Assembly.GetName().Version);

                _pluginScreenSpace = pluginScreenSpace;
                _pluginStatusText = pluginStatusText;

                _settings.Load();
                Log(_settings.ToString());

                _ui = new StatterPluginTab(this, _settings);
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

                // Update pattern for file download
                // See: https://gist.github.com/EQAditu/4d6e3a1945fed2199f235fedc1e3ec56#Act_Plugin_Update.cs
                ActGlobals.oFormActMain.UpdateCheckClicked += OFormActMain_UpdateCheckClicked;
                if (ActGlobals.oFormActMain.GetAutomaticUpdatesAllowed())
                {
                    new System.Threading.Thread(new System.Threading.ThreadStart(OFormActMain_UpdateCheckClicked)) { IsBackground = true }.Start();
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        // The exit-point from the ACT plugin engine, called when the plugin is unloaded
        public void DeInitPlugin()
        {
            Log("Deinitializing plugin");
            try
            {
                ActGlobals.oFormActMain.UpdateCheckClicked -= OFormActMain_UpdateCheckClicked;

                DettachFromActForm();

                ShowPluginStatus(false);
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            finally
            {
                Log("Deinitialized plugin");
            }
        }

        private void OFormActMain_UpdateCheckClicked()
        {
            // This ID must be the same ID used on ACT's website.
            int pluginId = PLUGIN_ID;
            string pluginName = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            try
            {
                Version localVersion = GetType().Assembly.GetName().Version;
                Version remoteVersion = new Version(ActGlobals.oFormActMain.PluginGetRemoteVersion(pluginId).TrimStart(new char[] { 'v' }));
                if (remoteVersion > localVersion)
                {
                    DialogResult result = MessageBox.Show(
                        $"There is an updated version of the {pluginName} plugin.  Update it now?\n\n(If there is an update to ACT, you should click No and update ACT first.)",
                        "New Version", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        FileInfo updatedFile = ActGlobals.oFormActMain.PluginDownload(pluginId);
                        ActPluginData pluginData = ActGlobals.oFormActMain.PluginGetSelfData(this);
                        pluginData.pluginFile.Delete();
                        updatedFile.MoveTo(pluginData.pluginFile.FullName);

                        // You can choose to simply restart the plugin, if the plugin can properly clean-up in DeInit
                        // and has no external assemblies that update.
                        ThreadInvokes.CheckboxSetChecked(ActGlobals.oFormActMain, pluginData.cbEnabled, false);
                        Application.DoEvents();
                        ThreadInvokes.CheckboxSetChecked(ActGlobals.oFormActMain, pluginData.cbEnabled, true);
                    }
                }
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(ex, $"Plugin Update Check - {pluginName}");
            }
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

                    Log("Attached to tree");
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

                    Log("Detached from tree");
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
            string marker = "] ";
            int markerPos = logLine.IndexOf(marker);
            if (markerPos >= 0)
                logLine = logLine.Substring(markerPos + marker.Length).Trim();

            switch (_parseState)
            {
                // ParseState.None implies that we haven't detected the start of a stat block,
                // so try to find one
                case ParseState.None:
                    if (_regexStatPackStart.IsMatch(logLine))
                    {
                        _parseState = ParseState.ReadingStats;
                        _statCollection.StartStatGroup();
                        Log("Parsing stats");
                    }
                    break;

                // ParseState.ReadingStats implies that we are currently parsing stats, so keep
                // reading them until we hit the limit
                case ParseState.ReadingStats:
                    if (!_statCollection.AddStatGroupReading(logLine, logInfo.detectedTime))
                        _parseState = ParseState.None;
                    break;
            }

            // Also check for DarqUI StatMon loglines
            if (_regexDarqStatMonLine.IsMatch(logLine))
            {
                _usingDarqUI = true;

                marker = ", \"";
                markerPos = logLine.IndexOf(marker);
                if (markerPos >= 0)
                    logLine = logLine.Substring(markerPos + marker.Length).TrimEnd('"');
                _statCollection.AddDarqReading(logLine, logInfo.detectedTime);
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
                ShowStatDialog(encounterData);
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

            Log(_settings.ToString());
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
                    sb.AppendLine(string.Format("{0}{1}", DYNAMICDATA_COMMAND_PREFIX, stat.Key));

                try
                {
                    ActGlobals.oFormActMain.SendToMacroFile(MACRO_FILENAME, sb.ToString(), "");
                    Log("Wrote macro file: " + Path.Combine(ActGlobals.oFormActMain.GameMacroFolder, MACRO_FILENAME));
                }
                catch (Exception ex)
                {
                    Log("Failed to write macro file: " + Path.Combine(ActGlobals.oFormActMain.GameMacroFolder, MACRO_FILENAME));
                    Log(ex.Message);
                }
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
                        Log("Found encounter data: " + encounterData.Title);
                    }
                    catch (Exception ex)
                    {
                        Log("Failed to find encounter data");
                        Log(ex.Message);
                    }
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

        private void ShowStatDialog(EncounterData encounterData = null)
        {
            if (encounterData == null)
            {
                encounterData = GetSelectedEncounter();
            }

            if (encounterData != null)
            {
                Log("Showing encounter data");
                var dlgViewStats = new StatterViewStatsForm(this, _settings);
                dlgViewStats.ShowStats(_statCollection.GetReadings(encounterData.StartTime, encounterData.EndTime), encounterData);
            }
            else
            {
                Log("No encounter data");
            }
        }
    }
}
