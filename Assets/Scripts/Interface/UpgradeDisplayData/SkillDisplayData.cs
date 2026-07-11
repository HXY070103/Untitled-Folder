using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class SkillDisplayData
{
    public string skillName;
    [TextArea]
    public string description;
    public List<SkillUpgradeDisplayData> upgrades = new List<SkillUpgradeDisplayData>();
    public List<SkillUpgradeDisplayData> betterUpgrades = new List<SkillUpgradeDisplayData>();
    public List<SkillUpgradeDisplayData> skillPhaseUpgrades = new List<SkillUpgradeDisplayData>();
}
