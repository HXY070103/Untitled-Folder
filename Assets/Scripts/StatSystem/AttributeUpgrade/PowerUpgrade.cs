using UnityEngine;
using System.Collections.Generic;
public class PowerUpgrage : AttributeUpgrade {
    [SerializeField] private float bonusDamage = 10;

    public PowerUpgrage(PlayerStats stats) : base(stats) {
        name = "Power";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();

        stats.damage.AddModifier(bonusDamage, name);
    }

    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("Damage", bonusDamage, false)
        };
    }
}
