using UnityEngine;

public class PlayerWallJumpState : PlayerState {
    public PlayerWallJumpState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        stateTimer = player.wallJumpDuration;

        float xVelocity = -player.facingDir * player.wallJumpSpeed.x;
        float yVelocity = player.wallJumpSpeed.y;
        player.Move(xVelocity, yVelocity);
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
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
 