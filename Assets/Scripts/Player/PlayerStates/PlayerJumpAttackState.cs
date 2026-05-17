using UnityEngine;

public class PlayerJumpAttackState : PlayerState {
    private bool touchedGround;

    public PlayerJumpAttackState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        touchedGround = false;
        player.SetVelocity(player.facingDir * player.jumpAttackSpeed.x, player.jumpAttackSpeed.y);
    }

    public override void Update() {
        base.Update();

        if (player.groundDetected && !touchedGround) {
            touchedGround = true;
            anim.SetTrigger("JumpAttackTrigger"); 
            player.SetVelocity(0, rb.linearVelocity.y);
        }

        if (triggerCalled && player.groundDetected) {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit() {
        base.Exit(); 
    }
}
 