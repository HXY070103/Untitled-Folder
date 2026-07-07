using UnityEngine;

public class EntityStats : MonoBehaviour {
    public StatSetupSO defaultStatSetup;

    [Header("Resource")]
    public Stat maxHP;

    [Header("Offense")]
    public Stat damage;
    public Stat critPower;
    public Stat critChance;


    [Header("Defense")]
    public Stat armor;

    public float GetMaxHP() {
        return maxHP.GetValue();
    }

    public Damage GetDamage() {
        float chance = Mathf.Clamp(critChance.GetValue(), 0, 100);
        bool isCrit = Random.Range(0, 100) < chance;
        float finalDamage = isCrit ? damage.GetValue() * critPower.GetValue() : damage.GetValue();

        if (finalDamage < 5) {
            finalDamage = 5;
        }

        return new Damage(finalDamage, isCrit);
    }

    public float GetArmorMitigation() {
        float finalArmor = armor.GetValue();

        if (finalArmor < 0) {
            finalArmor = 0;
        }

        return finalArmor / (finalArmor + 100);
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefualtStatSetup() {
        if (defaultStatSetup == null) {
            Debug.Log("No default stat setup assigned!");
            return;
        }

        maxHP.SetBaseValue(defaultStatSetup.maxHP);
        
        damage.SetBaseValue(defaultStatSetup.damage);
        critPower.SetBaseValue(defaultStatSetup.critPower);
        critChance.SetBaseValue(defaultStatSetup.critChance);

        armor.SetBaseValue(defaultStatSetup.armor);
    }
}
