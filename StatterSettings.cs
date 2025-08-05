using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Advanced_Combat_Tracker;
using ActStatter.Model;
using ActStatter.UI;
using System.Text.RegularExpressions;

namespace ActStatter
{
    public class StatterSettings
    {
        private string _settingsFile = null;
        public string SettingsFile {
            get
            {
                string filename = "Undefined";
                if (_settingsFile != null)
                {
                    filename = _settingsFile;
                    try
                    {
                        foreach (Match match in Regex.Matches(_settingsFile, @"^C:\\Users\\[^\\]+\\AppData\\Roaming\\(.+)$", RegexOptions.IgnoreCase))
                            if (match.Groups.Count >= 2)
                            {
                                var group = match.Groups[1];
                                if (group.Captures.Count > 0)
                                {
                                    var path = group.Captures[0].ToString();
                                    filename = $"%AppData%\\{path}";
                                }
                            }
                    }
                    catch { }
                }

                return filename;
            }
        }

        public bool ParseOnImport = false;
        public bool RestrictToChannels = false;
        public bool GraphShowAverage = false;
        public bool GraphShowEncDps = false;
        public bool GraphShowEncHps = false;
        public bool GraphShowEncAuto = false;
        public bool GraphShowEncSkills = false;
        public bool GraphShowRanges = false;
        public int EncDpsResolution = 0;
        public int PopupLastW = 0;
        public int PopupLastH = 0;
        public string LastPlayers = "";
        public string LastStat = "";
        public string YMin = "";
        public List<string> RestrictedChannels = new List<string>();

        public List<StatterStat> Stats = new List<StatterStat>();

        public static List<string> GetListFromString(string input, bool toLower = true)
        {
            var delimiters = new string[] { " ", ",", ":", "\t", Environment.NewLine };
            var tempList = new List<string>(input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));
            var retVal = tempList;
            if (toLower)
            {
                retVal = new List<string>();
                tempList.ForEach(item => retVal.Add(item.ToLower()));
            }
            return retVal;
        }

        public static string GetStringFromList(List<string> input)
        {
            return string.Join(", ", input);
        }

        public StatterSettings()
        {
            try
            {
                _settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\Statter.config.xml");
            }
            catch { }
        }

        public void Load()
        {
            if (_settingsFile == null || !File.Exists(_settingsFile)) return;

            XmlDocument doc = new XmlDocument();
            doc.Load(_settingsFile);
            XmlNode rootNode = doc.SelectSingleNode("Settings");

            ParseOnImport = RetrieveSetting<bool>(rootNode, "ParseOnImport", true);
            RestrictToChannels = RetrieveSetting<bool>(rootNode, "RestrictToChannels", false);
            GraphShowAverage = RetrieveSetting<bool>(rootNode, "GraphShowAverage", false);
            GraphShowEncDps = RetrieveSetting<bool>(rootNode, "GraphShowEncDps", false);
            GraphShowEncHps = RetrieveSetting<bool>(rootNode, "GraphShowEncHps", false);
            GraphShowEncAuto = RetrieveSetting<bool>(rootNode, "GraphShowEncAuto", false);
            GraphShowEncSkills = RetrieveSetting<bool>(rootNode, "GraphShowEncSkills", false);
            GraphShowRanges = RetrieveSetting<bool>(rootNode, "GraphShowRanges", false);
            EncDpsResolution = RetrieveSetting<int>(rootNode, "EncDpsResolution", 5);

            var minSize = StatterViewStatsForm.GetDefaultSize();
            PopupLastW = RetrieveSetting<int>(rootNode, "PopupLastW", minSize.Width);
            PopupLastH = RetrieveSetting<int>(rootNode, "PopupLastH", minSize.Height);
            var curScreen = Screen.FromControl(ActGlobals.oFormActMain);

            LastPlayers = RetrieveSetting<string>(rootNode, "LastPlayers", "");
            LastStat = RetrieveSetting<string>(rootNode, "LastStat", "");
            YMin = RetrieveSetting<string>(rootNode, "YMin", "Min Val");
            RestrictedChannels = GetListFromString(RetrieveSetting<string>(rootNode, "RestrictedChannels", ""));

            LoadStats(rootNode.SelectSingleNode("Stats"));
        }

