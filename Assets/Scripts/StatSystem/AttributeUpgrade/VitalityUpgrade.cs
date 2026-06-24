using UnityEngine;
using System.Collections.Generic;

public class VatalityUpgrade : AttributeUpgrade {
    public VatalityUpgrade(PlayerStats stats) : base(stats) {
        name = "Vitality";
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();

        PlayerHealth health = stats.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.IncreaseMaxHP(stats.bonusMaxHP, name);
        } else
        {
            stats.maxHP.AddModifier(stats.bonusMaxHP, name);
        }
        stats.armor.AddModifier(stats.bonusArmor, name);
    }
    public override List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>
        {
            new AttributeUpgradeDisplay("MaxHP", stats.bonusMaxHP, false),
            new AttributeUpgradeDisplay("Armor", stats.bonusArmor, false)
        };
    }
}
