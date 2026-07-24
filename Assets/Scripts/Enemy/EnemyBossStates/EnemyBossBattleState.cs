using UnityEngine;

public static class EnemyBossGroundFissurePatterns {
    public const int RegionCount = 5;

    public static readonly int[][] All = {
        new[] { 0, 3 },
        new[] { 0, 4 },
        new[] { 1, 4 },
        new[] { 0, 2 },
        new[] { 2, 4 },
        new[] { 0, 1, 4 },
        new[] { 0, 3, 4 },
        new[] { 0, 1, 2 },
        new[] { 2, 3, 4 }
    };
}

public class EnemyBossBattleState : EnemyBattleState {
    private readonly EnemyBoss enemyBoss;
    private EnemyBossPhaseOneAction phaseOneAction =
        EnemyBossPhaseOneAction.Combo;
    private float phaseOneRecoveryTimer;
    private EnemyBossPhaseTwoAction phaseTwoAction =
        EnemyBossPhaseTwoAction.Combo;
    private float phaseTwoRecoveryTimer;
    private EnemyBossPhaseThreeAction phaseThreeAction =
        EnemyBossPhaseThreeAction.Combo;
    private float phaseThreeRecoveryTimer;
    private EnemyBossPhase configuredPhase = EnemyBossPhase.PhaseOne;

