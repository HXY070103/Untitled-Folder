using UnityEngine;

public class EnemyStoneGuardShieldState : EnemyState {
    private readonly EnemyStoneGuard stoneGuard;

    private float elapsedTime;
    private bool playerPushed;

    public EnemyStoneGuardShieldState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        elapsedTime = 0;
        playerPushed = false;

        stoneGuard.BeginShieldCast();
        stoneGuard.MoveRaw(0, rb.linearVelocity.y);
        stoneGuard.ExitAttackWindow();
        stoneGuard.vfx.SetAttackAlert(false);
    }

    public override void Update() {
        base.Update();

        elapsedTime += Time.deltaTime * tc.localTimeScale;

        if (!playerPushed && elapsedTime >= stoneGuard.shieldWindup) {
            playerPushed = true;
            stoneGuard.PushPlayerAway();
        }

        if (elapsedTime >= stoneGuard.shieldDuration) {
            stateMachine.ChangeState(stoneGuard.battleState);
        }
    }
}
