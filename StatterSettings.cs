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

namespace ActStatter
{
    public class StatterSettings
    {
        private string _settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\Statter.config.xml");

        public bool ParseOnImport = true;
        public bool StepLines = true;
        public List<StatterStat> Stats = new List<StatterStat>();

        public StatterSettings()
        {
        }

        public void Load()
        {
            if (!File.Exists(_settingsFile)) return;

            XmlDocument doc = new XmlDocument();
            doc.Load(_settingsFile);
            XmlNode rootNode = doc.SelectSingleNode("Settings");

            ParseOnImport = RetrieveSetting<bool>(rootNode, "ParseOnImport");
            StepLines = RetrieveSetting<bool>(rootNode, "StepLines");

            LoadStats(rootNode.SelectSingleNode("Stats"));
        }

        public void Save()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));

            XmlElement rootNode = doc.CreateElement("Settings");
            doc.AppendChild(rootNode);

            AttachChildNode(rootNode, "ParseOnImport", ParseOnImport.ToString());
            AttachChildNode(rootNode, "StepLines", StepLines.ToString());

            XmlElement statsNode = AttachChildNode(rootNode, "Stats", null);
            SaveStats(statsNode);

            doc.Save(_settingsFile);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Settings:");
            sb.AppendLine("  ParseOnImport = " + ParseOnImport);
            sb.AppendLine("  StepLines = " + StepLines);
            List<string> _trackedStats = new List<string>();
            foreach (StatterStat stat in Stats)
            {
                _trackedStats.Add(stat.Name);
            }
            sb.Append("  Stats = [" + string.Join(", ", _trackedStats.ToArray()) + "]");

            return sb.ToString();
        }

        private T RetrieveSetting<T>(XmlNode attachPoint, string name)
        {
            T settingVal = default(T);

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
