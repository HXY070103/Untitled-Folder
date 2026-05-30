using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats {
    private AttributeUpgrade[] upgrades;
    private List<AttributeUpgrade> obtainAttributeUpgrades = new List<AttributeUpgrade>();
    public List<AttributeUpgrade> ObtainAttributeUpgrades => obtainAttributeUpgrades;

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
        
        AttributeUpgrade upgrade = upgrades[(int) upgrageType];
        if (upgrade == null)
        {
            Debug.LogError("Attribute upgrade is null: " + upgrageType);
            return;
        }
        upgrade.ApplyUpgrade();
        obtainAttributeUpgrades.Add(upgrade);
    }

    [ContextMenu("Update Skill Default Stat Setup")]
    public void ApplySkillDefualtStatSetup() {
        if (defaultStatSetup == null) {
            Debug.Log("No default stat setup assigned!");
            return;
        }

        skillEffect.SetBaseValue(defaultStatSetup.skillEffect);
    }

    private void Update()
{
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
        addUpgrade(AttributeUpgrageType.Vatality);
        Debug.Log("Added Vatality Upgrade");
    }
}
}
