using UnityEngine;

public class EnemyPreAttackState : EnemyState {
    public EnemyPreAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        stateTimer = enemy.currentWindup;
        enemy.vfx.PlayPreAttackVFX();
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
            stateMachine.ChangeState(enemy.currentAttack);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
