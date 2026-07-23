using UnityEngine;

public class EnemyBossDashState : EnemyState {
    private const float HarvestDashDamageMultiplier = 20f / 18f;

    private readonly EnemyBoss enemyBoss;
    private Transform player;
    private int dashDir;
    private float windupTimer;
    private float visualLeadTimer;
    private float damageDelayTimer;
    private float dashTimer;
    private bool dashVisualStarted;
    private bool dashMovementStarted;
    private bool playerHit;
    private Transform pendingHitTarget;

    public EnemyBossDashState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        enemyBoss.ExitAttackWindow();
        enemyBoss.vfx.SetAttackAlert(false);
        enemyBoss.vfx.PlayPreAttackVFX();

        int directionToPlayer = DirectionToPlayer();
        enemyBoss.HandleFlip(directionToPlayer);
        dashDir = directionToPlayer == 0
            ? enemyBoss.facingDir
            : directionToPlayer;

        windupTimer = Mathf.Max(0.65f, enemyBoss.harvestDashWindup);
        visualLeadTimer = 0;
        damageDelayTimer = 0;
        dashTimer = 0;
        dashVisualStarted = false;
        dashMovementStarted = false;
        playerHit = false;
        pendingHitTarget = null;
    }

    public override void Update() {
        base.Update();

        float deltaTime = Time.deltaTime * tc.localTimeScale;

        if (!dashVisualStarted) {
            windupTimer -= deltaTime;

            if (windupTimer > 0) {
                enemyBoss.MoveRaw(0, rb.linearVelocity.y);
                return;
            }

            dashVisualStarted = true;
            visualLeadTimer = Mathf.Max(
                0.1f,
                enemyBoss.harvestDashVisualLeadTime
            );
            anim.SetBool("Battle", false);
            anim.SetBool("Attack", true);
            anim.Play("BossAttack", 0, 0f);
            enemyBoss.vfx.DoImageEchoEffect(float.PositiveInfinity);
        }

        if (!dashMovementStarted) {
            visualLeadTimer -= deltaTime;
            enemyBoss.MoveRaw(0, rb.linearVelocity.y);

            if (visualLeadTimer > 0) {
                return;
            }

            dashMovementStarted = true;
        }

        dashTimer += deltaTime;
        enemyBoss.MoveRaw(
            dashDir * enemyBoss.dashSpeed,
            rb.linearVelocity.y
        );
        UpdatePlayerHit(deltaTime);

        if (enemyBoss.OutOfMoveRange()
            || dashTimer >= enemyBoss.harvestDashDuration) {
            if (pendingHitTarget != null && !playerHit) {
                enemyBoss.MoveRaw(0, rb.linearVelocity.y);
                return;
            }

            enemyBoss.CompleteHarvestDashAction();
            stateMachine.ChangeState(enemyBoss.battleState);
        }
    }

    public override void Exit() {
        base.Exit();

        enemyBoss.ExitAttackWindow();
        enemyBoss.vfx.SetAttackAlert(false);
        enemyBoss.vfx.StopImageEchoEffect();
        anim.SetBool("Attack", false);
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
    }

    private void UpdatePlayerHit(float deltaTime) {
        if (playerHit) {
            return;
        }

        if (pendingHitTarget != null) {
            damageDelayTimer -= deltaTime;

            if (damageDelayTimer > 0) {
                return;
            }

            playerHit = true;
            enemyBoss.combat.PerformAttackOnTarget(
                pendingHitTarget,
                HarvestDashDamageMultiplier
            );
            return;
        }

        foreach (Collider2D target in enemyBoss.combat.GetDetectedColliders(
            enemyBoss.combat.whatIsTarget
        )) {
            Player detectedPlayer = target.GetComponentInParent<Player>();

            if (detectedPlayer == null) {
                continue;
            }

            pendingHitTarget = detectedPlayer.transform;
            damageDelayTimer = Mathf.Max(
                0.05f,
                enemyBoss.harvestDashDamageDelay
            );
            anim.Play("BossAttack", 0, 0f);
            return;
        }
    }

    private int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemyBoss.transform.position.x ? 1 : -1;
    }
}

