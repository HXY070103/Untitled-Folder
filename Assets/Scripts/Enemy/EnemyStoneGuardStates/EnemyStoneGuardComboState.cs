using UnityEngine;

public class EnemyStoneGuardComboState : EnemyState {
    private const string LaserAnimBoolName = "Laser";

    private readonly EnemyStoneGuard stoneGuard;

    private EnemyStoneGuardLaser activeLaser;
    private float elapsedTime;
    private int currentComboAttackId;
    private bool projectileReleased;
    private bool laserStarted;
    private bool laserFired;
    private bool coreExposureQueued;

    public EnemyStoneGuardComboState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        elapsedTime = 0;
        currentComboAttackId += 1;
        projectileReleased = false;
        laserStarted = false;
        laserFired = false;
        coreExposureQueued = false;

        anim.SetBool(LaserAnimBoolName, false);
        stoneGuard.MoveRaw(0, rb.linearVelocity.y);
        stoneGuard.ExitAttackWindow();
        stoneGuard.vfx.SetAttackAlert(false);
        stoneGuard.BeginComboAttack();
    }

    public override void Update() {
        base.Update();

        elapsedTime += Time.deltaTime * tc.localTimeScale;
        activeLaser?.SetTimeScale(tc.localTimeScale);

        if (!projectileReleased && elapsedTime >= stoneGuard.projectileReleaseTime) {
            projectileReleased = true;
            stoneGuard.ReleaseArmProjectile(currentComboAttackId);
        }

        float laserStartTime = stoneGuard.projectileReleaseTime + stoneGuard.comboLaserStartDelay;

        if (!laserStarted && elapsedTime >= laserStartTime) {
            StartLaser();
        }

        float laserFireTime = laserStartTime + stoneGuard.laserAimDuration;

        if (!laserFired && elapsedTime >= laserFireTime) {
            laserFired = true;
            activeLaser?.Fire();
        }

        if (elapsedTime >= laserFireTime + stoneGuard.laserBeamDuration) {
            FinishLaser();
            stateMachine.ChangeState(coreExposureQueued ? stoneGuard.coreExposedState : stoneGuard.battleState);
        }
    }

    public override void Exit() {
        FinishLaser();
        anim.SetBool(LaserAnimBoolName, false);
        base.Exit();
    }

    public void HandleArmProjectileCounter(int comboAttackId) {
        if (comboAttackId == currentComboAttackId && !laserFired) {
            stateMachine.ChangeState(stoneGuard.coreExposedState);
            return;
        }

        coreExposureQueued = true;
    }

    private void StartLaser() {
        laserStarted = true;
        anim.SetBool(animBoolName, false);
        anim.SetBool(LaserAnimBoolName, true);

        activeLaser = stoneGuard.CreateLaser();
        activeLaser?.SetTimeScale(tc.localTimeScale);
    }

    private void FinishLaser() {
        activeLaser?.Finish();
        activeLaser = null;
    }
}
