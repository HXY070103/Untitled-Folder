using UnityEngine;

public class AttributeUpgrade {
    protected string name;
    protected PlayerStats stats;

    public AttributeUpgrade(PlayerStats stats) {
        this.stats = stats;
    }

    public virtual void ApplyUpgrade() {}
}
