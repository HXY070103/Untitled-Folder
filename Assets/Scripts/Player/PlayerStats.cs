using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats {
    private AttributeUpgrade[] upgrades;

    [Header("Skill")]
    public Stat skillEffect;

    public float GetSkillEffect() {
        return skillEffect.GetValue();
    }

    private void Awake() {
        upgrades = new AttributeUpgrade[4];
        
        upgrades[(int) AttributeUpgrageType.Vatality] = new VatalityUpgrade(this);
        upgrades[(int) AttributeUpgrageType.Power] = new PowerUpgrage(this);
        upgrades[(int) AttributeUpgrageType.Precision] = new PrecisionUpgrade(this);
        upgrades[(int) AttributeUpgrageType.Technique] = new TechniqueUpgrade(this);
    }

    public void addUpgrade(AttributeUpgrageType upgrageType) {
        upgrades[(int) upgrageType].ApplyUpgrade();
    }

    [ContextMenu("Update Skill Default Stat Setup")]
    public void ApplySkillDefualtStatSetup() {
        if (defaultStatSetup == null) {
            Debug.Log("No default stat setup assigned!");
            return;
        }

        skillEffect.SetBaseValue(defaultStatSetup.skillEffect);
    }
}
