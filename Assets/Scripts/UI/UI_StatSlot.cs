using UnityEngine;
using TMPro;
using System.Data;

public class UI_StatSlot : MonoBehaviour
{   
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statName;
    [SerializeField] private TextMeshProUGUI statValue;

    private PlayerStats playerStats;

    private void Start() 
    {
        FindPlayerStat();
        UpdateStatValue();
    }

    private void OnEnable()
    {
        FindPlayerStat();
        UpdateStatValue();
    }

    private void OnValidate()
    {
        gameObject.name = "UI_Stat - " + GetStatNameByType(statType);
        statName.text = GetStatNameByType(statType);
    }

    private void FindPlayerStat()
    {
        if (Player.instance != null)
        {
            playerStats = Player.instance.stats;
        }
    }

    public void UpdateStatValue() 
    {
        if (playerStats == null) return;

        float value = 0f;
        switch (statType) 
        {
            case StatType.MaxHealth:
                value = playerStats.GetMaxHP();
                break;
            case StatType.Damage:
                value = playerStats.GetFinalDamage(); // Get base damage without crit
                break;
            case StatType.CritChance:
                value = Mathf.Clamp(playerStats.GetCritChance(), 0, 100);
                break;
            case StatType.CritPower:
                value = playerStats.GetCritPower() * 100;
                break;
            case StatType.ArmorReduction:
                value = playerStats.GetArmorMitigation() * 100; // Convert to percentage
                break;
            case StatType.Armor:
                value = playerStats.GetArmor();
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
