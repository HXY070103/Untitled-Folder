using UnityEngine;

public class EnemyDarkKnightSwordwaveState : EnemyState {
    private EnemyDarkKnight enemyDarkKnight;

    public EnemyDarkKnightSwordwaveState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyDarkKnight = enemy as EnemyDarkKnight;
    }

    public override void Enter() {
        base.Enter();
        
        enemy.Move(0, 0);
    }

    public override void Update() {
        base.Update();

        if (triggerCalled) {
            stateMachine.ChangeState(enemyDarkKnight.battleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
