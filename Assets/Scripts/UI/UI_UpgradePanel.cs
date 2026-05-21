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
    [SerializeField] private List<AttributeUpgradeDisplayData> attributeUpgrade = new List<AttributeUpgradeDisplayData>();

    [SerializeField] private PlayerSkillManager skillManager;

    private void OnEnable()
    {
        if (skillManager != null)
        {
            SetCurrentSkill(skillManager.GetCurrentSkill());
        } else
        {
            ShowSkillDetails();
        }
    }

    public void ShowSkillDetails()
    {
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

        bool hasEvolution = false;
        foreach (var evo in currentSkill.evolutions)
        {
            if (evo.unlocked)
            {   
                if (!hasEvolution)
                {
                    sb.AppendLine("Evolution Effects:");
                    hasEvolution = true;
                }
                sb.AppendLine($"- {evo.evolutionName}: {evo.changeDescription}");
            }
        }
        contentText.text = sb.ToString();
    }

    public void ShowSkillUpgrades()
    {
        titleText.text = "Skill Upgrades";
        descriptionText.text = "Current skill upgrades:";

        StringBuilder sb = new StringBuilder();
        if (currentSkill == null)
        {
            sb.AppendLine("No skill selected.");
            contentText.text = sb.ToString();
            return;
        }
        if (currentSkill.upgrades == null || currentSkill.upgrades.Count == 0)
        {
            sb.AppendLine("No skill upgrades available.");
            contentText.text = sb.ToString();
            return;
        }
        bool hasUnlockedUpgrade = false;
        foreach (var upgrade in currentSkill.upgrades)
        {
            if (upgrade.unlocked)
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

    public void ShowAttributeUpgrades()
    {
        titleText.text = "Attribute Upgrades";
        descriptionText.text = "Current attribute upgrades:";

        StringBuilder sb = new StringBuilder();
        if (attributeUpgrade.Count == 0)
        {
            sb.AppendLine("No attribute upgrades available.");
        }
        else
        {
            foreach (var attr in attributeUpgrade)
            {
                sb.AppendLine(attr.GetDisplayText());
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
            evolutions = skill.Evolutions != null 
                ? new List<SkillEvolutionDisplayData>(skill.Evolutions)
                : new List<SkillEvolutionDisplayData>(),

            upgrades = skill.Upgrades != null 
                ? new List<SkillUpgradeDisplayData>(skill.Upgrades)
                : new List<SkillUpgradeDisplayData>()
        };
        
        ShowSkillDetails();
    }

    public void SetAttributeUpgrades(List<AttributeUpgradeDisplayData> upgrades)
    {
        attributeUpgrade = upgrades;
        ShowAttributeUpgrades();
    }
}
