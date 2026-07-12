using System.Collections.Generic;
using UnityEngine;

public class PlayerHeavyAttackState : PlayerState {
    private SkillPhase phase;

    private float originalGravityScale;
    private int attackDir;
    private HashSet<Transform> targets = new HashSet<Transform>();

    public PlayerHeavyAttackState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {
        phase = player.GetComponentInChildren<SkillPhase>();
    }

    public override void Enter() {
        base.Enter();

        vfx.DoImageEchoEffect(float.PositiveInfinity);
        phase.InterruptSkill();
        StartHeavyAttack();
        StartChainPhantasmReward();

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        attackDir = player.moveInput.x != 0 ? (int) player.moveInput.x : player.facingDir;
    }

    public override void Update() {
        base.Update();

        float xVelocity = attackDir * phase.heavyAttackSpeed;
        player.Move(xVelocity, 0);

        AddTarget();

        if (triggerCalled) {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit() {
        base.Exit();

        rb.gravityScale = originalGravityScale;
        player.Move(0, 0);

        vfx.StopImageEchoEffect();
        player.UpdateHeavyAttack();
        EndHeavyAttack();
        EndChainPhantasmReward();
    }

    private void AddTarget() {
        Collider2D[] colliders = player.combat.GetDetectedColliders(player.combat.whatIsTarget);

        foreach (Collider2D collider in colliders) {
            Transform target = collider.transform;
            targets.Add(target);
        }
    }

    private void StartHeavyAttack() {
        if (phase.Unlocked("Shadow Assassination")) {
            player.combat.SetNextAttackToInstantKill();
            return;
        }

        player.stats.damage.AddMultiplier(phase.GetDamageMultiplier(), "HeavyAttack");
    }

    private void EndHeavyAttack() {
        PerformAttack();
        player.stats.damage.RemoveMultiplier("HeavyAttack");
    }

    private void PerformAttack() {
        foreach (Transform target in targets) {
            player.combat.PerformAttackOnTarget(target);
        }

        targets.Clear();
    }

    private void StartChainPhantasmReward() {
        if (phase.Unlocked("Chain Phantasm")) {
            phase.EnableBonusPhase();
        }
    }

    private void EndChainPhantasmReward() {
        if (phase.Unlocked("Chain Phantasm")) {
            phase.DisableBonusPhase();
            phase.TryToEnterBonusPhase();
        }
    }
}
