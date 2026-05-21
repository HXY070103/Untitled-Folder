using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class SkillDisplayData
{
    public string skillName;
    [TextArea]
    public string description;
    public List<SkillEvolutionDisplayData> evolutions = new List<SkillEvolutionDisplayData>();
    public List<SkillUpgradeDisplayData> upgrades = new List<SkillUpgradeDisplayData>();
}