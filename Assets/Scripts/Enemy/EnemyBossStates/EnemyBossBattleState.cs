using UnityEngine;

public class EnemyBossBattleState : EnemyBattleState {
    private EnemyBoss enemyBoss;

    private int attackIndex = 1;

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
        if (WithInAttackRange() && enemy.PlayerDetected() && CanAttack()) {
            HandleAttackLoop();
            UpdateAttackTimer();
            UpdateAttackCounter();
        } else {
            if (!enemyBoss.PlayerDetected()) {
                enemyBoss.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemyBoss.wallDetected) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemyBoss.battleMoveSpeed;
            enemyBoss.Move(xVelocity, rb.linearVelocity.y);
            enemyBoss.sfx?.TryPlayMove(0.2f);
        }
    }

    private void HandleAttackLoop() {
        switch (enemyBoss.phaseIndex) {
            case 0: 
                HandlePhaseOneAttackLoop();
                break;
            case 1: 
                HandlePhaseTwoAttackLoop();
                break;
            case 2: 
                HandlePhaseThreeAttackLoop();
                break;
        }
    }

    private void HandlePhaseOneAttackLoop() {
        switch (attackIndex) {
            case 1 or 2: 
                ApplyComboAttack();
                break;
            case 3: 
                ApplyEnergyWave();
                break;
            case 4: 
                ApplyDashAttack();
                break;
        }
    }

    private void HandlePhaseTwoAttackLoop() {
        switch (attackIndex) {
            case 1: 
                ApplyComboAttack();
                break;
            case 2: 
                ApplyEnergyWave();
                break;
            case 3: 
                ApplyDashAttack();
                break;
            case 4:
                ApplyLightning();
                break;
        }
    }

    private void HandlePhaseThreeAttackLoop() {
        switch (attackIndex) {
            case 1: 
                ApplyComboAttack();
                break;
            case 2: 
                ApplyEnergyWave();
                break;
            case 3: 
                ApplyDashAttack();
                break;
            case 4: 
                ApplyLightning();
                break;
        }
    }

    protected override bool WithInAttackRange() {
        switch (enemyBoss.phaseIndex) {
            case 0: 
                return WithInPhaseOneAttackRange();
            case 1: 
                return WithInPhaseTwoAttackRange();
            case 2: 
                return WithInPhaseThreeAttackRange();
            default: 
                return false;
        }
    }

    private bool WithInPhaseOneAttackRange() {
        switch (attackIndex) {
            case 1 or 2: 
                return DistanceToPlayer() < enemyBoss.attackDistance;
            case 3: 
                return DistanceToPlayer() < enemyBoss.energyWaveDistance;
            case 4: 
                return DistanceToPlayer() < enemyBoss.dashAttackDistance;
            default: 
                return false;
        }
    }

    private bool WithInPhaseTwoAttackRange() {
        switch (attackIndex) {
            case 1: 
                return DistanceToPlayer() < enemyBoss.attackDistance;
            case 2: 
                return DistanceToPlayer() < enemyBoss.energyWaveDistance;
            case 3: 
                return DistanceToPlayer() < enemyBoss.dashAttackDistance;
            case 4: 
                return DistanceToPlayer() < enemyBoss.lightningDistance;
            default: 
                return false;
        }
    }

    private bool WithInPhaseThreeAttackRange() {
        switch (attackIndex) {
            case 1: 
                return DistanceToPlayer() < enemyBoss.attackDistance;
            case 2: 
                return DistanceToPlayer() < enemyBoss.energyWaveDistance;
            case 3: 
                return DistanceToPlayer() < enemyBoss.dashAttackDistance;
            case 4: 
                return DistanceToPlayer() < enemyBoss.lightningDistance;
            default: 
                return false;
        }
    }

    private void ApplyComboAttack() {
        enemyBoss.SetAttack(
                enemyBoss.comboAttackWindup[enemyBoss.phaseIndex], 
                enemyBoss.comboAttackCooldown[enemyBoss.phaseIndex], 
                enemyBoss.attackState
            );
        stateMachine.ChangeState(enemyBoss.preAttackState);
    }

    private void ApplyEnergyWave() {
        enemyBoss.SetAttack(
                enemyBoss.energyWaveWindup[enemyBoss.phaseIndex], 
                enemyBoss.energyWaveCooldown[enemyBoss.phaseIndex], 
                enemyBoss.energyWaveState
            );
        stateMachine.ChangeState(enemyBoss.preAttackState);
    }

    private void ApplyDashAttack() {
        enemyBoss.SetAttack(
                enemyBoss.dashAttackWindup[enemyBoss.phaseIndex], 
                enemyBoss.dashAttackCooldown[enemyBoss.phaseIndex], 
                enemyBoss.dashAttackState
            );
        stateMachine.ChangeState(enemyBoss.preAttackState);
    }

    private void ApplyLightning() {
        if (enemyBoss.numOfLightningRound[enemyBoss.phaseIndex] == 0) {
            return;
        }

        enemyBoss.SetAttack(
                enemyBoss.lightningWindup[enemyBoss.phaseIndex], 
                enemyBoss.lightningCooldown[enemyBoss.phaseIndex], 
                enemyBoss.lightningState
            );
        stateMachine.ChangeState(enemyBoss.lightningState);
    }

    private void UpdateAttackCounter() {
        attackIndex += 1;

        if (attackIndex > enemyBoss.numOfAttack[enemyBoss.phaseIndex]) {
            attackIndex = 1;
        }
    }
}
