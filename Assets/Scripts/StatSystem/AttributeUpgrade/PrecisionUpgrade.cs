using UnityEngine;
using System.Collections.Generic;
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
    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("CritChance", bonusCritChance, true),
            new AttributeUpgradeDisplay("CritPower", bonusCritPower * 100, true)
        };
    }
}
