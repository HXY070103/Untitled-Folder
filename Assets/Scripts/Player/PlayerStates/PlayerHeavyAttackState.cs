using UnityEngine;

public class PlayerHeavyAttackState : PlayerState {
    private SkillPhase phase;
    private float attackMovementTimer;
    private int attackDir;

    public PlayerHeavyAttackState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {
        phase = player.GetComponentInChildren<SkillPhase>();
    }

    public override void Enter() {
        base.Enter();

        phase.InterruptSkill();
        IncreaseDamage();
        HandleInstantKill();
        StartHandleBonusPhase();

        attackDir = player.moveInput.x != 0 ? (int) player.moveInput.x : player.facingDir;
    }

    public override void Update() {
        base.Update();

        HandleAttackMovement();

        if (triggerCalled) {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit() {
        base.Exit();

        ResetDamage();
        player.UpdateHeavyAttack();
        EndHandleBonusPhase();
    }

    private void StartHandleBonusPhase() {
        if (phase.UnLocked("Shadow Execution")) {
            phase.EnableBonusPhase();
        }
    }

    private void EndHandleBonusPhase() {
        if (phase.UnLocked("Shadow Execution")) {
            phase.DisableBonusPhase();
            phase.TryToEnterBonusPhase();
        }
    }

    private void HandleInstantKill() {
        if (phase.UnLocked("Shadow Assassination")) {
            player.combat.SetNextAttackToInstantKill();
        }
    }

    private void IncreaseDamage() {
        float baseDamage = player.stats.GetFinalDamage();
        float bonusDamage = baseDamage * (phase.GetDamageMultiplier() - 1);
        player.stats.damage.AddModifier(bonusDamage, "HeavyAttack");
    }

    private void ResetDamage() {
        player.stats.damage.RemoveModifier("HeavyAttack");
    }

    public void ApplyAttackMovement() {
        attackMovementTimer = player.attackMovementDuration;
        player.Move(attackDir * player.heavyAttackSpeed.x, player.heavyAttackSpeed.y);
    }

    private void HandleAttackMovement() {
        attackMovementTimer -= Time.deltaTime;

        if (attackMovementTimer < 0) {
            player.Move(0, rb.linearVelocity.y);
        }
    }
}
