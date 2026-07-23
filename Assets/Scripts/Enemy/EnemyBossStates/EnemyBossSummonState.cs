using UnityEngine;

public class EnemyBossSummonState : EnemyState {
    private EnemyBoss enemyBoss;

    public EnemyBossSummonState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.ClearAttackPresentation();
        enemyBoss.Move(0, 0);
        enemyBoss.SetSummonPerformed(false);
        enemyBoss.StartSummon();
    }

    public override void Update() {
        base.Update();
        enemyBoss.RefreshSummonCompletion();

        if (enemyBoss.SummonPerformed) {
            enemyBoss.CompleteSummonPhase();
        }
    }

    public override void Exit() {
        enemyBoss.ClearAttackPresentation();
        base.Exit();
    }
}
