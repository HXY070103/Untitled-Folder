using UnityEngine;

public class TechniqueUpgrade : AttributeUpgrade {
    [SerializeField] private float bonusSkillEffect;

    public TechniqueUpgrade(PlayerStats stats) : base(stats) {}

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();
        
        stats.skillEffect.addModifier(bonusSkillEffect, name);
    }
}
