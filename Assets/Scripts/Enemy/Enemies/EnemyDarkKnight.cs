using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class EnemyDarkKnight : Enemy, IEnemyPhaseTransition {
    public EnemyPhaseTransitionState phaseTransitionState;
    public EnemyDarkKnightSwordwaveState swordwaveState;
    public EnemyDarkKnightDashAttackState dashAttackState;

    [Header("Dark knight specific info")]
    public int numOfAttack = 3;
    public float phaseTwoHPThreshold = 0.5f;
    public float phaseTransitionDuration = 2;

    public bool isPhaseTwo { get; private set; } = false;
    public int phaseIndex { get; private set; } = 0;
    private bool phaseTransitionTriggered = false;

    [Header("Combo attack info")]
    public float[] comboAttackWindup = {0.5f, 0.3f};
    public float[] comboAttackCooldown = {1, 0.6f};
    public int[] comboLimit = {2, 3};

    private Coroutine enterAttackStateWithDelayCo;

    [Header("Sword wave info")]
    [SerializeField] private GameObject swordWavePrefab;
    [SerializeField] private Transform swordWaveStartPoint;
    [SerializeField] private float swordWaveSpeed = 8;
    [SerializeField] private float[] numOfSwordwave = {1, 2};
    [SerializeField] private float swordwaveInterval = 0.3f;
    public float[] swordwaveWindup = {0.8f, 0.5f};
    public float[] swordwaveCooldown = {2, 1.5f};
    public float swordwaveDistance = 12;

    private Coroutine swordwaveCo;

    [Header("Dash attack info")]
    public float dashAttackMaxDistance = 15;
    public float dashSpeed = 15;
    public float dashAttackWindup = 0.5f;
    public float dashAttackCooldown = 3;
    public float dashDistance = 12;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyEliteIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyDarkKnightBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyDarkKnightAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        phaseTransitionState = new EnemyPhaseTransitionState(this, stateMachine, "Battle");
        swordwaveState = new EnemyDarkKnightSwordwaveState(this, stateMachine, "Swordwave");
        dashAttackState = new EnemyDarkKnightDashAttackState(this, stateMachine, "DashAttack");

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
        vfx.DoFadeEffect();
        gameObject.layer = LayerMask.NameToLayer("Untargetable");
    }

    public void CompletePhaseTransition() {
        isPhaseTwo = true;
        phaseIndex = 1;
    }

    public void EndPhaseTransition() {
        vfx.ResetTransparency();
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

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
        StopSwordwave();
        StopComboAttack();

        vfx.SetAttackAlert(false);
        ExitAttackWindow();
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        Swordwave();
    }

    private void Swordwave() {
        if (swordwaveCo != null) {
            StopCoroutine(swordwaveCo);
        }

        swordwaveCo = StartCoroutine(SwordwaveCo());
    }

    private void StopSwordwave() {
        if (swordwaveCo != null) {
            StopCoroutine(swordwaveCo);
            swordwaveCo = null;
        }
    }

    private IEnumerator SwordwaveCo() {
        for (int i = 0; i < numOfSwordwave[phaseIndex]; i += 1) {
            GameObject swordWave = 
                Instantiate(swordWavePrefab, swordWaveStartPoint.position, Quaternion.identity);
            swordWave.GetComponent<EnemyDarkKnightSwordWave>().SetUpSwordWave(facingDir * swordWaveSpeed, combat);
            yield return new WaitForSeconds(swordwaveInterval);
        }
    }

    public void EnterAttackStateWithDelay() {
        if (enterAttackStateWithDelayCo != null) {
            StopCoroutine(enterAttackStateWithDelayCo);
        }

        enterAttackStateWithDelayCo = StartCoroutine(EnterAtatckStateWithDelayCo());
    }

    private void StopComboAttack() {
        if (enterAttackStateWithDelayCo != null) {
            StopCoroutine(enterAttackStateWithDelayCo);
            enterAttackStateWithDelayCo = null;
        }
    }

    private IEnumerator EnterAtatckStateWithDelayCo() {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(attackState);
    }
}
