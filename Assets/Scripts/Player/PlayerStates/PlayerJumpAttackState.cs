using UnityEngine;

public class PlayerJumpAttackState : PlayerState {
    private SkillPhase phase;
    private bool touchedGround;

    public PlayerJumpAttackState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {
        phase = player.GetComponentInChildren<SkillPhase>();
    }

    public override void Enter() {
        base.Enter();

        touchedGround = false;
        player.Move(player.facingDir * player.jumpAttackSpeed.x, player.jumpAttackSpeed.y);
    }

    public override void Update() {
        base.Update();

        if (player.groundDetected && !touchedGround) {
            touchedGround = true;
            anim.SetTrigger("JumpAttackTrigger"); 
            player.Move(0, rb.linearVelocity.y);
        }

        if (triggerCalled && player.groundDetected) {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit() {
        base.Exit(); 
    }
}
 