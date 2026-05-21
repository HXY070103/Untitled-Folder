using UnityEngine;
using System.Collections.Generic;
using System.Linq;
[System.Serializable]
public class SkillUpgradeDisplayData
{
    public string upgradeName;
    [TextArea]
    public string description;

    public bool unlocked;
}