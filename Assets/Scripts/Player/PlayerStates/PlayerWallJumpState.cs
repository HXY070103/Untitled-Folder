using UnityEngine;

public class PlayerWallJumpState : PlayerState {
    public PlayerWallJumpState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        player.SetVelocity(-player.facingDir * player.wallJumpSpeed.x, player.wallJumpSpeed.y);
    }

    public override void Update() {
        base.Update();

        if (rb.linearVelocity.y < 0) {
            stateMachine.ChangeState(player.fallState);
        }

        if (player.wallDetected) {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
 