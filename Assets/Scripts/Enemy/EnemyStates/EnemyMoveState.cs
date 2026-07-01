using UnityEngine;

public class EnemyMoveState : EnemyGroundedState {
    public EnemyMoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        if (!enemy.groundDetected || enemy.wallDetected) {
            enemy.Flip();
        }
    }

    public override void Update() {
        base.Update();

        float xVelocity = enemy.facingDir * enemy.moveSpeed;
        enemy.Move(xVelocity, rb.linearVelocity.y);

        if (!enemy.groundDetected || enemy.wallDetected) {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
