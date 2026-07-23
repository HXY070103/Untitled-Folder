using UnityEngine;

public class PlayerCombat : EntityCombat {
    private SkillPhase phase;
    private bool canExecute = false;

    [Header("Conuter attack info")]
    [SerializeField] private float counterRecoveryDuration = 0.25f;
    [SerializeField] private LayerMask whatIsCounterable;
    [SerializeField] private float bossCounterCheckRadius = 3.5f;

    protected override void Awake() {
        base.Awake();

        phase = GetComponentInChildren<SkillPhase>();
    }

    public void SetCanExecute(bool canExecute) {
        this.canExecute = canExecute;
    }

    public override void PerformAttackOnTarget(Transform target) {
        PerformAttackOnTarget(target, 1f);
    }

    public override void PerformAttackOnTarget(Transform target, float damageMultiplier) {
        base.PerformAttackOnTarget(target, damageMultiplier);

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

        if (TryCounterBossAtExtendedRange()) {
            hasPerformedCounter = true;
        }

        return hasPerformedCounter;
    }

    private bool TryCounterBossAtExtendedRange() {
        float facingDirection = Mathf.Sign(
            TargetCheck.position.x - transform.position.x
        );

        foreach (Collider2D target in GetDetectedColliders(
            whatIsCounterable,
            bossCounterCheckRadius
        )) {
            EnemyBoss boss = target.GetComponentInParent<EnemyBoss>();

            if (boss == null) {
                continue;
            }

            float directionToBoss = boss.transform.position.x - transform.position.x;

            if (directionToBoss * facingDirection < 0 || !boss.CanBeCountered()) {
                continue;
            }

            boss.HandleCounter();
            return true;
        }

        return false;
    }

    public float GetCounterRecoveryDuration() {
        return counterRecoveryDuration;
    }
}
