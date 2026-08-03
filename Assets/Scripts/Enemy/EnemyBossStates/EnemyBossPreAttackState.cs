using UnityEngine;

public class EnemyBossPreAttackState : EnemyPreAttackState {
    EnemyBoss enemyBoss;

    public EnemyBossPreAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.SetDashAttackDir(DirectionToPlayer());
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }
}
