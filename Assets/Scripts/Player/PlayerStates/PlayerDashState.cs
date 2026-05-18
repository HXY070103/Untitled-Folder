using UnityEngine;

public class PlayerDashState : PlayerState {
    private float originalGravityScale;
    private int dashDir;

    public PlayerDashState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        vfx.DoImageEchoEffect(player.dashDuration);

        stateTimer = player.dashDuration;
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        dashDir = player.moveInput.x != 0 ? (int) player.moveInput.x : player.facingDir;
    }

    public override void Update() {
        base.Update();

        CancelDashIfNeeded();
        player.SetVelocity(dashDir * player.dashSpeed, 0);

        if (stateTimer < 0) {
            HandleStateExit();
        }
    }

    public override void Exit() {
        base.Exit();

        rb.gravityScale = originalGravityScale;
        player.SetVelocity(0, 0);
    }

    private void HandleStateExit() {
        if (player.groundDetected) {
            stateMachine.ChangeState(player.idleState);
        } else {
            stateMachine.ChangeState(player.fallState);
        }
    }

    private void CancelDashIfNeeded() {
        if (player.wallDetected) {
            if (player.groundDetected) {
                stateMachine.ChangeState(player.idleState);
            } else {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
    }
}
 