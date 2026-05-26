using UnityEngine;

public class TemporalFreeze : TimeControlUpgrade {
    public TemporalFreeze(SkillTimeControl timeControl) : base(timeControl) {
        rarity = SkillUpgradeRarity.Rare;
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();

        timeControl.FreezeDomain();
    }
}
