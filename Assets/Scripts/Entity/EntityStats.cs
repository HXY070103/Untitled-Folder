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

    public float GetDamage(out bool isCrit) {
        float chance = Mathf.Clamp(critChance.GetValue(), 0, 85);
        isCrit = Random.Range(0, 100) < chance;

        return isCrit ? damage.GetValue() * critPower.GetValue() : damage.GetValue();
    }

    public float GetArmorMitigation() {
        return armor.GetValue() / (armor.GetValue() + 100);
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
