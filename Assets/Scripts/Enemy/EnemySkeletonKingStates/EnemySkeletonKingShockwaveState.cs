using UnityEngine;

public class EnemySkeletonKingShockwaveState : EnemyState {
    public EnemySkeletonKingShockwaveState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        enemy.MoveRaw(0, 0);
    }

    public override void Update() {
        base.Update();

        if (triggerCalled) {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
