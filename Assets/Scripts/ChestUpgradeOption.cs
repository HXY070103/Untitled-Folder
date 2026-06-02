using System.Collections.Generic;
using System.Text;

public enum ChestUpgradeOptionType
{
    Attribute,
    Skill
}

public class ChestUpgradeOption
{
    public ChestUpgradeOptionType optionType;

    public AttributeUpgrade attributeUpgrade;
    public PlayerStats playerStats;
    public SkillBase skill;
    public SkillUpgradeDisplayData skillUpgrade;

    public string GetTitle()
    {
        if (optionType == ChestUpgradeOptionType.Attribute)
        {
            return attributeUpgrade.Upgradename;
        }

        return skillUpgrade.upgradeName;
    }

    public string GetDescription()
    {
        if (optionType == ChestUpgradeOptionType.Skill)
        {
            return skillUpgrade.description;
        }

        List<AttributeUpgradeDisplay> effects = attributeUpgrade.GetDisplayEffects();

        StringBuilder builder = new StringBuilder();

        foreach (AttributeUpgradeDisplay effect in effects)
        {
            builder.Append(effect.attributeName);
            builder.Append(" +");
            builder.Append(effect.GetValueText());
            builder.AppendLine();
        }

        return builder.ToString();
    }

    public void Apply()
    {
        if (optionType == ChestUpgradeOptionType.Attribute)
        {
            playerStats.addUpgrade(attributeUpgrade);
            return;
        }

        skill.UnlockSkillUpgrade(skillUpgrade.upgradeName);
    }
}