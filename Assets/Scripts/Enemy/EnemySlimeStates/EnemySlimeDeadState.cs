using UnityEngine;

public class EnemySlimeDeadState : EnemyDeadState{
    private EnemySlime enemySlime;

    public EnemySlimeDeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemySlime = enemy as EnemySlime;
    }
    
    public override void Enter() {
        base.Enter();

        enemySlime.CreateSlimeOnDeath();
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }
}
