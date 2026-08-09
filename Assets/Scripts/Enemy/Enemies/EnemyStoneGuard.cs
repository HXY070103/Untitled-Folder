using UnityEngine;

public class EnemyStoneGuard : Enemy, IEnemyPhaseTransition {
    public const string CoreDamageSource = "StoneGuardCoreExpose";
    public const string PhaseTransitionImmunitySource = "StoneGuardPhaseTransition";

    public EnemyStoneGuardShootState shootState { get; private set; }
    public EnemyStoneGuardLaserState laserState { get; private set; }
    public EnemyStoneGuardShieldState shieldState { get; private set; }
    public EnemyPhaseTransitionState phaseTransitionState { get; private set; }

    [Header("Stone guard specific info")]
    public float rangeAttackDistance = 5;
    public int numOfRangeAttack = 2;
    public float phaseTwoHPThreshold = 0.55f;
    public float phaseTransitionDuration = 0.8f;

    public bool isPhaseTwo { get; private set; }
    private bool phaseTransitionTriggered;

    [Header("Shoot info")]
    [SerializeField] private GameObject armProjectilePrefab;
    [SerializeField] private Transform armProjectileStartPoint;
    [SerializeField] private float armProjectileSpeed = 10;
    public float shootWindup = 0.5f;
    public float shootCooldown = 1.5f;
    public float shootDistance = 12;

    [Header("Laser info")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform laserStartPoint;
    public float laserDuration = 1;
    public float laserWindup = 0.5f;
    public float laserCooldown = 1.5f;
    public float laserDistance = 12;

    private EnemyStoneGuardLaser laser;

    [Header("Combo info")]
    public float comboLaserStartDelay = 0.8f;
    public float comboCooldown = 6;


    [Header("Shield info")]
    [SerializeField] private Transform shieldCheck;
    [SerializeField] private float shieldStayDuration = 0.5f;
    public float shieldDistance = 2;
    public Vector2 shieldPushVelocity = new Vector2(14, 2);
    public float shieldPushDuration = 0.5f;
    public float shieldCooldown = 7;

    private float shieldStayTimer = 0;

    [Header("Core expose info")]
    public float coreDamageTakenMultiplier = 1.6f;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyEliteIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyStoneGuardBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Dead");
        stunnedState = new EnemyStoneGuardStunnedState(this, stateMachine, "Stunned");
        phaseTransitionState = new EnemyPhaseTransitionState(this, stateMachine, "PhaseTransition");
        shootState = new EnemyStoneGuardShootState(this, stateMachine, "Shoot");
        laserState = new EnemyStoneGuardLaserState(this, stateMachine, "LaserCast");
        shieldState = new EnemyStoneGuardShieldState(this, stateMachine, "ShieldCast");

        anim.SetBool("HasStunnedRecovery", true);
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
        health.OnHealthUpdate += TryStartPhaseTransition;
        TryStartPhaseTransition();
    }

    protected override void Update() {
        base.Update();

        UpdateShieldStayTimer();
    }

    private void OnDestroy() {
        health.OnHealthUpdate -= TryStartPhaseTransition;
    }

    public void BeginPhaseTransition() {
        health.AddDamageImmunity(PhaseTransitionImmunitySource);
    }

    public float GetPhaseTransitionDuration() {
        return phaseTransitionDuration;
    }

    public EntityState GetPhaseTransitionNextState() {
        return battleState;
    }

    public void CompletePhaseTransition() {
        isPhaseTwo = true;
    }

    public void EndPhaseTransition() {
        health.RemoveDamageImmunity(PhaseTransitionImmunitySource);
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
        anim.SetBool("LaserCastPerformed", true);
        vfx.SetAttackAlert(false);
        ExitAttackWindow();
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        if (stateMachine.currentState == shootState) {
            Shoot();
        }

        if (stateMachine.currentState == laserState) {
            LaserFire();
        }
    }

    public void Shoot() {
        GameObject projectile = Instantiate(armProjectilePrefab, armProjectileStartPoint.position, Quaternion.identity);
        projectile.GetComponent<EnemyStoneGuardArmProjectile>().SetUpArmProjectile(facingDir * armProjectileSpeed, combat);
    }

    public void CreateLaser() {
        GameObject laserObject = Instantiate(laserPrefab, laserStartPoint.position, Quaternion.identity);
        laser = laserObject.GetComponent<EnemyStoneGuardLaser>();
        laser.SetUpLaser(combat, facingDir);
    }

    private void LaserFire() {
        if (laser == null) {
            return;
        }

        laser.StartLaser();
    }

    public void DestroyLaser() {
        if (laser == null) {
            return;
        }

        laser.EndLaser();
        laser = null;
    }

    private void UpdateShieldStayTimer() {
        if (!PlayerWithinShieldRange()) {
            shieldStayTimer = 0;
            return;
        }

        shieldStayTimer += Time.deltaTime * tc.localTimeScale;
    }

    private bool PlayerWithinShieldRange() {
        return Vector2.Distance(shieldCheck.position, player.position) < shieldDistance;
    }

    public bool ReachedShieldStayDuration() {
        return shieldStayTimer > shieldStayDuration;
    }

    protected override Transform DetectPlayer() {
        Collider2D player = 
            Physics2D.OverlapCircle(playerCheck.position, playerCheckDistance, whatIsPlayer);

        if (player == null) {
            return default;
        }

        return player.transform;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCheck.position, playerCheckDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(shieldCheck.position, shieldDistance);
    }
}
