using UnityEngine;

public class EnemyDarkKnightBattleState : EnemyBattleState {
    private EnemyDarkKnight enemyDarkKnight;

    private int attackIndex = 1;
    private bool hasChangedPhase = false;

    public EnemyDarkKnightBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyDarkKnight = enemy as EnemyDarkKnight;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        if (!hasChangedPhase && IsLowHP()) {
            hasChangedPhase = true;
            enemyDarkKnight.EnterNextPhase();
        }

        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (WithInAttackRange() && enemyDarkKnight.PlayerDetected() && CanAttack()) {
            switch (attackIndex) {
                case 1: 
                    ApplyComboAttack();
                    break;
                case 2: 
                    ApplySwordwave();
                    break;
                case 3:
                    ApplyDashAttack();
                    break;
            }

            UpdateAttackTimer();
            UpdateAttackCounter();
        } else {
            if (!enemyDarkKnight.PlayerDetected()) {
                enemyDarkKnight.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemyDarkKnight.wallDetected) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemyDarkKnight.battleMoveSpeed;
            enemyDarkKnight.Move(xVelocity, rb.linearVelocity.y);
        }
    }

    protected override bool WithInAttackRange() {
        switch (attackIndex) {
            case 1: 
                return DistanceToPlayer() < enemyDarkKnight.attackDistance;
            case 2: 
                return DistanceToPlayer() < enemyDarkKnight.swordwaveDistance;
            case 3:
                return DistanceToPlayer() < enemyDarkKnight.dashDistance;
            default: 
                return false;
        }
    }

    private void ApplyComboAttack() {
        enemyDarkKnight.SetAttack(
                enemyDarkKnight.comboAttackWindup[enemyDarkKnight.phaseIndex], 
                enemyDarkKnight.comboAttackCooldown[enemyDarkKnight.phaseIndex], 
                enemyDarkKnight.attackState
            );
        stateMachine.ChangeState(enemyDarkKnight.preAttackState);
    }

    private void ApplySwordwave() {
        enemyDarkKnight.SetAttack(
                enemyDarkKnight.swordwaveWindup[enemyDarkKnight.phaseIndex], 
                enemyDarkKnight.swordwaveCooldown[enemyDarkKnight.phaseIndex], 
                enemyDarkKnight.swordwaveState
            );
        stateMachine.ChangeState(enemyDarkKnight.preAttackState);
    }

    private void ApplyDashAttack() {
        enemyDarkKnight.SetAttack(
                enemyDarkKnight.dashAttackWindup, 
                enemyDarkKnight.dashAttackCooldown, 
                enemyDarkKnight.dashAttackState
            );
        stateMachine.ChangeState(enemyDarkKnight.preAttackState);
    }

    private void UpdateAttackCounter() {
        attackIndex += 1;

        if (attackIndex > enemyDarkKnight.numOfAttack) {
            attackIndex = 1;
        }
    }

    private bool IsLowHP() {
        return enemyDarkKnight.health.GetHPPercentage() < enemyDarkKnight.HPThreshold;
    }
}