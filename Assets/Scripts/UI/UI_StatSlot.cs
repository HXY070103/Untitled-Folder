using UnityEngine;
using TMPro;
using System.Data;

public class UI_StatSlot : MonoBehaviour
{   
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statName;
    [SerializeField] private TextMeshProUGUI statValue;

    [SerializeField] private EntityStats entityStats;

    private void Start() 
    {
        UpdateStatValue();
    }

    private void OnValidate()
    {
        gameObject.name = "UI_Stat - " + GetStatNameByType(statType);
        statName.text = GetStatNameByType(statType);
    }

    public void UpdateStatValue() 
    {
        if (entityStats == null) return;

        float value = 0f;
        switch (statType) 
        {
            case StatType.MaxHealth:
                value = entityStats.maxHP.GetValue();
                break;
            case StatType.Damage:
                value = entityStats.damage.GetValue(); // Get base damage without crit
                break;
            case StatType.CritChance:
                value = entityStats.critChance.GetValue();
                break;
            case StatType.CritPower:
                value = entityStats.critPower.GetValue() * 100;
                break;
            case StatType.ArmorReduction:
                value = entityStats.GetArmorMitigation() * 100; // Convert to percentage
                break;
            case StatType.Armor:
                value = entityStats.armor.GetValue();
                break;
        }
        statValue.text = value.ToString("F1"); // Format to 1 decimal place
        if (statType == StatType.CritChance || statType == StatType.ArmorReduction || statType == StatType.CritPower) 
        {
            statValue.text += "%"; // Append percentage sign for relevant stats
        }
    }

    private string GetStatNameByType(StatType type) 
    {
        switch (type) 
        {
            case StatType.MaxHealth:
                return "Max HP";
            case StatType.HealthRegen:
                return "HP Regen";
            case StatType.Damage:
                return "Damage";
            case StatType.CritChance:
                return "Crit Chance";
            case StatType.CritPower:
                return "Crit Power";
            case StatType.ArmorReduction:
                return "Armor Reduction";
            case StatType.Armor:
                return "Armor";
            default:
                return "Unknown Stat";
        }
    }
}
