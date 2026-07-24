using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class UI_UpgradePanel : MonoBehaviour
{
    private const float ContentTopInset = 150f;
    private const float ContentSideInset = 20f;
    private const float ContentBottomInset = 20f;

    [Header("Right Detial Texts")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("Display Data")]
    [SerializeField] private SkillDisplayData currentSkill;
    [SerializeField] private PlayerStats playerStats;
    private List<AttributeUpgrade> attributeUpgrades = new List<AttributeUpgrade>();

    [SerializeField] private PlayerSkillManager skillManager;

    private ScrollRect contentScrollRect;
    private RectTransform contentViewport;

    private void Awake()
    {
        SetupScrollableContent();
    }

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
            SetContent(sb.ToString());
            return;
        }
        sb.AppendLine($"[{currentSkill.skillName}]");
        sb.AppendLine(currentSkill.description);
        sb.AppendLine();

        SetContent(sb.ToString());
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
            SetContent(sb.ToString());
            return;
        }
        if ((currentSkill.upgrades == null || currentSkill.upgrades.Count == 0) 
            && (currentSkill.betterUpgrades == null || currentSkill.betterUpgrades.Count == 0))
        {
            sb.AppendLine("No skill upgrades available.");
            SetContent(sb.ToString());
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
        SetContent(sb.ToString());
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
            SetContent(sb.ToString());
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
            SetContent(sb.ToString());
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

        SetContent(sb.ToString());
    }

    private void SetupScrollableContent()
    {
        if (contentText == null || contentScrollRect != null)
        {
            return;
        }

        RectTransform detailPanel = contentText.rectTransform.parent as RectTransform;
        if (detailPanel == null)
        {
            return;
        }

        GameObject viewportObject = new GameObject(
            "ContentViewport",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(RectMask2D),
            typeof(ScrollRect)
        );

        contentViewport = viewportObject.GetComponent<RectTransform>();
        contentViewport.SetParent(detailPanel, false);
        contentViewport.SetSiblingIndex(contentText.rectTransform.GetSiblingIndex());
        contentViewport.anchorMin = Vector2.zero;
        contentViewport.anchorMax = Vector2.one;
        contentViewport.offsetMin = new Vector2(ContentSideInset, ContentBottomInset);
        contentViewport.offsetMax = new Vector2(-ContentSideInset, -ContentTopInset);

        Image viewportImage = viewportObject.GetComponent<Image>();
        viewportImage.color = Color.clear;
        viewportImage.raycastTarget = true;

        RectTransform contentRect = contentText.rectTransform;
        contentRect.SetParent(contentViewport, false);
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;

        contentText.alignment = TextAlignmentOptions.TopLeft;
        contentText.margin = new Vector4(8f, 8f, 8f, 8f);

        contentScrollRect = viewportObject.GetComponent<ScrollRect>();
        contentScrollRect.content = contentRect;
        contentScrollRect.viewport = contentViewport;
        contentScrollRect.horizontal = false;
        contentScrollRect.vertical = true;
        contentScrollRect.movementType = ScrollRect.MovementType.Clamped;
        contentScrollRect.inertia = true;
        contentScrollRect.scrollSensitivity = 30f;
    }

    private void SetContent(string value)
    {
        contentText.text = value;
        RefreshContentLayout();
    }

    private void RefreshContentLayout()
    {
        if (contentScrollRect == null || contentViewport == null)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();

        RectTransform contentRect = contentText.rectTransform;
        float availableWidth = Mathf.Max(
            0f,
            contentViewport.rect.width - contentText.margin.x - contentText.margin.z
        );
        float preferredHeight = contentText.GetPreferredValues(
            contentText.text,
            availableWidth,
            Mathf.Infinity
        ).y + contentText.margin.y + contentText.margin.w;

        contentRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            Mathf.Max(contentViewport.rect.height, preferredHeight)
        );
        contentRect.anchoredPosition = Vector2.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        contentScrollRect.verticalNormalizedPosition = 1f;
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
