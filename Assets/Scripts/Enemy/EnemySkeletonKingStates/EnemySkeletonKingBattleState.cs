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
        if (enemySkeletonKing.ArenaAttackInProgress) {
            enemySkeletonKing.Move(0, rb.linearVelocity.y);
            return;
        }

        if (enemySkeletonKing.PlayerDetected() && enemySkeletonKing.CanUseHorizontalSweep()) {
            enemySkeletonKing.StartHorizontalSweep();
            return;
        }

        if (WithInAttackRange() && enemySkeletonKing.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();

            if (enemySkeletonKing.ShouldSummon()) {
                stateMachine.ChangeState(enemySkeletonKing.summonState);
            } else {
                stateMachine.ChangeState(enemySkeletonKing.preAttackState); 
            }
        } else {
            if (!enemySkeletonKing.PlayerDetected()) {
                enemySkeletonKing.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemySkeletonKing.wallDetected) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemySkeletonKing.battleMoveSpeed;
            enemySkeletonKing.Move(xVelocity, rb.linearVelocity.y);
        }
    }

}
