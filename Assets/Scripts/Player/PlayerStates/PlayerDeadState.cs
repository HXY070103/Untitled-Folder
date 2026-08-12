using UnityEngine;

public class PlayerDeadState : PlayerState {
    public PlayerDeadState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        input.Disable();
        stateMachine.SwitchOff();
        player.sfx?.PlayDeath();
    }

    public override void Update() {
        base.Update();

        if (player.groundDetected) {
            rb.simulated = false;
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
 