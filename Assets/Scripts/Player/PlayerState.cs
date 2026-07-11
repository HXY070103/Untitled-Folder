using UnityEngine;

public abstract class PlayerState : EntityState{
    protected Player player;
    protected PlayerInputSet input;
    protected PlayerVFX vfx;
    protected PlayerSkillManager skillManager;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) 
        : base(player, stateMachine, animBoolName) {
        this.player = player;

        input = player.input;
        vfx = player.vfx;
        skillManager = player.skillManager;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() { 
        base.Update();

        if (input.Player.Dash.WasPressedThisFrame() && CanDash()) {
            stateMachine.ChangeState(player.dashState);
        }

        if (input.Player.Skill.WasPressedThisFrame()) {
            skillManager.HandleSkill();
        }
    }

    public override void Exit() {
        base.Exit();
    }

    public override void UpdateAnimationPatameters() {
        base.UpdateAnimationPatameters();

        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetFloat("AttackAnimSpeedMultiplier", player.stats.GetAttackSpeed());
        Debug.Log(player.stats.GetAttackSpeed());
    }

    private bool CanDash() {
        if (player.wallDetected) {
            return false;
        }

        if (stateMachine.currentState == player.dashState) {
            return false;
        }

        if (player.InDashCooldown()) {
            return false;
        }

        return true;
    }
}