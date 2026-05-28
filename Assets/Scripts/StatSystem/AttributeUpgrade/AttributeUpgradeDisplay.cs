using UnityEngine;

[System.Serializable]
public class AttributeUpgradeDisplay
{
    public string attributeName;
    public float value;
    public bool isPercent;

    public AttributeUpgradeDisplay(string attributeName, float value, bool isPercent = true)
    {
        this.attributeName = attributeName;
        this.value = value;
        this.isPercent = isPercent;
    }

    public string GetValueText()
    {
        return isPercent ? $"{value}%" : value.ToString();
    }
}