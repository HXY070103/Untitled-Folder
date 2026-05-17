using UnityEngine;

public class PlayerWallSlideState : PlayerState {
    public PlayerWallSlideState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        HandleWallSlide();

        if (input.Player.Jump.WasPressedThisFrame()) {
            stateMachine.ChangeState(player.wallJumpState);
        }

        if (player.groundDetected) {
            stateMachine.ChangeState(player.idleState);
            
            if (player.facingDir != player.moveInput.x) {
                player.Flip();
            }
        }

        if (!player.wallDetected) {
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit() {
        base.Exit();
    }

    private void HandleWallSlide() {
        if (player.moveInput.y < 0) {
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y);
        } else {
            player.SetVelocity(player.moveInput.x, rb.linearVelocity.y * player.wallSlideMultiplier);
        }
    }
}
 