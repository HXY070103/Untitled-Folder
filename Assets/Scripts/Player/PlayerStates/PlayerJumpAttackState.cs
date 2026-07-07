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
            StartHandleBonusPhase();
        }

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

        if (player.IsHeavyAttack) {
            ResetDamage();
            player.UpdateHeavyAttack();
            EndHandleBonusPhase();
        }
    }

    private void StartHandleBonusPhase() {
        if (phase.Unlocked("Shadow Execution")) {
            phase.EnableBonusPhase();
        }
    }

    private void EndHandleBonusPhase() {
        if (phase.Unlocked("Shadow Execution")) {
            phase.DisableBonusPhase();
            phase.TryToEnterBonusPhase();
        }
    }

    private void HandleInstantKill() {
        if (phase.Unlocked("Shadow Assassination")) {
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
 