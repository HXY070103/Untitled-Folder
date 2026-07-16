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

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        attackDir = player.moveInput.x != 0 ? (int) player.moveInput.x : player.facingDir;
    }

    public override void Update() {
        base.Update();

        CancelAttackIfNeeded();

        float xVelocity = attackDir * phase.heavyAttackSpeed;
        player.Move(xVelocity, 0);

        AddTarget();

        if (triggerCalled) {
            HandleStateExit();
        }
    }

    public override void Exit() {
        base.Exit();

        rb.gravityScale = originalGravityScale;
        player.Move(0, 0);

        vfx.StopImageEchoEffect();
        EndHeavyAttack();
    }

    private void HandleStateExit() {
        if (player.groundDetected) {
            stateMachine.ChangeState(player.idleState);
        } else {
            stateMachine.ChangeState(player.fallState);
        }
    }

    private void CancelAttackIfNeeded() {
        if (player.wallDetected) {
            if (player.groundDetected) {
                stateMachine.ChangeState(player.idleState);
            } else {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
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
            player.combat.SetCanExecute(true);
        }

        player.stats.damage.AddMultiplier(phase.GetDamageMultiplier(), "HeavyAttack");
    }

    private void EndHeavyAttack() {
        PerformAttack();
        vfx.DoGlobalPause();

        if (phase.Unlocked("Shadow Assassination")) {
            player.combat.SetCanExecute(false);
        }

        player.stats.damage.RemoveMultiplier("HeavyAttack");
        player.UpdateHeavyAttack();
    }

    private void PerformAttack() {
        bool chainPhantasmUnlocked = phase.Unlocked("Chain Phantasm");
        if (chainPhantasmUnlocked) {
            phase.EnableBonusPhase();
        }

        foreach (Transform target in targets) {
            player.combat.PerformAttackOnTarget(target);
        }

        if (chainPhantasmUnlocked) {
            phase.DisableBonusPhase();
            phase.TryToEnterBonusPhase();
        }

        targets.Clear();
    }
}
