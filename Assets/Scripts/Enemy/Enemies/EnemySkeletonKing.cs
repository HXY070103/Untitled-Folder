using System;
using System.Collections;
using UnityEngine;

public class EnemySkeletonKing : Enemy, IEnemyPhaseTransition {
    public EnemyPhaseTransitionState phaseTransitionState;
    public EnemySkeletonKingShockwaveState shockwaveState;

    [Header("Skeleton king specific info")]
    public int numOfAttack = 3;
    public float phaseTwoHPThreshold = 0.5f;
    public float phaseTransitionDuration = 2;
    [SerializeField] private float phaseTwoBonusDamageMultiplier = 2;
    [SerializeField] private float phaseTwoBonusArmorMultiplier = 2;

    public bool isPhaseTwo { get; private set; } = false;
    public int phaseIndex { get; private set; } = 0;
    private bool phaseTransitionTriggered = false;

    [Header("Basic attack info")]
    public float[] basicAttackWindup = {0.5f, 0.3f};
    public float[] basicAttackCooldown = {1, 0.6f};

    [Header("Shockwave info")]
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private Transform shockwaveStartPoint;
    [SerializeField] private float shockwaveSpeed = 10;
    public float[] shockwaveWindup = {0.8f, 0.5f};
    public float[] shockwaveCooldown = {2, 1.2f};
    public float shockwaveDistance = 12;

    [Header("Summon info")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private Transform summonStartPoint;
    [SerializeField] private int amountOfSkeletonToSummon = 3;
    [SerializeField] private float summonInterval = 0.5f;

    private Coroutine summonCo;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyEliteIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemySkeletonKingBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        phaseTransitionState = new EnemyPhaseTransitionState(this, stateMachine, "Battle");
        shockwaveState = new EnemySkeletonKingShockwaveState(this, stateMachine, "Shockwave");

        anim.SetBool("HasStunnedRecovery", false);
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
        health.OnHealthUpdate += TryStartPhaseTransition;
        TryStartPhaseTransition();
    }

    private void OnDestroy() {
        health.OnHealthUpdate -= TryStartPhaseTransition;
    }

    public float GetPhaseTransitionDuration() {
        return phaseTransitionDuration;
    }

    public EntityState GetPhaseTransitionNextState() {
        return battleState;
    }
        
    public void BeginPhaseTransition() {
        MoveRaw(0, 0);
        StartSummon();
    }

    public void CompletePhaseTransition() {
        isPhaseTwo = true;
        phaseIndex = 1;
        stats.damage.AddMultiplier(phaseTwoBonusDamageMultiplier, "PhaseTwoBuff");
        stats.armor.AddMultiplier(phaseTwoBonusArmorMultiplier, "PhaseTwoBuff");
    }

    public void EndPhaseTransition() {}

    private void TryStartPhaseTransition() {
        if (health.GetCurrentHP() <= 0 ||  stateMachine.currentState == deadState) {
            return;
        }

        if (phaseTransitionTriggered || !IsLowHP()) {
            return;
        }

        InterruptAllAttacks();

        phaseTransitionTriggered = true;
        stateMachine.ChangeState(phaseTransitionState);
    }

    private bool IsLowHP() {
        return health.GetHPPercentage() < phaseTwoHPThreshold;
    }

    private void InterruptAllAttacks() {
        StopSummon();

        vfx.SetAttackAlert(false);
        ExitAttackWindow();
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        Shockwave();
    }

    private void Shockwave() {
        GameObject shockwave = Instantiate(shockwavePrefab, shockwaveStartPoint.position, Quaternion.identity);
        shockwave.GetComponent<EnemySkeletonKingProjectile>().SetUpShockwave(facingDir * shockwaveSpeed, combat);

        if (isPhaseTwo) {
            GameObject bonusShockwave = Instantiate(shockwavePrefab, shockwaveStartPoint.position, Quaternion.identity);
            bonusShockwave.GetComponent<EnemySkeletonKingProjectile>().SetUpShockwave(-facingDir * shockwaveSpeed, combat);
        }
    }

    private void StartSummon() {
        if (summonCo != null) {
            StopCoroutine(summonCo);
        }

        summonCo = StartCoroutine(SummonCo());
    }

    private void StopSummon() {
        if (summonCo != null) {
            StopCoroutine(summonCo);
            summonCo = null;
        }
    }

    private IEnumerator SummonCo() {
        for (int i = 0; i < amountOfSkeletonToSummon; i += 1) {
            GameObject skeleton = 
                Instantiate(skeletonPrefab, summonStartPoint.position, Quaternion.identity);
            skeleton.GetComponent<EnemySkeleton>().HandleFlip(facingDir);
            skeleton.GetComponent<EnemySkeleton>().TryToEnterBattleState(player);

            Color color = Color.purple;
            vfx.GenerateParticleEffect(color);

            yield return new WaitForSeconds(summonInterval);
        }
    }
}