using UnityEngine;

public class EnemySkeletonKingBattleState : EnemyBattleState {
    private EnemySkeletonKing enemySkeletonKing;

    private bool shouldSummon;

    public EnemySkeletonKingBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemySkeletonKing = enemy as EnemySkeletonKing;

        shouldSummon = false;
    }

    public override void Enter() {
        base.Enter();

    }

    public override void Update() {
        if (!shouldSummon && IsLowHealth()) {
            shouldSummon = true;
            enemySkeletonKing.attackCooldown = enemySkeletonKing.summonAttackCooldown;
            enemySkeletonKing.attackDistance = enemySkeletonKing.summonAttackDistance;
        }

        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (WithInAttackRange() && enemySkeletonKing.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();

            if (shouldSummon) {
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