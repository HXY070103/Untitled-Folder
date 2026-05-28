using UnityEngine;

public class PowerUpgrage : AttributeUpgrade {
    [SerializeField] private float bonusDamage = 10;

    public PowerUpgrage(PlayerStats stats) : base(stats) {
        name = "Power";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();

        stats.damage.addModifier(bonusDamage, name);
    }
}