public class EnemyBossBidirectionalHarvestState : EnemyState {
    private const float FirstDashDamageMultiplier = 16f / 18f;
    private const float SecondDashDamageMultiplier = 20f / 18f;

    private enum HarvestStage {
        FirstWindup,
        FirstVisualLead,
        FirstDash,
        TurnPause,
        SecondVisualLead,
        SecondDash,
        Cancelled
    }

    private readonly EnemyBoss enemyBoss;
    private Transform player;
    private Transform pendingHitTarget;
    private HarvestStage stage;
    private int dashDirection;
    private float stageTimer;
    private float dashTimer;
    private float damageDelayTimer;
    private bool segmentHit;
    private bool counterCancelled;

    public bool IsSecondDashCounterable =>
        !counterCancelled
        && (stage == HarvestStage.SecondVisualLead
            || stage == HarvestStage.SecondDash);

    public EnemyBossBidirectionalHarvestState(
        EnemyBoss enemyBoss,
        StateMachine stateMachine,
        string animBoolName
    ) : base(enemyBoss, stateMachine, animBoolName) {
        this.enemyBoss = enemyBoss;
    }

    public override void Enter() {
        base.Enter();

        player = enemyBoss.GetPlayerReference();
        int directionToPlayer = DirectionToPlayer();
        enemyBoss.HandleFlip(directionToPlayer);
        dashDirection = directionToPlayer == 0
            ? enemyBoss.facingDir
            : directionToPlayer;

        stage = HarvestStage.FirstWindup;
        stageTimer = Mathf.Max(
            0.65f,
            enemyBoss.harvestDashWindup
        );
        dashTimer = 0;
        damageDelayTimer = 0;
        segmentHit = false;
        pendingHitTarget = null;
        counterCancelled = false;

        CloseCounterWindow();
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        enemyBoss.vfx.PlayPreAttackVFX();
    }

    public override void Update() {
        base.Update();

        float deltaTime = Time.deltaTime * tc.localTimeScale;

        switch (stage) {
            case HarvestStage.FirstWindup:
                UpdateFirstWindup(deltaTime);
                break;
            case HarvestStage.FirstVisualLead:
                UpdateVisualLead(
                    deltaTime,
                    HarvestStage.FirstDash
                );
                break;
            case HarvestStage.FirstDash:
                UpdateDash(
                    deltaTime,
                    FirstDashDamageMultiplier,
                    false
                );
                break;
            case HarvestStage.TurnPause:
                UpdateTurnPause(deltaTime);
                break;
            case HarvestStage.SecondVisualLead:
                UpdateVisualLead(
                    deltaTime,
                    HarvestStage.SecondDash
                );
                break;
            case HarvestStage.SecondDash:
                UpdateDash(
                    deltaTime,
                    SecondDashDamageMultiplier,
                    true
                );
                break;
            case HarvestStage.Cancelled:
                enemyBoss.MoveRaw(0, rb.linearVelocity.y);
                break;
        }
    }

    public override void Exit() {
        CloseCounterWindow();
        enemyBoss.vfx.StopImageEchoEffect();
        anim.SetBool("Attack", false);
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        base.Exit();
    }

    public void CancelForCounter() {
        counterCancelled = true;
        stage = HarvestStage.Cancelled;
        CloseCounterWindow();
        enemyBoss.vfx.StopImageEchoEffect();
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
    }

    private void UpdateFirstWindup(float deltaTime) {
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        stageTimer -= deltaTime;

        if (stageTimer > 0) {
            return;
        }

        BeginDashVisual(false);
        stage = HarvestStage.FirstVisualLead;
    }

