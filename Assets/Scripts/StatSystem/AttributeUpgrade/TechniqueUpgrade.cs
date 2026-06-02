using UnityEngine;
using System.Collections.Generic;

public class TechniqueUpgrade : AttributeUpgrade {
    [SerializeField] private float bonusSkillEffect = 0.1f;

    public TechniqueUpgrade(PlayerStats stats) : base(stats)
    {
        name = "Technique";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();
        
        stats.skillEffect.AddModifier(bonusSkillEffect, name);
    }
    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("SkillEffect", bonusSkillEffect, false)
        };
    }
}
