using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : EntityStats, ISaveable {
    [Header("Skill")]
    public Stat skillEffect;

    [Header("Upgrade info")]

    public float bonusMaxHP = 20;
    public float bonusDamage = 30;
    public float bonusCritPower = 0.5f;
    public float bonusCritChance = 10;
    public float bonusArmor = 40;
    public float bonusSkillEffect = 0.4f;

    private AttributeUpgrade[] upgrades;
    private List<AttributeUpgrade> obtainAttributeUpgrades = new List<AttributeUpgrade>();
    public List<AttributeUpgrade> ObtainAttributeUpgrades => obtainAttributeUpgrades;

    public float GetSkillEffect() {
        return skillEffect.GetValue();
    }

    private void Awake() {
        InitializeAttributeUpgrades();
    }
    
    private void InitializeAttributeUpgrades()
    {
        upgrades = new AttributeUpgrade[4];

        upgrades[(int) AttributeUpgrageType.Vatality] = new VatalityUpgrade(this);
        upgrades[(int) AttributeUpgrageType.Power] = new PowerUpgrage(this);
        upgrades[(int) AttributeUpgrageType.Precision] = new PrecisionUpgrade(this);
        upgrades[(int) AttributeUpgrageType.Technique] = new TechniqueUpgrade(this);
    }

    private void ClearAllAttributeUpgradeModifiers()
    {
        if (upgrades == null)
        {
            InitializeAttributeUpgrades();
        }

        foreach (AttributeUpgrade upgrade in upgrades)
        {
            if (upgrade == null)
            {
                continue;
            }

            string source = upgrade.Upgradename;

            maxHP.RemoveModifier(source);
            damage.RemoveModifier(source);
            critPower.RemoveModifier(source);
            critChance.RemoveModifier(source);
            armor.RemoveModifier(source);
            skillEffect.RemoveModifier(source);
        }
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

    public void ReachLevel2Skill()
    {
        skillEffect.AddMultiplier(1.2f, "Level2Skill");
    }

    public void ReachLevel6Skill()
    {
        maxHP.AddMultiplier(1.1f, "Level6Skill");
        damage.AddMultiplier(1.1f, "Level6Skill");
        critChance.AddMultiplier(1.1f, "Level6Skill");
        critPower.AddMultiplier(1.1f, "Level6Skill");
        armor.AddMultiplier(1.1f, "Level6Skill");
    }
    public void ReachFullLevel()
    {
        maxHP.AddMultiplier(1.2f, "LevelFullSkill");
        damage.AddMultiplier(1.2f, "LevelFullSkill");
        critChance.AddMultiplier(1.2f, "LevelFullSkill");
        critPower.AddMultiplier(1.2f, "LevelFullSkill");
        armor.AddMultiplier(1.2f, "LevelFullSkill");
    }

    public void RestoreSkillPhaseBonuses(bool level2Unlocked, bool level6Unlocked, bool fullUnlocked)
    {
        skillEffect.RemoveModifier("Level2Skill");

        maxHP.RemoveModifier("Level6Skill");
        damage.RemoveModifier("Level6Skill");
        critChance.RemoveModifier("Level6Skill");
        critPower.RemoveModifier("Level6Skill");
        armor.RemoveModifier("Level6Skill");

        maxHP.RemoveModifier("LevelFullSkill");
        damage.RemoveModifier("LevelFullSkill");
        critChance.RemoveModifier("LevelFullSkill");
        critPower.RemoveModifier("LevelFullSkill");
        armor.RemoveModifier("LevelFullSkill");

        if (level2Unlocked)
        {
            ReachLevel2Skill();
        }
        if (level6Unlocked)
        {
            ReachLevel6Skill();
        }
        if (fullUnlocked)
        {
            ReachFullLevel();
        }
    }

    public void LoadData(GameData data)
    {
        if (upgrades == null)
        {
            InitializeAttributeUpgrades();
        }
        ClearAllAttributeUpgradeModifiers();    
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
