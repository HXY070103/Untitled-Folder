using UnityEngine;

public class PlayerHealth : EntityHealth {
    private SkillPhase phase;

    private bool isVulnerable = false;
    private float damageMultiplier;

    protected override void Awake() {
        base.Awake();

        phase = GetComponentInChildren<SkillPhase>();
    }

    public override void TakeDamage(Damage damage, Transform damageDealer) {
        if (isVulnerable) {
            damage = new Damage(damage.value * damageMultiplier, damage.isCrit);
        }

        base.TakeDamage(damage, damageDealer);

        if (phase.InPerfectPhaseWindow) {
            phase.PerfectPhaseTriggered();
        }
    }

    public void StartVulnerability(float damageMultiplier) {
        isVulnerable = true;
        this.damageMultiplier = damageMultiplier;
    }

    public void EndVulnerability() {
        isVulnerable = false;
    }
}
