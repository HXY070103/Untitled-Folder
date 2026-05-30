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
        float chance = Mathf.Clamp(critChance.GetValue(), 0, 85);
        bool isCrit = Random.Range(0, 100) < chance;
        float finalDamage = isCrit ? damage.GetValue() * critPower.GetValue() : damage.GetValue();

        if (finalDamage < 5) {
            finalDamage = 5;
        }

        return new Damage(finalDamage, isCrit);
    }

    public float GetArmorMitigation() {
        float finalMigitation = armor.GetValue() / (armor.GetValue() + 100);

        if (finalMigitation < 0) {
            finalMigitation = 0;
        }

        return finalMigitation;
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
