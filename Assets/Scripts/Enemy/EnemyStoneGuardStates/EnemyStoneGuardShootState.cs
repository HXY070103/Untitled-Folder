using UnityEngine;

public class EnemyStoneGuardShootState : EnemyState {
    private EnemyStoneGuard enemyStoneGuard;

    public EnemyStoneGuardShootState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        enemyStoneGuard = enemy as EnemyStoneGuard;
    }

    public override void Enter() {
        base.Enter();
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

    private void HandleStateExit() {
        if (enemyStoneGuard.isPhaseTwo) {
            stateMachine.ChangeState(enemyStoneGuard.laserState);
        } else {
            stateMachine.ChangeState(enemyStoneGuard.battleState);
        }
    }
}
