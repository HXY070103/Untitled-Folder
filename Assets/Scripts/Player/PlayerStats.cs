using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : EntityStats, ISaveable {
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

    public void AddUpgrade(AttributeUpgrade upgrade) {
        if (upgrade == null)
        {
            Debug.LogError("Attribute upgrade is null: " + upgrade);
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
    
    public AttributeUpgrade[] GetAttributeUpgrades() {
        return upgrades;
    }

    public void LoadData(GameData data)
    {
        obtainAttributeUpgrades.Clear();

        foreach(AttributeUpgradeSaveData saveData in data.attributeUpgrades)
        {
            AttributeUpgrageType type = (AttributeUpgrageType) saveData.upgradeType;
            if (type < 0 || (int) type >= upgrades.Length)
            {
                continue;
            }
            AttributeUpgrade upgrade = upgrades[(int) type];
            if (upgrade == null)
            {
                continue;
            }
            for (int i = 0; i < saveData.count; ++i)
            {
                AddUpgrade(upgrade);
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.attributeUpgrades.Clear();

        for (int i = 0; i < upgrades.Length; ++i)
        {
            AttributeUpgrade upgrade = upgrades[i];
            if (upgrade == null)
            {
                continue;
            }
            int count = obtainAttributeUpgrades.Count(x => x.Upgradename == upgrade.Upgradename);
            if (count <= 0)
            {
                continue;
            }
            AttributeUpgradeSaveData saveData = new AttributeUpgradeSaveData();
            saveData.upgradeType = i;
            saveData.count = count;

            data.attributeUpgrades.Add(saveData);
        }
    }
}
