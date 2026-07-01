using UnityEngine;
using System.Collections.Generic;

public class TechniqueUpgrade : AttributeUpgrade {
    public TechniqueUpgrade(PlayerStats stats) : base(stats)
    {
        name = "Technique";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();
        
        stats.skillEffect.AddModifier(stats.bonusSkillEffect, name);
    }
    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("SkillEffect", stats.bonusSkillEffect, false)
        };
    }
}
