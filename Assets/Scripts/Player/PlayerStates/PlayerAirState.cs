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
            float xVelocity = player.moveInput.x * player.moveSpeed * player.inAirMoveMultiplier;
            player.Move(xVelocity, rb.linearVelocity.y);
        }

        if (player.canAttack && input.Player.Attack.WasPressedThisFrame()) {
            HandleAttack();
        }
    }

    public override void Exit() {
        base.Exit();
    }

    private void HandleAttack() {
        if (player.isHeavyAttack) {
            stateMachine.ChangeState(player.heavyAttackState);
        } else {
            stateMachine.ChangeState(player.jumpAttackState);
        }
    }
}
 