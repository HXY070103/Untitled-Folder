using UnityEngine;

public class PlayerStats : EntityStats {
    [Header("Skill")]
    public Stat skillDuration;
    public Stat skillEffect;

    public float GetSkillDuration() {
        return skillDuration.GetValue();
    }

    public float GetSkillEffect() {
        return skillEffect.GetValue();
    }

    [ContextMenu("Update Skill Default Stat Setup")]
    public void ApplySkillDefualtStatSetup() {
        if (defaultStatSetup == null) {
            Debug.Log("No default stat setup assigned!");
            return;
        }

        skillDuration.SetBaseValue(defaultStatSetup.skillDuration);
        skillEffect.SetBaseValue(defaultStatSetup.skillEffect);
    }
}