    private void UpdateVisualLead(
        float deltaTime,
        HarvestStage nextStage
    ) {
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        stageTimer -= deltaTime;

        if (stageTimer > 0) {
            return;
        }

        stage = nextStage;
        dashTimer = 0;
    }

    private void UpdateDash(
        float deltaTime,
        float damageMultiplier,
        bool isSecondDash
    ) {
        dashTimer += deltaTime;
        enemyBoss.MoveRaw(
            dashDirection * enemyBoss.dashSpeed,
            rb.linearVelocity.y
        );
        UpdateSegmentHit(deltaTime, damageMultiplier);

        bool dashFinished =
            enemyBoss.OutOfMoveRange()
            || dashTimer >= enemyBoss.harvestDashDuration;

        if (!dashFinished) {
            return;
        }

        if (pendingHitTarget != null && !segmentHit) {
            enemyBoss.MoveRaw(0, rb.linearVelocity.y);
            return;
        }

        if (isSecondDash) {
            FinishHarvest();
            return;
        }

        BeginTurnPause();
    }

    private void UpdateSegmentHit(
        float deltaTime,
        float damageMultiplier
    ) {
        if (segmentHit) {
            return;
        }

        if (pendingHitTarget != null) {
            damageDelayTimer -= deltaTime;

            if (damageDelayTimer > 0) {
                return;
            }

            segmentHit = true;
            enemyBoss.combat.PerformAttackOnTarget(
                pendingHitTarget,
                damageMultiplier
            );
            return;
        }

        foreach (Collider2D target in enemyBoss.combat.GetDetectedColliders(
            enemyBoss.combat.whatIsTarget
        )) {
            Player detectedPlayer =
                target.GetComponentInParent<Player>();

            if (detectedPlayer == null) {
                continue;
            }

            pendingHitTarget = detectedPlayer.transform;
            damageDelayTimer = Mathf.Max(
                0.05f,
                enemyBoss.harvestDashDamageDelay
            );
            anim.Play("BossAttack", 0, 0f);
            return;
        }
    }

    private void BeginTurnPause() {
        CloseCounterWindow();
        enemyBoss.vfx.StopImageEchoEffect();
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        anim.SetBool("Attack", false);
        anim.SetBool("Battle", true);

        stage = HarvestStage.TurnPause;
        stageTimer = Mathf.Max(
            0.1f,
            enemyBoss.bidirectionalHarvestPauseDuration
        );
    }

    private void UpdateTurnPause(float deltaTime) {
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        stageTimer -= deltaTime;

        if (stageTimer > 0) {
            return;
        }

        dashDirection = -dashDirection;
        enemyBoss.HandleFlip(dashDirection);
        ResetSegmentHit();
        BeginDashVisual(true);
        stage = HarvestStage.SecondVisualLead;
    }

    private void BeginDashVisual(bool counterable) {
        stageTimer = Mathf.Max(
            0.1f,
            enemyBoss.harvestDashVisualLeadTime
        );
        anim.SetBool("Battle", false);
        anim.SetBool("Attack", true);
        anim.Play("BossAttack", 0, 0f);
        enemyBoss.vfx.DoImageEchoEffect(float.PositiveInfinity);

        if (counterable) {
            enemyBoss.EnterAttackWindow();
            enemyBoss.vfx.SetAttackAlert(true);
        } else {
            CloseCounterWindow();
        }
    }

    private void FinishHarvest() {
        CloseCounterWindow();
        enemyBoss.vfx.StopImageEchoEffect();
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        enemyBoss.CompleteBidirectionalHarvestAction();
        stateMachine.ChangeState(enemyBoss.battleState);
    }

    private void ResetSegmentHit() {
        segmentHit = false;
        pendingHitTarget = null;
        damageDelayTimer = 0;
        dashTimer = 0;
    }

    private void CloseCounterWindow() {
        enemyBoss.ExitAttackWindow();
        enemyBoss.vfx.SetAttackAlert(false);
    }

    private int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemyBoss.transform.position.x
            ? 1
            : -1;
    }
}
