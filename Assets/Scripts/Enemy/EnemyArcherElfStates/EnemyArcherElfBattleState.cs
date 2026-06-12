using UnityEngine;

public class EnemyArcherElfBattleState : EnemyBattleState {
    private bool reachDeadEnd;

    public EnemyArcherElfBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        reachDeadEnd = false;
    }

    public override void Update() {
        base.Update();

        if (!enemy.groundDetected || enemy.wallDetected) {
            reachDeadEnd = true;
        }
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (CanAttack()) {
            enemy.SetVelocity(0, rb.linearVelocity.y);

            if (!enemy.PlayerDetected()) {
                enemy.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() && enemy.PlayerDetected()) {
                UpdateAttackTimer();
                stateMachine.ChangeState(enemy.attackState);
            }
        } else { 
            bool shouldWalkAway = 
                !reachDeadEnd && DistanceToPlayer() < enemy.attackDistance * 0.8f;

            if (shouldWalkAway) {
                float xVeloity = -DirectionToPlayer() * enemy.GetBattleMoveSpeed();
                enemy.SetVelocity(xVeloity, rb.linearVelocity.y);
            } else {
                enemy.SetVelocity(0, rb.linearVelocity.y);

                if (!enemy.PlayerDetected()) {
                    enemy.HandleFlip(DirectionToPlayer());
                }
            }
        }
    }
}
