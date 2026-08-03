using UnityEngine;

public class EnemyBossSummonState : EnemyState {
    private EnemyBoss enemyBoss;

    public EnemyBossSummonState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.MoveRaw(0, 0);
        enemyBoss.SetSummonPerformed(false);
        enemyBoss.StartSummon();
    }

    public override void Update() {
        base.Update();

        if (enemyBoss.SummonPerformed) {
            enemyBoss.CompleteSummon();
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
