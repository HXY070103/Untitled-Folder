using System.Collections.Generic;
using UnityEngine;

public class EnemyStoneGuard : Enemy, IEnemyPhaseTransition {
    public const string CoreDamageSource = "MechaGolemCore";
    public const string PhaseTransitionImmunitySource = "MechaGolemPhaseTransition";

    public EnemyStoneGuardShootState shootState { get; private set; }
    public EnemyStoneGuardLaserState laserState { get; private set; }
    public EnemyStoneGuardComboState comboState { get; private set; }
    public EnemyStoneGuardShieldState shieldState { get; private set; }
    public EnemyPhaseTransitionState phaseTransitionState { get; private set; }
    public EnemyStoneGuardCoreExposedState coreExposedState { get; private set; }
    public EnemyStoneGuardCoreRecoveryState coreRecoveryState { get; private set; }

    public bool isPhaseTwo { get; private set; }

    private readonly HashSet<EnemyStoneGuardArmProjectile> activeArmProjectiles = new HashSet<EnemyStoneGuardArmProjectile>();
    private readonly HashSet<EnemyStoneGuardLaser> activeLasers = new HashSet<EnemyStoneGuardLaser>();
    private bool useArmProjectileNext = true;

    [Header("Stone Guard melee info")]
    public float meleeDuration = 0.78f;
    public float meleeCounterWindowStart = 0.52f;
    public float meleeHitTime = 0.7f;

    [Header("Stone Guard arm projectile info")]
    [SerializeField] private EnemyStoneGuardArmProjectile armProjectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    public float shootDuration = 0.75f;
    public float projectileReleaseTime = 0.7f;

    [Header("Stone Guard laser info")]
    [SerializeField] private EnemyStoneGuardLaser laserPrefab;
    [SerializeField] private Transform laserSpawnPoint;
    public float laserAimDuration = 0.8f;
    public float laserBeamDuration = 0.4f;

    [Header("Stone Guard combo info")]
    public float comboLaserStartDelay = 0.8f;
    public float comboCooldown = 6;

    private float lastComboTime = float.NegativeInfinity;

    [Header("Stone Guard shield info")]
    [SerializeField] private Transform shieldCheck;
    public float shieldTriggerDistance = 2;
    public float shieldStayDuration = 1.5f;
    public float shieldCooldown = 7;
    public float shieldWindup = 0.45f;
    public float shieldDuration = 0.5f;
    public Vector2 shieldPushVelocity = new Vector2(8, 2);
    public float shieldPushDuration = 0.5f;

    private float shieldProximityTimer;
    private float lastShieldTime = float.NegativeInfinity;

    [Header("Stone Guard phase info")]
    [Range(0, 1)] public float phaseTwoHPThreshold = 0.55f;
    public float phaseTransitionDuration = 0.8f;

    private bool phaseTransitionTriggered;

