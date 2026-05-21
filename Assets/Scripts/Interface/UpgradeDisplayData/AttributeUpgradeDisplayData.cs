using UnityEngine;
using System.Collections.Generic;
using System.Linq;
[System.Serializable]
public class AttributeUpgradeDisplayData
{
    public string attributeName;
    public List<float> percentages = new List<float>();
    public float GetTotalPercent()
    {
        return percentages.Sum();
    }
    public string GetDisplayText()
    {
        float total = GetTotalPercent();
        string detail = string.Join(" + ", percentages.Select(p => $"{p}%"));
        return $"{attributeName} increases {total}% ({detail})";
    }

}