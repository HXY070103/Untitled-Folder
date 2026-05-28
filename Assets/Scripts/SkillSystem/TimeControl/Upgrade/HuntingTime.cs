using UnityEngine;

public class HuntingTime : TimeControlUpgrade {
    [SerializeField] private float bonusSkillTime = 1;

    public HuntingTime(SkillTimeControl timeControl) : base(timeControl) {
        rarity = SkillUpgradeRarity.Uncommon;
    }

    public override void ApplyUpgrade() {
        base.ApplyUpgrade();

        if (timeControl.IsSkillActive) {
            timeControl.ExtendSkillTime(bonusSkillTime);
        }
    }
}