    [Header("Stone Guard core info")]
    public float coreExposedDuration = 1.8f;
    public float coreRecoveryDuration = 0.35f;
    public float coreDamageTakenMultiplier = 1.6f;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyStoneGuardIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Idle");
        battleState = new EnemyStoneGuardBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyStoneGuardAttackState(this, stateMachine, "Attack");
        shootState = new EnemyStoneGuardShootState(this, stateMachine, "Shoot");
        laserState = new EnemyStoneGuardLaserState(this, stateMachine, "Laser");
        comboState = new EnemyStoneGuardComboState(this, stateMachine, "Shoot");
        shieldState = new EnemyStoneGuardShieldState(this, stateMachine, "ShieldCast");
        phaseTransitionState = new EnemyPhaseTransitionState(this, stateMachine, "PhaseTransition");
        coreExposedState = new EnemyStoneGuardCoreExposedState(this, stateMachine, "CoreExposed");
        coreRecoveryState = new EnemyStoneGuardCoreRecoveryState(this, stateMachine, "CoreRecovery");
        deadState = new EnemyStoneGuardDeadState(this, stateMachine, "Dead");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Battle");
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
        health.OnHealthUpdate += TryStartPhaseTransition;
        TryStartPhaseTransition();
    }

    protected override void Update() {
        base.Update();
        UpdateShieldProximityTimer();
    }

    protected override Transform DetectPlayer() {
        Collider2D detectedCollider = Physics2D.OverlapCircle(playerCheck.position, playerCheckDistance, whatIsPlayer);

        if (detectedCollider == null) {
            return null;
        }

        Player detectedPlayer = detectedCollider.GetComponentInParent<Player>();

        return detectedPlayer != null ? detectedPlayer.transform : null;
    }

    public override void HandleCounter() {
        if (!CanBeCountered()) {
            return;
        }

        ExposeCore();
    }

    public void ExposeCore() {
        if (stateMachine.currentState == coreExposedState || stateMachine.currentState == coreRecoveryState || stateMachine.currentState == deadState) {
            return;
        }

        stateMachine.ChangeState(coreExposedState);
    }

    public void ReleaseArmProjectile(int comboAttackId = 0) {
        if (armProjectilePrefab == null || projectileSpawnPoint == null) {
            Debug.LogWarning("Stone Guard arm projectile prefab or spawn point is missing.", this);
            return;
        }

        EnemyStoneGuardArmProjectile projectile = Instantiate(armProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        projectile.SetUp(this, facingDir, comboAttackId);
    }

    public void RegisterArmProjectile(EnemyStoneGuardArmProjectile projectile) {
        if (projectile != null) {
            activeArmProjectiles.Add(projectile);
        }
    }

    public void UnregisterArmProjectile(EnemyStoneGuardArmProjectile projectile) {
        activeArmProjectiles.Remove(projectile);
    }

    public void DestroyActiveArmProjectiles() {
        foreach (EnemyStoneGuardArmProjectile projectile in activeArmProjectiles) {
            if (projectile != null) {
                projectile.DestroyFromOwner();
            }
        }

        activeArmProjectiles.Clear();
    }

    public EntityState GetNextRangedAttackState() {
        if (isPhaseTwo) {
            if (tc.time >= lastComboTime + comboCooldown) {
                return comboState;
            }

            return shootState;
        }

        EntityState nextState = useArmProjectileNext ? shootState : laserState;
        useArmProjectileNext = !useArmProjectileNext;
        return nextState;
    }

    public void BeginComboAttack() {
        lastComboTime = tc.time;
    }

    public void HandleArmProjectileCounter(int comboAttackId) {
        if (stateMachine.currentState == comboState) {
            comboState.HandleArmProjectileCounter(comboAttackId);
            return;
        }

        ExposeCore();
    }

    public EnemyStoneGuardLaser CreateLaser() {
        if (laserPrefab == null || laserSpawnPoint == null) {
            Debug.LogWarning("Stone Guard laser prefab or spawn point is missing.", this);
            return null;
        }

        EnemyStoneGuardLaser laser = Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.identity);
        laser.SetUp(this, facingDir);
        return laser;
    }

    public void RegisterLaser(EnemyStoneGuardLaser laser) {
        if (laser != null) {
            activeLasers.Add(laser);
        }
    }

    public void UnregisterLaser(EnemyStoneGuardLaser laser) {
        activeLasers.Remove(laser);
    }

    public void DestroyActiveLasers() {
        foreach (EnemyStoneGuardLaser laser in activeLasers) {
            if (laser != null) {
                laser.DestroyFromOwner();
            }
        }

        activeLasers.Clear();
    }

    public void DestroyActiveAttacks() {
        DestroyActiveArmProjectiles();
        DestroyActiveLasers();
    }

    public void BeginPhaseTransition() {
        MoveRaw(0, rb.linearVelocity.y);
        DestroyActiveAttacks();
        ExitAttackWindow();
        vfx.SetAttackAlert(false);
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

    public bool ShouldUseShield() {
        return PlayerWithinShieldRange() && shieldProximityTimer >= shieldStayDuration && tc.time >= lastShieldTime + shieldCooldown;
    }

    public void BeginShieldCast() {
        shieldProximityTimer = 0;
        lastShieldTime = tc.time;
    }

    public void PushPlayerAway() {
        Transform playerTransform = GetPlayerReference();

        if (playerTransform == null || !PlayerWithinShieldRange()) {
            return;
        }

        Player targetPlayer = playerTransform.GetComponent<Player>();

        if (targetPlayer == null) {
            targetPlayer = playerTransform.GetComponentInParent<Player>();
        }

        if (targetPlayer == null) {
            return;
        }

        int pushDirection = GetPushDirection(playerTransform);
        Vector2 pushVelocity = new Vector2(pushDirection * shieldPushVelocity.x, shieldPushVelocity.y);
        targetPlayer.ReceiveKnockback(pushVelocity, shieldPushDuration);
    }

    private void UpdateShieldProximityTimer() {
        if (!CanTrackShieldProximity() || !PlayerWithinShieldRange()) {
            shieldProximityTimer = 0;
            return;
        }

        shieldProximityTimer += Time.deltaTime * tc.localTimeScale;
    }

    private bool CanTrackShieldProximity() {
        return stateMachine.currentState != shieldState && stateMachine.currentState != phaseTransitionState && stateMachine.currentState != coreExposedState && stateMachine.currentState != coreRecoveryState && stateMachine.currentState != deadState;
    }

    private void TryStartPhaseTransition() {
        if (phaseTransitionTriggered || health.GetCurrentHP() <= 0 || health.GetHPPercentage() > phaseTwoHPThreshold || stateMachine.currentState == deadState) {
            return;
        }

        phaseTransitionTriggered = true;
        stateMachine.ChangeState(phaseTransitionState);
    }

    private bool PlayerWithinShieldRange() {
        Transform playerTransform = GetPlayerReference();

        if (shieldCheck == null || playerTransform == null) {
            return false;
        }

        return Vector2.Distance(shieldCheck.position, playerTransform.position) <= shieldTriggerDistance;
    }

    private int GetPushDirection(Transform playerTransform) {
        if (playerTransform.position.x > transform.position.x) {
            return 1;
        }

        if (playerTransform.position.x < transform.position.x) {
            return -1;
        }

        return facingDir;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

        if (shieldCheck == null) {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(shieldCheck.position, shieldTriggerDistance);
    }
}
