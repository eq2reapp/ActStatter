using System;
using System.Collections.Generic;
using System.Drawing;

namespace ActStatter.Model
{
    public class StatterStat
    {
        public static readonly Color DEFAULT_COLOUR = Color.Black;

        private string _name = "";
        private string _key = "";

        public string Name { get { return _name; } }
        public string Key { get { return _key; } }
        public Color Colour { get; set; }
        public bool HasSecondary { get; set; }

        // Trackable stats are pulled from <EQ2_Dir>\UI\Default\eq2ui_gamedata.xml
        // Search for: <DataSource description="Stats" Name="Stats">
        private static Dictionary<string, string> StatKeyToFriendlyNames = new Dictionary<string, string>()
        {
            { "AAXPMod", "AA XP Mod" },
            { "AAXPModCap", "AA XP Mod Cap" },
            { "Ability_Mod", "Ability Mod" },
            { "AbilityDoubleAttack", "Ability Double Cast" },
            { "Accuracy", "Accuracy" },
            { "AE_AutoAtk_Percent", "AE Auto" },
            { "Agility", "Agility" },
            { "Arcane", "Arcane Mit" },
            { "CombatXPMod", "CombatXPMod" },
            { "CombatXPModCap", "CombatXPModCap" },
            { "ConcentrationRange", "Concentration" },
            { "Crit_Bonus", "Crit Bonus" },
            { "Crit_Chance", "Crit Chance" },
            { "Critical_Mitigation", "Crit Mit" },
            { "CurrentStatus", "Status" },
            { "Damage_Reduction_Arcane", "DR Arcane" },
            { "Damage_Reduction_Elemental", "DR Elemental" },
            { "Damage_Reduction_Noxious", "DR Noxious" },
            { "Damage_Reduction_Percentage_Arcane", "Arcane Damage Reduction %" },
            { "Damage_Reduction_Percentage_Elemental", "Elemental Damage Reduction %" },
            { "Damage_Reduction_Percentage_Noxious", "Noxious Damage Reduction %" },
            { "Damage_Reduction_Percentage_Physical", "Physical Damage Reduction %" },
            { "Damage_Reduction_Percentage_Power", "DR Power %" },
            { "Damage_Reduction_Physical", "DR Physical" },
            { "Damage_Reduction_Power", "DR Power" },
            { "Defense", "Defense" },
            { "Defense_Avoidance", "Avoidance" },
            { "Defense_AvoidanceBase", "Avoidance %" },
            { "Defense_AvoidanceBlock", "Block Chance" },
            { "Defense_AvoidanceParry", "Avoidance Parry" },
            { "Defense_Mitigation", "Mitigation" },
            { "Defense_MitigationPercent", "Mitigation %" },
            { "Defense_Toughness", "Toughness" },
            { "Defense_ToughnessPercent", "Toughness %" },
            { "Deflection_Chance", "Deflection Chance" },
            { "Double_Atk_Percent", "Multi Attack" },
            { "DPS", "DPS" },
            { "Dungeons", "Dungeons" },
            { "Elemental", "Elemental Mit" },
            { "Fervor", "Fervor" },
            { "Flurry", "Flurry" },
            { "FlurryMult", "Flurry Multiplier" },
            { "Haste", "Haste" },
            { "Hate_Mod", "Hate" },
            { "HealthRange", "Max Health" },
            { "Houses", "Houses" },
            { "HP_Regen", "HP Regen" },
            { "Intelligence", "Intelligence" },
            { "Lethality", "Lethality" },
            { "LethalityPercent", "Lethality %" },
            { "LifetimeStatus", "Lifetime Status" },
            { "MeleeMult", "Melee Multiplier" },
            { "Noxious", "Noxious Mit" },
            { "Physical", "Physical Mit" },
            { "Potency", "Potency" },
            { "Power", "Power Mit" },
            { "Power_Regen", "Power Regen" },
            { "PowerRange", "Max Mana" },
            { "Primary_Damage_Range", "Primary Dmg" },
            { "Primary_Delay", "Primary Delay" },
            { "PvP_Critical_Mitigation", "PvP Crit Mit" },
            { "PVPSpellDoubleAttack", "PvP Doublecast" },
            { "Ranged_Damage_Range", "Ranged Dmg" },
            { "Ranged_Delay", "Ranged Delay" },
            { "Ranged_Double_Atk_Percent", "Ranged Doublecast" },
            { "Resolve", "Resolve" },
            { "Run_Speed", "Run Speed" },
            { "Secondary_Damage_Range", "Secondary Dmg" },
            { "Secondary_Delay", "Secondary Delay" },
            { "Shield_Effectiveness", "Block %" },
            { "Spell_Cast_Percent", "Casting Speed" },
            { "Spell_Recovery_Percent", "Recovery Speed" },
            { "Spell_Reuse_Percent", "Reuse Speed" },
            { "Spell_Reuse_Spell_Only", "Spell Reuse" },
            { "SpellDoubleAttack", "Spell Doublecast" },
            { "Stamina", "Stamina" },
            { "Strength", "Strength" },
            { "Strikethrough", "Strikethrough" },
            { "TradeskillXPMod", "Tradeskill XP Mod" },
            { "TradeskillXPModCap", "Tradeskill XP Mod Cap" },
            { "Weapon_Damage_Bonus", "Weapon Damage Bonus" },
            { "Wisdom", "Wisdom" }
        };
        private static Dictionary<string, StatterStat> _cachedStats = new Dictionary<string, StatterStat>();

        public StatterStat(string name) : base()
        {
            _name = name;
            _key = GetKeyForStatName(name);
            if (_key == null)
                _key = name.Replace(" ", "").Replace("%", "Pct");

            Colour = DEFAULT_COLOUR;

            // Cache some stats whenever they're created.
            if (!_cachedStats.ContainsKey(_key))
                _cachedStats.Add(_key, this);
        }

        private string GetKeyForStatName(string name)
        {
            foreach (KeyValuePair<string, string> kvp in StatKeyToFriendlyNames)
                if (kvp.Value == name)
                    return kvp.Key;

            return null;
        }

        public override bool Equals(object obj)
        {
            StatterStat other = obj as StatterStat;
            if (other == null) return false;

            return other.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static StatterStat GetStatForKey(string key)
        {
            if (_cachedStats.ContainsKey(key))
                return _cachedStats[key];

            string name = key;
            if (StatKeyToFriendlyNames.ContainsKey(key))
            {
                name = StatKeyToFriendlyNames[key];
            }
            return new StatterStat(name);
        }

        // Return a list of stat names we know how to track, minus the ones specified
        public static List<string> GetAvailableStatNames(List<string> usedStats)
        {
            List<string> availableStats = new List<string>();

            foreach (string stat in StatKeyToFriendlyNames.Values)
                if (!usedStats.Contains(stat))
                    availableStats.Add(stat);

            availableStats.Sort();
            return availableStats;
        }
    }
}
