using UnityEngine;

public class EnemySkeletonKingBattleState : EnemyBattleState {
    private EnemySkeletonKing enemySkeletonKing;

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
            UpdateAttackTimer();

            if (IsLowHealth()) {
                enemySkeletonKing.attackCooldown = enemySkeletonKing.summonAttackCooldown;
                stateMachine.ChangeState(enemySkeletonKing.summonState);
            } else {
                stateMachine.ChangeState(enemySkeletonKing.attackState); 
            }
        } else {
            if (!enemySkeletonKing.PlayerDetected()) {
                enemySkeletonKing.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemySkeletonKing.wallDetected || IsLowHealth()) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemySkeletonKing.GetBattleMoveSpeed();
            enemySkeletonKing.SetVelocity(xVelocity, rb.linearVelocity.y);
        }
    }

    private bool IsLowHealth() {
        return enemy.health.GetHPPercentage() < enemySkeletonKing.healthThresholdForSummon;
    }
}