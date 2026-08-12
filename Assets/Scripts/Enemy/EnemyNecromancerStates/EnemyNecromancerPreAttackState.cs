using UnityEngine;

public class EnemyNecromancerPreAttackState : EnemyPreAttackState {
    EnemyNecromancer enemyNecromancer;

    public EnemyNecromancerPreAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyNecromancer = enemy as EnemyNecromancer;
    }

    public override void Enter() {
        base.Enter();

        enemyNecromancer.UpdateTargetPosition();
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }
}
