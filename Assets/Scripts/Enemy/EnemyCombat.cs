using UnityEngine;

public class EnemyCombat : EntityCombat {
    public override void PerformAttackOnTarget(Transform target) {
        base.PerformAttackOnTarget(target);

        HurtCheckerController hurtChecker = target.GetComponent<HurtCheckerController>();

        if (hurtChecker == null) {
            return;
        }

        hurtChecker.GetHurt();
    }
}