    public EnemyBossBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.HandleFlip(DirectionToPlayer());
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        ConfigureCurrentPhase();

        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseOne) {
            HandlePhaseOneBattle();
            return;
        }

        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseTwo) {
            HandlePhaseTwoBattle();
            return;
        }

        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseThree) {
            HandlePhaseThreeBattle();
            return;
        }

        if (WithInAttackRange() && enemy.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            enemyBoss.SetAttack(
                enemyBoss.attackWindup,
                enemyBoss.attackCooldown,
                enemyBoss.attackState
            );
            Debug.Log($"Boss basic attack started at distance {DistanceToPlayer():0.00}");
            stateMachine.ChangeState(enemyBoss.preAttackState);
        } else {
            if (!enemyBoss.PlayerDetected()) {
                enemyBoss.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemyBoss.OutOfMoveRange()) {
                return;
            }

            if (WithInDashRange()) {
                stateMachine.ChangeState(enemyBoss.dashState);
                return;
            }

            float xVelocity = DirectionToPlayer() * enemyBoss.battleMoveSpeed;
            enemyBoss.Move(xVelocity, rb.linearVelocity.y);
            enemyBoss.sfx?.TryPlayMove(0.2f);
        }
    }

    public void CompletePhaseOneAction(
        EnemyBossPhaseOneAction completedAction
    ) {
        if (phaseOneAction != completedAction) {
            return;
        }

        switch (completedAction) {
            case EnemyBossPhaseOneAction.Combo:
                phaseOneAction = EnemyBossPhaseOneAction.EnergyWave;
                break;
            case EnemyBossPhaseOneAction.EnergyWave:
                phaseOneAction = EnemyBossPhaseOneAction.HarvestDash;
                break;
            case EnemyBossPhaseOneAction.HarvestDash:
                phaseOneAction = EnemyBossPhaseOneAction.Recovery;
                phaseOneRecoveryTimer =
                    enemyBoss.phaseOneRecoveryDuration;
                break;
        }
    }

    public void CompleteComboAction() {
        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseOne) {
            CompletePhaseOneAction(EnemyBossPhaseOneAction.Combo);
            return;
        }

        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseTwo) {
            CompletePhaseTwoAction(EnemyBossPhaseTwoAction.Combo);
            return;
        }

        CompletePhaseThreeAction(EnemyBossPhaseThreeAction.Combo);
    }

    public void CompleteEnergyWaveAction() {
        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseOne) {
            CompletePhaseOneAction(EnemyBossPhaseOneAction.EnergyWave);
            return;
        }

        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseTwo) {
            CompletePhaseTwoAction(EnemyBossPhaseTwoAction.EnergyWave);
            return;
        }

        CompletePhaseThreeAction(
            EnemyBossPhaseThreeAction.EnergyWave
        );
    }

    public void CompleteHarvestDashAction() {
        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseOne) {
            CompletePhaseOneAction(EnemyBossPhaseOneAction.HarvestDash);
            return;
        }

        CompletePhaseTwoAction(EnemyBossPhaseTwoAction.HarvestDash);
    }

    public void CompleteGroundFissureAction() {
        if (enemyBoss.CurrentPhase == EnemyBossPhase.PhaseTwo) {
            CompletePhaseTwoAction(
                EnemyBossPhaseTwoAction.GroundFissure
            );
            return;
        }

        CompletePhaseThreeAction(
            EnemyBossPhaseThreeAction.DoubleGroundFissure
        );
    }

    public void CompleteBidirectionalHarvestAction() {
        CompletePhaseThreeAction(
            EnemyBossPhaseThreeAction.BidirectionalHarvest
        );
    }

    private void CompletePhaseTwoAction(
        EnemyBossPhaseTwoAction completedAction
    ) {
        if (enemyBoss.CurrentPhase != EnemyBossPhase.PhaseTwo
            || phaseTwoAction != completedAction) {
            return;
        }

        switch (completedAction) {
            case EnemyBossPhaseTwoAction.Combo:
                phaseTwoAction = EnemyBossPhaseTwoAction.EnergyWave;
                break;
            case EnemyBossPhaseTwoAction.EnergyWave:
                phaseTwoAction =
                    EnemyBossPhaseTwoAction.RecoveryAfterEnergyWave;
                phaseTwoRecoveryTimer =
                    enemyBoss.phaseTwoRecoveryDuration;
                break;
            case EnemyBossPhaseTwoAction.GroundFissure:
                phaseTwoAction = EnemyBossPhaseTwoAction.HarvestDash;
                break;
            case EnemyBossPhaseTwoAction.HarvestDash:
                phaseTwoAction =
                    EnemyBossPhaseTwoAction.RecoveryAfterHarvestDash;
                phaseTwoRecoveryTimer =
                    enemyBoss.phaseTwoRecoveryDuration;
                break;
        }
    }

    private void CompletePhaseThreeAction(
        EnemyBossPhaseThreeAction completedAction
    ) {
        if (enemyBoss.CurrentPhase != EnemyBossPhase.PhaseThree
            || phaseThreeAction != completedAction) {
            return;
        }

        switch (completedAction) {
            case EnemyBossPhaseThreeAction.Combo:
                phaseThreeAction =
                    EnemyBossPhaseThreeAction.EnergyWave;
                break;
            case EnemyBossPhaseThreeAction.EnergyWave:
                phaseThreeAction =
                    EnemyBossPhaseThreeAction
                        .RecoveryAfterEnergyWave;
                phaseThreeRecoveryTimer =
                    enemyBoss.phaseThreeRecoveryDuration;
                break;
            case EnemyBossPhaseThreeAction.BidirectionalHarvest:
                phaseThreeAction =
                    EnemyBossPhaseThreeAction
                        .DoubleGroundFissure;
                break;
            case EnemyBossPhaseThreeAction.DoubleGroundFissure:
                phaseThreeAction =
                    EnemyBossPhaseThreeAction
                        .RecoveryAfterGroundFissure;
                phaseThreeRecoveryTimer =
                    enemyBoss.phaseThreeRecoveryDuration;
                break;
        }
    }

    private void HandlePhaseOneBattle() {
        switch (phaseOneAction) {
            case EnemyBossPhaseOneAction.Combo:
                HandlePhaseOneCombo();
                break;
            case EnemyBossPhaseOneAction.EnergyWave:
                StartEnergyWave();
                break;
            case EnemyBossPhaseOneAction.HarvestDash:
                StartHarvestDash();
                break;
            case EnemyBossPhaseOneAction.Recovery:
                HandleRecovery();
                break;
        }
    }

    private void HandlePhaseOneCombo() {
        if (WithInAttackRange() && enemy.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            enemyBoss.SetAttack(
                enemyBoss.attackWindup,
                enemyBoss.attackCooldown,
                enemyBoss.attackState
            );
            Debug.Log("Boss phase one action started: Combo");
            stateMachine.ChangeState(enemyBoss.preAttackState);
            return;
        }

        ApproachPlayer();
    }

    private void StartEnergyWave() {
        if (!enemy.PlayerDetected()) {
            ApproachPlayer();
            return;
        }

        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        enemyBoss.HandleFlip(DirectionToPlayer());
        Debug.Log(
            $"Boss {enemyBoss.CurrentPhase} action started: " +
            "EnergyWave"
        );
        stateMachine.ChangeState(enemyBoss.spellCastState);
    }

    private void StartHarvestDash() {
        if (!enemy.PlayerDetected()) {
            ApproachPlayer();
            return;
        }

        enemyBoss.HandleFlip(DirectionToPlayer());
        Debug.Log(
            $"Boss {enemyBoss.CurrentPhase} action started: " +
            "HarvestDash"
        );
        stateMachine.ChangeState(enemyBoss.dashState);
    }

    private void HandleRecovery() {
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        phaseOneRecoveryTimer -=
            Time.deltaTime * tc.localTimeScale;

        if (phaseOneRecoveryTimer > 0) {
            return;
        }

        phaseOneAction = EnemyBossPhaseOneAction.Combo;
    }

    private void HandlePhaseTwoBattle() {
        switch (phaseTwoAction) {
            case EnemyBossPhaseTwoAction.Combo:
                HandleLaterPhaseCombo();
                break;
            case EnemyBossPhaseTwoAction.EnergyWave:
                StartEnergyWave();
                break;
            case EnemyBossPhaseTwoAction.RecoveryAfterEnergyWave:
                HandlePhaseTwoRecovery(
                    EnemyBossPhaseTwoAction.GroundFissure
                );
                break;
            case EnemyBossPhaseTwoAction.GroundFissure:
                StartGroundFissure();
                break;
            case EnemyBossPhaseTwoAction.HarvestDash:
                StartHarvestDash();
                break;
            case EnemyBossPhaseTwoAction.RecoveryAfterHarvestDash:
                HandlePhaseTwoRecovery(
                    EnemyBossPhaseTwoAction.Combo
                );
                break;
        }
    }

    private void HandleLaterPhaseCombo() {
        if (WithInAttackRange()
            && enemy.PlayerDetected()
            && CanAttack()) {
            UpdateAttackTimer();
            enemyBoss.SetAttack(
                enemyBoss.attackWindup,
                enemyBoss.attackCooldown,
                enemyBoss.attackState
            );
            Debug.Log(
                $"Boss {enemyBoss.CurrentPhase} action started: " +
                "Combo"
            );
            stateMachine.ChangeState(enemyBoss.preAttackState);
            return;
        }

        ApproachPlayer();
    }

    private void StartGroundFissure() {
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        Debug.Log(
            $"Boss {enemyBoss.CurrentPhase} action started: " +
            "GroundFissure"
        );
        stateMachine.ChangeState(enemyBoss.groundFissureState);
    }

    private void HandlePhaseTwoRecovery(
        EnemyBossPhaseTwoAction nextAction
    ) {
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        phaseTwoRecoveryTimer -=
            Time.deltaTime * tc.localTimeScale;

        if (phaseTwoRecoveryTimer > 0) {
            return;
        }

        phaseTwoAction = nextAction;
    }

    private void HandlePhaseThreeBattle() {
        switch (phaseThreeAction) {
            case EnemyBossPhaseThreeAction.Combo:
                HandleLaterPhaseCombo();
                break;
            case EnemyBossPhaseThreeAction.EnergyWave:
                StartEnergyWave();
                break;
            case EnemyBossPhaseThreeAction.RecoveryAfterEnergyWave:
                HandlePhaseThreeRecovery(
                    EnemyBossPhaseThreeAction.BidirectionalHarvest
                );
                break;
            case EnemyBossPhaseThreeAction.BidirectionalHarvest:
                StartBidirectionalHarvest();
                break;
            case EnemyBossPhaseThreeAction.DoubleGroundFissure:
                StartGroundFissure();
                break;
            case EnemyBossPhaseThreeAction.RecoveryAfterGroundFissure:
                HandlePhaseThreeRecovery(
                    EnemyBossPhaseThreeAction.Combo
                );
                break;
        }
    }

    private void StartBidirectionalHarvest() {
        if (!enemy.PlayerDetected()) {
            ApproachPlayer();
            return;
        }

        enemyBoss.HandleFlip(DirectionToPlayer());
        Debug.Log(
            "Boss PhaseThree action started: " +
            "BidirectionalHarvest"
        );
        stateMachine.ChangeState(
            enemyBoss.bidirectionalHarvestState
        );
    }

    private void HandlePhaseThreeRecovery(
        EnemyBossPhaseThreeAction nextAction
    ) {
        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        phaseThreeRecoveryTimer -=
            Time.deltaTime * tc.localTimeScale;

        if (phaseThreeRecoveryTimer > 0) {
            return;
        }

        phaseThreeAction = nextAction;
    }

    private void ConfigureCurrentPhase() {
        if (configuredPhase == enemyBoss.CurrentPhase) {
            return;
        }

        configuredPhase = enemyBoss.CurrentPhase;

        if (configuredPhase == EnemyBossPhase.PhaseOne) {
            phaseOneAction = EnemyBossPhaseOneAction.Combo;
            phaseOneRecoveryTimer = 0;
        } else if (configuredPhase == EnemyBossPhase.PhaseTwo) {
            phaseTwoAction = EnemyBossPhaseTwoAction.Combo;
            phaseTwoRecoveryTimer = 0;
        } else if (configuredPhase == EnemyBossPhase.PhaseThree) {
            phaseThreeAction = EnemyBossPhaseThreeAction.Combo;
            phaseThreeRecoveryTimer = 0;
        }
    }

    private void ApproachPlayer() {
        if (!enemy.PlayerDetected()) {
            enemy.HandleFlip(DirectionToPlayer());
        }

        if (WithInAttackRange() || enemyBoss.OutOfMoveRange()) {
            enemyBoss.MoveRaw(0, rb.linearVelocity.y);
            return;
        }

        float xVelocity = DirectionToPlayer() * enemy.battleMoveSpeed;
        enemy.Move(xVelocity, rb.linearVelocity.y);
    }

    private bool WithInDashRange() {
        return DistanceToPlayer() > enemyBoss.minDashDistance;
    }
}

