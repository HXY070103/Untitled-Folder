using UnityEngine;

public class EnemyIdleState : EnemyGroundedState {
    public EnemyIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        stateTimer = enemy.idleDurarion;
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