        public void Save(StatterMain statter)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));

            XmlElement rootNode = doc.CreateElement("Settings");
            doc.AppendChild(rootNode);

            AttachChildNode(rootNode, "ParseOnImport", ParseOnImport.ToString());
            AttachChildNode(rootNode, "RestrictToChannels", RestrictToChannels.ToString());
            AttachChildNode(rootNode, "GraphShowAverage", GraphShowAverage.ToString());
            AttachChildNode(rootNode, "GraphShowEncDps", GraphShowEncDps.ToString());
            AttachChildNode(rootNode, "GraphShowEncHps", GraphShowEncHps.ToString());
            AttachChildNode(rootNode, "GraphShowEncAuto", GraphShowEncAuto.ToString());
            AttachChildNode(rootNode, "GraphShowEncSkills", GraphShowEncSkills.ToString());
            AttachChildNode(rootNode, "GraphShowRanges", GraphShowRanges.ToString());
            AttachChildNode(rootNode, "EncDpsResolution", EncDpsResolution.ToString());
            AttachChildNode(rootNode, "PopupLastW", PopupLastW.ToString());
            AttachChildNode(rootNode, "PopupLastH", PopupLastH.ToString());
            AttachChildNode(rootNode, "LastPlayers", LastPlayers);
            AttachChildNode(rootNode, "LastStat", LastStat);
            AttachChildNode(rootNode, "YMin", YMin);
            AttachChildNode(rootNode, "RestrictedChannels", GetStringFromList(RestrictedChannels));

            XmlElement statsNode = AttachChildNode(rootNode, "Stats", null);
            SaveStats(statsNode);

            try
            {
                doc.Save(_settingsFile);
            }
            catch (Exception ex) {
                if (statter != null)
                {
                    statter.Log("Unable to save to settings file: " + SettingsFile, true);
                    statter.Log(ex.ToString());
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Settings:");
            sb.AppendLine("  File = " + SettingsFile);
            sb.AppendLine("  ParseOnImport = " + ParseOnImport);
            sb.AppendLine("  RestrictToChannels = " + RestrictToChannels);
            sb.AppendLine("  GraphShowAverage = " + GraphShowAverage);
            sb.AppendLine("  GraphShowEncDps = " + GraphShowEncDps);
            sb.AppendLine("  GraphShowEncHps = " + GraphShowEncHps);
            sb.AppendLine("  GraphShowEncAuto = " + GraphShowEncAuto);
            sb.AppendLine("  GraphShowEncSkills = " + GraphShowEncSkills);
            sb.AppendLine("  GraphShowRanges = " + GraphShowRanges);
            sb.AppendLine("  EncDpsResolution = " + EncDpsResolution.ToString());
            sb.AppendLine("  PopupLastW = " + PopupLastW.ToString());
            sb.AppendLine("  PopupLastH = " + PopupLastH.ToString());
            sb.AppendLine("  LastPlayers = " + LastPlayers);
            sb.AppendLine("  LastStat = " + LastStat);
            sb.AppendLine("  YMin = " + YMin);
            sb.AppendLine("  RestrictedChannels = " + GetStringFromList(RestrictedChannels));
            List<string> _trackedStats = new List<string>();
            foreach (StatterStat stat in Stats)
            {
                _trackedStats.Add(stat.Name);
            }
            sb.Append("  Stats = [" + string.Join(", ", _trackedStats.ToArray()) + "]");

            return sb.ToString();
        }

        private T RetrieveSetting<T>(XmlNode attachPoint, string name, T defaultVal)
        {
            T settingVal = defaultVal;

            XmlNode settingNode = attachPoint.SelectSingleNode(name);
            if (settingNode != null)
                settingVal = (T)Convert.ChangeType(settingNode.InnerText, typeof(T));

            return settingVal;
        }

        private void LoadStats(XmlNode rootNode)
        {
            if (rootNode == null) return;

            Stats.Clear();
            foreach (XmlNode statNode in rootNode.SelectNodes("Stat"))
            {
                string name = "";
                Color colour = StatterStat.DEFAULT_COLOUR;

                foreach (XmlNode childNode in statNode.ChildNodes)
                {
                    string nodeVal = childNode.InnerText.Trim();
                    switch (childNode.Name.ToLower())
                    {
                        case "name":
                            name = nodeVal;
                            break;
                        case "colour":
                            TryParseColour(nodeVal, ref colour);
                            break;
                    }
                }

                try
                {
                    StatterStat stat = new StatterStat(name)
                    {
                        Colour = colour
                    };
                    if (!Stats.Contains(stat))
                        Stats.Add(stat);
                }
                catch { }
            }
        }

        private void SaveStats(XmlElement attachPoint)
        {
            foreach (StatterStat stat in Stats)
            {
                XmlElement statNode = AttachChildNode(attachPoint, "Stat", null);
                AttachChildNode(statNode, "Name", stat.Name);
                AttachChildNode(statNode, "Key", stat.Key);
                AttachChildNode(statNode, "Colour", ColourToString(stat.Colour));
            }
        }

        private XmlElement AttachChildNode(XmlElement parentNode, string name, string value)
        {
            XmlElement childNode = parentNode.OwnerDocument.CreateElement(name);
            if (value != null)
                childNode.InnerText = value;
            parentNode.AppendChild(childNode);

            return childNode;
        }

        private bool TryParseColour(string colourString, ref Color colour)
        {
            bool success = false;

            try
            {
                int r, g, b;
                string[] rgbParts = colourString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (int.TryParse(rgbParts[0], out r) && int.TryParse(rgbParts[1], out g) && int.TryParse(rgbParts[2], out b))
                    colour = Color.FromArgb(Math.Min(255, Math.Max(0, r)), Math.Min(255, Math.Max(0, g)), Math.Min(255, Math.Max(0, b)));
                success = true;
            }
            catch { }

            return success;
        }

        private string ColourToString(Color colour)
        {
            return string.Format("{0},{1},{2}", colour.R, colour.G, colour.B);
        }
    }
}
