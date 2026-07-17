using UnityEngine;

public class EnemyAttackState : EnemyState {
    public EnemyAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        enemy.Move(0, 0);
    }

    public override void Update() {
        base.Update();

        if (triggerCalled) {
            HandleStateExit();
        }
    }

    public override void Exit() {
        base.Exit();
    }


    protected virtual void HandleStateExit() {
        stateMachine.ChangeState(enemy.battleState);
    }
}
