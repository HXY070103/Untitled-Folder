using UnityEngine;

public class PlayerCombat : EntityCombat {
    [Header("Conuter attack info")]
    [SerializeField] private float counterRecoveryDuration = 0.25f;
    [SerializeField] private LayerMask whatIsCounterable;

    private bool canApplyInstantKill = false;

    public override void PerformAttack() {
        base.PerformAttack();

        canApplyInstantKill = false;
    }

    public override void PerformAttackOnTarget(Transform target) {
        base.PerformAttackOnTarget(target);

        IDamageable damageable = target.GetComponent<IDamageable>();

        if (damageable == null) {
            return;
        }

        if (canApplyInstantKill) {
            damageable.TryApplyInstantKill();
        }
    }

    public void SetNextAttackToInstantKill() {
        canApplyInstantKill = true;
    }

    public bool CounterAttackPerformed() {
        bool hasPerformedCounter = false;

        foreach (var target in GetDetectedColliders(whatIsCounterable)) {
            ICounterable counterable = target.GetComponent<ICounterable>();
            
            if (counterable == null) {
                continue;
            }

            if (counterable.CanBeCountered()) {
                counterable.HandleCounter();
                hasPerformedCounter = true;
            }
        }

        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDuration() {
        return counterRecoveryDuration;
    }
}
