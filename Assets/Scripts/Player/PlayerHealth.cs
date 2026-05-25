using UnityEngine;

public class PlayerHealth : EntityHealth {
    private SkillPhase phase;

    protected override void Awake() {
        base.Awake();

        phase = GetComponentInChildren<SkillPhase>();
    }

    public override void TakeDamage(Damage damage, Transform damageDealer) {
        base.TakeDamage(damage, damageDealer);

        if (phase.InPerfectPhaseWindow) {
            phase.PerfectPhaseTriggered();
        }
    }
}
