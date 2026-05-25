using UnityEngine;

public class PlayerGroundedState : PlayerState {
    public PlayerGroundedState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (!player.groundDetected && rb.linearVelocity.y < 0) {
            stateMachine.ChangeState(player.fallState);
        }

        if (input.Player.Jump.WasPressedThisFrame()) {
            stateMachine.ChangeState(player.jumpState);
        }

        if (player.CanAttack && input.Player.Attack.WasPressedThisFrame()) {
            HandleAttack();
        }

        if (input.Player.CounterAttack.WasPressedThisFrame()) {
            stateMachine.ChangeState(player.counterAttackState);
        }
    }

    public override void Exit() {
        base.Exit();
    }

    private void HandleAttack() {
        if (player.IsHeavyAttack) {
            stateMachine.ChangeState(player.heavyAttackState);
        } else {
            stateMachine.ChangeState(player.basicAttackState);
        }
    }
}
