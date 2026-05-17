using UnityEngine;

public class EnemyGroundedState : EnemyState {
    public EnemyGroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (enemy.PlayerDetected()) {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
