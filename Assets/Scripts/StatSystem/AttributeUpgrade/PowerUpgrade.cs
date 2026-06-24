using UnityEngine;
using System.Collections.Generic;
public class PowerUpgrage : AttributeUpgrade {
    public PowerUpgrage(PlayerStats stats) : base(stats) {
        name = "Power";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();

        stats.damage.AddModifier(stats.bonusDamage, name);
    }

    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("Damage", stats.bonusDamage, false)
        };
    }
}
