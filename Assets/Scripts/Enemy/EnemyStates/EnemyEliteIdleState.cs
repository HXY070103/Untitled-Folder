using UnityEngine;

public class EnemyEliteIdleState : EnemyIdleState {
    public EnemyEliteIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
    }

    public override void Enter() {
        base.Enter();

        stateTimer = float.PositiveInfinity;
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }
}
