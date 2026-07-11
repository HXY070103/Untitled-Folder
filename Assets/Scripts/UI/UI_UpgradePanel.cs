using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class UI_UpgradePanel : MonoBehaviour
{
    [Header("Right Detial Texts")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("Display Data")]
    [SerializeField] private SkillDisplayData currentSkill;
    [SerializeField] private PlayerStats playerStats;
    private List<AttributeUpgrade> attributeUpgrades = new List<AttributeUpgrade>();

    [SerializeField] private PlayerSkillManager skillManager;

    private void OnEnable()
    {
        if (Player.instance != null)
        {
            playerStats = Player.instance.stats;
            skillManager = Player.instance.skillManager;
        }
        if (skillManager != null)
        {
            SetCurrentSkill(skillManager.GetCurrentSkill());
        } else
        {
            ShowSkillDetails();
        }
        if (playerStats != null)
        {
            SetAttributeUpgrades(playerStats.ObtainAttributeUpgrades);
        }
    }

    public void ShowSkillDetails()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        titleText.text = "Skill Details";
        descriptionText.text = "Current skill and its Evolutions:";

        StringBuilder sb = new StringBuilder();

        if (currentSkill == null)
        {
            sb.AppendLine("No skill selected.");
            contentText.text = sb.ToString();
            return;
        }
        sb.AppendLine($"[{currentSkill.skillName}]");
        sb.AppendLine(currentSkill.description);
        sb.AppendLine();

        contentText.text = sb.ToString();
    }

    public void ShowSkillUpgrades()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        titleText.text = "Skill Upgrades";
        descriptionText.text = "Current skill upgrades:";

        StringBuilder sb = new StringBuilder();
        if (currentSkill == null)
        {
            sb.AppendLine("No skill selected.");
            contentText.text = sb.ToString();
            return;
        }
        if ((currentSkill.upgrades == null || currentSkill.upgrades.Count == 0) 
            && (currentSkill.betterUpgrades == null || currentSkill.betterUpgrades.Count == 0))
        {
            sb.AppendLine("No skill upgrades available.");
            contentText.text = sb.ToString();
            return;
        }
        bool hasUnlockedUpgrade = false;

        foreach (var upgrade in currentSkill.skillPhaseUpgrades)
        {
            if (upgrade.upgradeName.Equals("4 Upgrades") && upgrade.unlocked)
            {
                hasUnlockedUpgrade = true;
                sb.AppendLine($"[{upgrade.upgradeName}]");
                sb.AppendLine(upgrade.description);
                sb.AppendLine();
            }
        }

        foreach (var upgrade in currentSkill.betterUpgrades)
        {
            if (upgrade.unlocked)
            {
                hasUnlockedUpgrade = true;
                sb.AppendLine($"[{upgrade.upgradeName}]");
                sb.AppendLine(upgrade.description);
                sb.AppendLine();
            }
        }
        foreach (var upgrade in currentSkill.upgrades)
        {
            if (upgrade.unlocked && !checkIfBetterUnlocked(upgrade.upgradeName + "+"))
            {
                hasUnlockedUpgrade = true;
                sb.AppendLine($"[{upgrade.upgradeName}]");
                sb.AppendLine(upgrade.description);
                sb.AppendLine();
            }
            
        }
        if (!hasUnlockedUpgrade)
        {
            sb.AppendLine("No skill upgrades yet");
        }
        contentText.text = sb.ToString();
    }

    private bool checkIfBetterUnlocked(string name)
    {
        foreach(var upgrade in currentSkill.betterUpgrades)
        {
            if (upgrade.upgradeName.Equals(name))
            {
                return upgrade.unlocked;
            }
        }
        return false;
    }

    public void ShowAttributeUpgrades()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        titleText.text = "Attribute Upgrades";
        descriptionText.text = "Current attribute upgrades:";

        StringBuilder sb = new StringBuilder();

        if (currentSkill == null)
        {
            sb.AppendLine("No skill selected.");
            contentText.text = sb.ToString();
            return;
        }

        foreach (var upgrade in currentSkill.skillPhaseUpgrades)
        {
            if (!upgrade.upgradeName.Equals("4 Upgrades") && upgrade.unlocked)
            {
                sb.AppendLine($"[{upgrade.upgradeName}]");
                sb.AppendLine(upgrade.description);
                sb.AppendLine();
            }
        }

        if (attributeUpgrades == null || attributeUpgrades.Count == 0)
        {
            sb.AppendLine("No attribute upgrades yet.");
            contentText.text = sb.ToString();
            return;
        }

        Dictionary<string, List<string>> detailMap = new Dictionary<string, List<string>>();
        Dictionary<string, float> totalMap = new Dictionary<string, float>();
        Dictionary<string, bool> percentMap = new Dictionary<string, bool>();

        foreach (AttributeUpgrade upgrade in attributeUpgrades)
        {
            foreach (AttributeUpgradeDisplay effect in upgrade.GetDisplayEffects())
            {
                if (!detailMap.ContainsKey(effect.attributeName))
                {
                    detailMap[effect.attributeName] = new List<string>();
                    totalMap[effect.attributeName] = 0f;
                    percentMap[effect.attributeName] = effect.isPercent;
                }

                detailMap[effect.attributeName].Add(effect.GetValueText());
                totalMap[effect.attributeName] += effect.value;
            }
        }

        foreach (var pair in detailMap)
        {
            string attributeName = pair.Key;
            string detail = string.Join(" + ", pair.Value);
            float total = totalMap[attributeName];

            if (percentMap[attributeName])
            {
                sb.AppendLine($"{attributeName} increases {total}% ({detail})");
            }
            else
            {
                sb.AppendLine($"{attributeName} increases {total} ({detail})");
            }
        }

        contentText.text = sb.ToString();
    }

    public void SetCurrentSkill(SkillBase skill)
    {
        if (skill == null)
        {
            currentSkill = null;
            ShowSkillDetails();
            return;
        }
        currentSkill = new SkillDisplayData
        {
            skillName = skill.SkillName,
            description = skill.Description,

            upgrades = skill.Upgrades != null 
                ? new List<SkillUpgradeDisplayData>(skill.Upgrades)
                : new List<SkillUpgradeDisplayData>(),

            betterUpgrades = skill.BetterUpgrades != null
                ? new List<SkillUpgradeDisplayData>(skill.BetterUpgrades)
                : new List<SkillUpgradeDisplayData>(),

            skillPhaseUpgrades = skill.SkillPhaseUpgrades != null
                ? new List<SkillUpgradeDisplayData>(skill.SkillPhaseUpgrades)
                : new List<SkillUpgradeDisplayData>()
        };
        
        ShowSkillDetails();
    }

    public void SetAttributeUpgrades(List<AttributeUpgrade> upgrades)
    {
        attributeUpgrades = upgrades;
    }
}
