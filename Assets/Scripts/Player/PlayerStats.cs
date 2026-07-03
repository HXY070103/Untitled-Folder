using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : EntityStats, ISaveable {
    [Header("Skill")]
    public Stat skillEffect;
    private float level2Skill = 1f;
    private float level6Skill = 1f;
    private float levelFullSkill = 1f;

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
        return skillEffect.GetValue() * level2Skill;
    }

    public override float GetMaxHP()
    {
        return maxHP.GetValue() * level6Skill * levelFullSkill;
    }
    public float GetFinalDamage()
    {
        return damage.GetValue() * level6Skill * levelFullSkill;
    }
    public float GetCritPower()
    {
        return critPower.GetValue() * level6Skill * levelFullSkill;
    }
    public float GetCritChance()
    {
        return critChance.GetValue() * level6Skill * levelFullSkill;
    }
    public float GetArmor()
    {
        return armor.GetValue() * level6Skill * levelFullSkill;
    }
    public override Damage GetDamage() {
        float chance = Mathf.Clamp(GetCritChance(), 0, 100);
        bool isCrit = Random.Range(0, 100) < chance;
        float finalDamage = isCrit ? GetFinalDamage() * GetCritPower() : GetFinalDamage();

        if (finalDamage < 5) {
            finalDamage = 5;
        }

        return new Damage(finalDamage, isCrit);
    }

    public override float GetArmorMitigation() {
        float finalArmor = GetArmor();

        if (finalArmor < 0) {
            finalArmor = 0;
        }

        return finalArmor / (finalArmor + 100);
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

    public void ReachLevel2Skill()
    {
        level2Skill = 1.2f;
    }

    public void ReachLevel6Skill()
    {
        level6Skill = 1.1f;
    }
    public void ReachFullLevel()
    {
        levelFullSkill = 1.2f;
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
