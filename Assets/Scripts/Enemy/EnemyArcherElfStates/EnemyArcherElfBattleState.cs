using UnityEngine;

public class EnemyArcherElfBattleState : EnemyBattleState {
    private bool reachDeadEnd;
    private int deadEndDir;

    public EnemyArcherElfBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        reachDeadEnd = false;
    }

    public override void Update() {
        if (!enemy.groundDetected || enemy.wallDetected) {
            reachDeadEnd = true;
            deadEndDir = enemy.facingDir;
        }

        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (WithInAttackRange() && enemy.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            stateMachine.ChangeState(enemy.preAttackState); 
        } else {
            if (CanWalkAway() && InWalkAwayRange()) {
                float xVeloity = -DirectionToPlayer() * enemy.battleMoveSpeed;
                enemy.Move(xVeloity, rb.linearVelocity.y);
            } else {
                enemy.Move(0, rb.linearVelocity.y);

                if (!enemy.PlayerDetected()) {
                    enemy.HandleFlip(DirectionToPlayer());
                }
            }
        }
    }

    private bool CanWalkAway() {
        return !reachDeadEnd || -DirectionToPlayer() != deadEndDir;
    }

    private bool InWalkAwayRange() {
        return DistanceToPlayer() < enemy.attackDistance * 0.8f;
    }
}
