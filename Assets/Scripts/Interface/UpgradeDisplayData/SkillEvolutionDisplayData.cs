using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SkillEvolutionDisplayData
{
    public string evolutionName;
    [TextArea]
    public string changeDescription;
    public bool unlocked;
}