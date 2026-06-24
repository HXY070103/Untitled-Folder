using UnityEngine;
using System.Collections.Generic;
public class PrecisionUpgrade : AttributeUpgrade {
    public PrecisionUpgrade(PlayerStats stats) : base(stats) {
        name = "Precision";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();
        
        stats.critPower.AddModifier(stats.bonusCritPower, name);
        stats.critChance.AddModifier(stats.bonusCritChance, name);
    }
    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("CritChance", stats.bonusCritChance, true),
            new AttributeUpgradeDisplay("CritPower", stats.bonusCritPower * 100, true)
        };
    }
}
