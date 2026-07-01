using UnityEngine;

public class PlayerCounterAttackState : PlayerState {
    private PlayerCombat combat;
    private bool hasPerformedCounter;

    public PlayerCounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {
        combat = player.GetComponent<PlayerCombat>();
    }

    public override void Enter() {
        base.Enter();

        hasPerformedCounter = combat.CounterAttackPerformed();
        anim.SetBool("CounterAttackPerformed", hasPerformedCounter);
        stateTimer = combat.GetCounterRecoveryDuration();
    }

    public override void Update() {
        base.Update();

        player.Move(0, rb.linearVelocity.y);
            
        if (triggerCalled) {
            stateMachine.ChangeState(player.idleState);
        }

        if (stateTimer < 0 && !hasPerformedCounter) {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
 