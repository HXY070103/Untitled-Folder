using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : EntityStats, ISaveable {
    [Header("Player specific ")]
    public Stat skillEffect;
    public Stat attackSpeed;

    [Header("Upgrade info")]
    public float bonusMaxHP = 20;
    public float bonusDamage = 30;
    public float bonusCritPower = 0.5f;
    public float bonusCritChance = 10;
    public float bonusArmor = 40;
    public float bonusSkillEffect = 0.4f;

    [Header("Level upgrade info")]
    [SerializeField] private float level2Skill = 1.2f;
    [SerializeField] private float level6Skill = 1.1f;
    [SerializeField] private float levelFullSkill = 1.2f;

    private AttributeUpgrade[] upgrades;
    private List<AttributeUpgrade> obtainAttributeUpgrades = new List<AttributeUpgrade>();
    public List<AttributeUpgrade> ObtainAttributeUpgrades => obtainAttributeUpgrades;

    public float GetSkillEffect() {
        return skillEffect.GetValue();
    }

    public float GetAttackSpeed() {
        return attackSpeed.GetValue();
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
            attackSpeed.RemoveModifier(source);
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
    
    public AttributeUpgrade[] GetAttributeUpgrades() {
        return upgrades;
    }

    public void ReachLevel2Skill() {
        skillEffect.AddMultiplier(level2Skill, "Level2Skill");
    }

    public void ReachLevel6Skill() {
        maxHP.AddMultiplier(level6Skill, "Level6Skill");
        damage.AddMultiplier(level6Skill, "Level6Skill");
        critChance.AddMultiplier(level6Skill, "Level6Skill");
        critPower.AddMultiplier(level6Skill, "Level6Skill");
        armor.AddMultiplier(level6Skill, "Level6Skill");
        attackSpeed.AddMultiplier(level6Skill, "Level6Skill");
    }

    public void ReachFullLevel() {
        maxHP.AddMultiplier(levelFullSkill, "LevelFullSkill");
        damage.AddMultiplier(levelFullSkill, "LevelFullSkill");
        critChance.AddMultiplier(levelFullSkill, "LevelFullSkill");
        critPower.AddMultiplier(levelFullSkill, "LevelFullSkill");
        armor.AddMultiplier(levelFullSkill, "LevelFullSkill");
        attackSpeed.AddMultiplier(levelFullSkill, "LevelFullSkill");
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

        if (level2Unlocked) ReachLevel2Skill();
        if (level6Unlocked) ReachLevel6Skill();
        if (fullUnlocked) ReachFullLevel();
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

        if (level2Unlocked) ReachLevel2Skill();
        if (level6Unlocked) ReachLevel6Skill();
        if (fullUnlocked) ReachFullLevel();
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
