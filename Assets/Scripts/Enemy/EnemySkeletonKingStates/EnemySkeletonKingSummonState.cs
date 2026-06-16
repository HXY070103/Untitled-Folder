using System.Collections;
using UnityEngine;

public class EnemySkeletonKingSummonState : EnemyState{
    private EnemySkeletonKing enemySkeletonKing;

    public EnemySkeletonKingSummonState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemySkeletonKing = enemy as EnemySkeletonKing;
    }

    public override void Enter() {
        base.Enter();

        enemySkeletonKing.SetVelocity(0, 0);
        enemySkeletonKing.StartSummon();
        enemySkeletonKing.SetSummonPerformed(false);
    }

    public override void Update() {
        base.Update();

        if (enemySkeletonKing.SummonPerformed) {
            stateMachine.ChangeState(enemySkeletonKing.battleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
