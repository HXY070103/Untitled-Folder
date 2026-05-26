using UnityEngine;

public class TimeControlUpgrade {
    protected SkillTimeControl timeControl;
    protected SkillUpgradeRarity rarity;

    public TimeControlUpgrade(SkillTimeControl timeControl) {
        this.timeControl = timeControl;
    }
    
    public virtual void ApplyUpgrade() {}
}
