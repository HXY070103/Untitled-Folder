using UnityEngine;

public class EnemyStoneGuardLaserState : EnemyState {
    private EnemyStoneGuard enemyStoneGuard;

    public EnemyStoneGuardLaserState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyStoneGuard = enemy as EnemyStoneGuard;
    }

    public override void Enter() {
        base.Enter();

        anim.SetBool("LaserCastPerformed", false);

        enemyStoneGuard.CreateLaser();
        stateTimer = enemyStoneGuard.laserDuration;
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
            anim.SetBool("LaserCastPerformed", true);
        }

        if (triggerCalled) {
            stateMachine.ChangeState(enemyStoneGuard.battleState);
        }
    }

    public override void Exit() {
        base.Exit();

        enemyStoneGuard.DestroyLaser();
    }
}
