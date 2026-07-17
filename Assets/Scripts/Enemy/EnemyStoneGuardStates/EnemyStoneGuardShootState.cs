using UnityEngine;

public class EnemyStoneGuardShootState : EnemyState {
    private readonly EnemyStoneGuard stoneGuard;

    private float elapsedTime;
    private bool projectileReleased;

    public EnemyStoneGuardShootState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        elapsedTime = 0;
        projectileReleased = false;

        stoneGuard.MoveRaw(0, rb.linearVelocity.y);
        stoneGuard.ExitAttackWindow();
        stoneGuard.vfx.SetAttackAlert(false);
    }

    public override void Update() {
        base.Update();

        elapsedTime += Time.deltaTime * tc.localTimeScale;

        if (!projectileReleased && elapsedTime >= stoneGuard.projectileReleaseTime) {
            projectileReleased = true;
            stoneGuard.ReleaseArmProjectile();
        }

        if (elapsedTime >= stoneGuard.shootDuration) {
            stateMachine.ChangeState(stoneGuard.battleState);
        }
    }
}
