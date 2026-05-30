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

        if (player.IsHeavyAttack) {
            phase.InterruptSkill();
            IncreaseDamage();
            HandleInstantKill();
        }

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

        if (player.IsHeavyAttack) {
            ResetDamage();
            player.ResetNextAttack();
        }
    }

    private void HandleInstantKill() {
        if (phase.UnLocked("Shadow Assassination")) {
            player.combat.SetNextAttackToInstantKill();
        }
    }

    private void IncreaseDamage() {
        float baseDamage = player.stats.damage.GetValue();
        float bonusDamage = baseDamage * (phase.GetDamageMultiplier() - 1);
        player.stats.damage.AddModifier(bonusDamage, "HeavyAttack");
    }

    private void ResetDamage() {
        player.stats.damage.RemoveModifier("HeavyAttack");
    }
}
 