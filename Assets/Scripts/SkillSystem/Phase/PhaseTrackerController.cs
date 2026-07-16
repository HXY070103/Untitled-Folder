using UnityEngine;

public class PhaseTrackerController : MonoBehaviour {
    private SkillPhase skill;

    public void SetUpPhaseTracker(SkillPhase skill) {
        this.skill = skill;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        EnemySkillEffectReceiver receiver = collision.GetComponent<EnemySkillEffectReceiver>();

        if (receiver == null) {
            return;
        }

        ApplyWeaknessMarkingEffect(receiver);
    }

    private void ApplyWeaknessMarkingEffect(EnemySkillEffectReceiver receiver) {
        SkillEffectBase debuff = 
            new DamageArmorDebuff("DamageArmorDebuff", skill.damageReduction, skill.armorReduction);
        receiver.ApplySkillEffect(debuff);

        if (skill.Unlocked("Weakness Marking+")) {
            SkillEffectBase damage = new DamageOnce("DamageOnce", skill.damage);
            receiver.ApplySkillEffect(damage);
        }
    }
}
