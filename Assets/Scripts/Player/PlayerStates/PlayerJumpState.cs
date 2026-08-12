using UnityEngine;

public class PlayerJumpState : PlayerAirState {
    public PlayerJumpState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        player.Move(rb.linearVelocity.x, player.jumpSpeed);
        player.sfx?.PlayJump();
    }

    public override void Update() {
        base.Update();

        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState) {
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
 