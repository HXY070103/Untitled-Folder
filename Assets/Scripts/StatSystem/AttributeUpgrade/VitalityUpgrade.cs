using UnityEngine;
using System.Collections.Generic;

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
    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("MaxHP", bonusMaxHP, false),
            new AttributeUpgradeDisplay("Armor", bonusArmor, false)
        };
    }
}
