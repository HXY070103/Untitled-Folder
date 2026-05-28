using UnityEngine;

public class VatalityUpgrade : AttributeUpgrade {
    [SerializeField] private float bonusMaxHP = 5;
    [SerializeField] private float bonusArmor = 10;

    public VatalityUpgrade(PlayerStats stats) : base(stats) {
        name = "Vitality";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();

        stats.maxHP.addModifier(bonusMaxHP, name);
        stats.armor.addModifier(bonusArmor, name);
    }
}
