using UnityEditor.PackageManager.UI;
using UnityEngine;

public class PlayerAirState : PlayerState {
    public PlayerAirState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (player.moveInput.x != 0) {
            player.SetVelocity(
                    player.moveInput.x * player.moveSpeed * player.inAirMoveMultiplier, 
                    rb.linearVelocity.y
                );
        }

        if (input.Player.Attack.WasPressedThisFrame()) {
            stateMachine.ChangeState(player.jumpAttackState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
 