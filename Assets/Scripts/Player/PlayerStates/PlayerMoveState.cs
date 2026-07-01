using UnityEngine;

public class PlayerMoveState : PlayerGroundedState {
    public PlayerMoveState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (player.moveInput.x == 0 || player.wallDetected) {
            stateMachine.ChangeState(player.idleState);
        }

        float xVelocity = player.moveInput.x * player.moveSpeed;
        player.Move(xVelocity, rb.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();
    }
}
