using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ChestUpgradeOptionType
{
    Attribute,
    Skill
}

public class ChestUpgradeOption
{
    public ChestUpgradeOptionType optionType;

    public AttributeUpgrade attributeUpgrade;
    public PlayerVFX vfx;
    public PlayerStats playerStats;
    public SkillBase skill;
    public SkillUpgradeDisplayData skillUpgrade;
    public bool isBetter;

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
            vfx.GenerateParticleEffect(Color.red);
            playerStats.AddUpgrade(attributeUpgrade);
            return;
        }

        vfx.GenerateParticleEffect(Color.yellow);
        skill.UnlockSkillUpgrade(skillUpgrade.upgradeName);
    }
}