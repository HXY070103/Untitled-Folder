using UnityEngine;

public class PrecisionUpgrade : AttributeUpgrade {
    [SerializeField] private float bonusCritPower = 0.2f;
    [SerializeField] private float bonusCritChance = 3;

    public PrecisionUpgrade(PlayerStats stats) : base(stats) {
        name = "Precision";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();
        
        stats.critPower.addModifier(bonusCritPower, name);
        stats.critChance.addModifier(bonusCritChance, name);
    }
}
