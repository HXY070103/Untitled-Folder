using UnityEngine;

public class PlayerFallState : PlayerAirState {
    public PlayerFallState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (player.groundDetected) {
            player.sfx?.PlayJumpland();
            stateMachine.ChangeState(player.idleState);
        }

        if (player.wallDetected) {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
 