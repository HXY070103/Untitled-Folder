using UnityEngine;

public class PlayerIdleState : PlayerGroundedState {
    public PlayerIdleState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        player.Move(0, rb.linearVelocity.y);
    }

    public override void Update() {
        base.Update();

        if (player.wallDetected && player.moveInput.x == player.facingDir) {
            return;
        }

        if (player.moveInput.x != 0) {
            stateMachine.ChangeState(player.moveState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
 