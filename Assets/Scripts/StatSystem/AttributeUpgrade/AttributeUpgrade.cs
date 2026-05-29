using UnityEngine;
using System.Collections.Generic;
using System.Linq;
[System.Serializable]
public class AttributeUpgrade {
    protected string name;
    protected PlayerStats stats;
    public string Upgradename => name;
    
    public AttributeUpgrade(PlayerStats stats) {
        this.stats = stats;
    }
    public virtual void ApplyUpgrade() {}

    public virtual List<AttributeUpgradeDisplay> GetDisplayEffects()
    {
        return new List<AttributeUpgradeDisplay>();
    }
}
