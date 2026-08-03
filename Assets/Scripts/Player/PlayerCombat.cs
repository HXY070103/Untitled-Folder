using UnityEngine;

public class PlayerCombat : EntityCombat {
    private SkillPhase phase;
    private bool canExecute = false;

    [Header("Conuter attack info")]
    [SerializeField] private float counterRecoveryDuration = 0.25f;
    [SerializeField] private LayerMask whatIsCounterable;

    protected override void Awake() {
        base.Awake();

        phase = GetComponentInChildren<SkillPhase>();
    }

    public void SetCanExecute(bool canExecute) {
        this.canExecute = canExecute;
    }

    public override void PerformAttackOnTarget(Transform target) {
        base.PerformAttackOnTarget(target);

        if (!canExecute) {
            return;
        }

        IDamageable damageable = target.GetComponent<IDamageable>();
            
        if (damageable == null) {
            return;
        }

        damageable.TryToExecute(phase.GetHPThreshold(), phase.GetBonusDamage());
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
