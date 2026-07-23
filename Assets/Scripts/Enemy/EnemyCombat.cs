using UnityEngine;

public class EnemyCombat : EntityCombat {
    public override void PerformAttackOnTarget(Transform target) {
        PerformAttackOnTarget(target, 1f);
    }

    public override void PerformAttackOnTarget(Transform target, float damageMultiplier) {
        base.PerformAttackOnTarget(target, damageMultiplier);

        HurtCheckerController hurtChecker = target.GetComponent<HurtCheckerController>();

        if (hurtChecker == null) {
            return;
        }

        hurtChecker.GetHurt();
    }
}
