using UnityEngine;
using System.Collections.Generic;

public class TechniqueUpgrade : AttributeUpgrade {
    [SerializeField] private float bonusSkillEffect;

    public TechniqueUpgrade(PlayerStats stats) : base(stats) {}

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();
        
        stats.skillEffect.addModifier(bonusSkillEffect, name);
    }
    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("SkillEffect", bonusSkillEffect, false)
        };
    }
}
