using UnityEngine;

public class EnemyBossSummonState : EnemyState {
    private EnemyBoss enemyBoss;

    public EnemyBossSummonState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.Move(0, 0);
        enemyBoss.StartSummon();
        enemyBoss.SetSummonPerformed(false);
    }

    public override void Update() {
        base.Update();

        if (enemyBoss.SummonPerformed) {
            stateMachine.ChangeState(enemyBoss.battleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