public class EnemyBossGroundFissureState : EnemyState {
    private readonly EnemyBoss enemyBoss;

    public EnemyBossGroundFissureState(
        EnemyBoss enemyBoss,
        StateMachine stateMachine,
        string animBoolName
    ) : base(enemyBoss, stateMachine, animBoolName) {
        this.enemyBoss = enemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.MoveRaw(0, rb.linearVelocity.y);
        enemyBoss.ExitAttackWindow();
        enemyBoss.vfx.SetAttackAlert(false);
        int roundCount =
            enemyBoss.CurrentPhase == EnemyBossPhase.PhaseThree
                ? 2
                : 1;
        enemyBoss.StartGroundFissure(roundCount);
    }

    public override void Update() {
        base.Update();

        enemyBoss.MoveRaw(0, rb.linearVelocity.y);

        if (!enemyBoss.GroundFissurePerformed) {
            return;
        }

        enemyBoss.CompleteGroundFissureAction();
        stateMachine.ChangeState(enemyBoss.battleState);
    }

    public override void Exit() {
        enemyBoss.ClearAttackPresentation();
        enemyBoss.CancelGroundFissure();
        base.Exit();
    }
}

public class EnemyBossComboState : EnemyAttackState {
    private static readonly float[] PhaseOneDamageMultipliers = {
        10f / 18f,
        13f / 18f
    };

