using UnityEngine;

public class EnemySkeletonKingBattleState : EnemyBattleState {
    private EnemySkeletonKing enemySkeletonKing;

    private int attackIndex = 1;

    public EnemySkeletonKingBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemySkeletonKing = enemy as EnemySkeletonKing;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (WithInAttackRange() && enemySkeletonKing.PlayerDetected() && CanAttack()) {
            HandleAttack();
            UpdateAttackTimer();
            UpdateAttackCounter();
        } else {
            if (!enemySkeletonKing.PlayerDetected()) {
                enemySkeletonKing.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemySkeletonKing.wallDetected) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemySkeletonKing.battleMoveSpeed;
            enemySkeletonKing.Move(xVelocity, rb.linearVelocity.y);
            enemySkeletonKing.sfx?.TryPlayMove(0.2f);
        }
    }

    protected override bool WithInAttackRange() {
        switch (attackIndex) {
            case 1 or 2: 
                return DistanceToPlayer() < enemySkeletonKing.attackDistance;
            case 3:
                return DistanceToPlayer() < enemySkeletonKing.shockwaveDistance;
            default: 
                return false;
        }
    }

    private void HandleAttack() {
        switch (attackIndex) {
            case 1 or 2: 
                ApplyAttack();
                break;
            case 3: 
                ApplyShockwave();
                break;
        }
    }

    private void ApplyAttack() {
        enemySkeletonKing.SetAttack(
                enemySkeletonKing.basicAttackWindup[enemySkeletonKing.phaseIndex], 
                enemySkeletonKing.basicAttackCooldown[enemySkeletonKing.phaseIndex], 
                enemySkeletonKing.attackState
            );
        stateMachine.ChangeState(enemy.preAttackState); 
    }

    private void ApplyShockwave() {
        enemySkeletonKing.SetAttack(
                enemySkeletonKing.shockwaveWindup[enemySkeletonKing.phaseIndex], 
                enemySkeletonKing.shockwaveCooldown[enemySkeletonKing.phaseIndex], 
                enemySkeletonKing.shockwaveState
            );
        stateMachine.ChangeState(enemy.preAttackState); 
    }

    private void UpdateAttackCounter() {
        attackIndex += 1;

        if (attackIndex > enemySkeletonKing.numOfAttack) {
            attackIndex = 1;
        }
    }
}