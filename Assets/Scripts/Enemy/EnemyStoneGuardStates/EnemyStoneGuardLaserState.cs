using UnityEngine;

public class EnemyStoneGuardLaserState : EnemyState {
    private readonly EnemyStoneGuard stoneGuard;

    private EnemyStoneGuardLaser activeLaser;
    private float elapsedTime;
    private bool laserFired;

    public EnemyStoneGuardLaserState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        elapsedTime = 0;
        laserFired = false;

        stoneGuard.MoveRaw(0, rb.linearVelocity.y);
        stoneGuard.ExitAttackWindow();
        stoneGuard.vfx.SetAttackAlert(false);

        activeLaser = stoneGuard.CreateLaser();
        activeLaser?.SetTimeScale(tc.localTimeScale);
    }

    public override void Update() {
        base.Update();

        elapsedTime += Time.deltaTime * tc.localTimeScale;
        activeLaser?.SetTimeScale(tc.localTimeScale);

        if (!laserFired && elapsedTime >= stoneGuard.laserAimDuration) {
            laserFired = true;
            activeLaser?.Fire();
        }

        if (elapsedTime >= stoneGuard.laserAimDuration + stoneGuard.laserBeamDuration) {
            FinishLaser();
            stateMachine.ChangeState(stoneGuard.battleState);
        }
    }

    public override void Exit() {
        FinishLaser();
        base.Exit();
    }

    private void FinishLaser() {
        activeLaser?.Finish();
        activeLaser = null;
    }
}