    private static readonly float[] LaterPhaseDamageMultipliers = {
        10f / 18f,
        12f / 18f,
        18f / 18f
    };

    private readonly EnemyBoss enemyBoss;

    private float[] currentDamageMultipliers;
    private int currentAttackIndex;
    private bool comboCancelled;
    private bool attackPerformed;

    public int CurrentAttackIndex => currentAttackIndex;
    public int ComboLength => currentDamageMultipliers?.Length ?? 0;

    public EnemyBossComboState(
        EnemyBoss enemyBoss,
        StateMachine stateMachine,
        string animBoolName
    ) : base(enemyBoss, stateMachine, animBoolName) {
        this.enemyBoss = enemyBoss;
    }

    public override void Enter() {
        ConfigureCombo();
        base.Enter();

        enemyBoss.MoveRaw(0, 0);
        CloseCounterWindow();
    }

    public override void Exit() {
        CloseCounterWindow();
        ResetCombo();
        base.Exit();
    }

    public void PerformCurrentHit() {
        if (stateMachine.currentState != this
            || comboCancelled
            || attackPerformed
            || currentDamageMultipliers == null
            || currentAttackIndex >= currentDamageMultipliers.Length) {
            return;
        }

        attackPerformed = true;
        enemyBoss.combat.PerformAttack(
            currentDamageMultipliers[currentAttackIndex]
        );
    }

    public void CancelCombo() {
        comboCancelled = true;
        currentAttackIndex = 0;
        attackPerformed = false;
        triggerCalled = false;
        CloseCounterWindow();
    }

    protected override void HandleStateExit() {
        CloseCounterWindow();

        if (comboCancelled) {
            return;
        }

        if (currentAttackIndex + 1 < currentDamageMultipliers.Length) {
            currentAttackIndex += 1;
            attackPerformed = false;
            triggerCalled = false;
            anim.Play("BossAttack", 0, 0f);
            return;
        }

        enemyBoss.CompleteComboAction();
        stateMachine.ChangeState(enemyBoss.battleState);
    }

    private void ConfigureCombo() {
        currentDamageMultipliers =
            enemyBoss.CurrentPhase == EnemyBossPhase.PhaseOne
                ? PhaseOneDamageMultipliers
                : LaterPhaseDamageMultipliers;
        currentAttackIndex = 0;
        comboCancelled = false;
        attackPerformed = false;
    }

    private void ResetCombo() {
        currentAttackIndex = 0;
        comboCancelled = false;
        attackPerformed = false;
        triggerCalled = false;
    }

    private void CloseCounterWindow() {
        enemyBoss.ExitAttackWindow();
        enemyBoss.vfx.SetAttackAlert(false);
    }
}
