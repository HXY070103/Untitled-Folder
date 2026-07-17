using UnityEngine;

public class EnemyStoneGuardAttackState : EnemyAttackState {
    private readonly EnemyStoneGuard stoneGuard;

    private float elapsedTime;
    private bool counterWindowOpened;
    private bool attackPerformed;

    public EnemyStoneGuardAttackState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        elapsedTime = 0;
        counterWindowOpened = false;
        attackPerformed = false;

        stoneGuard.MoveRaw(0, rb.linearVelocity.y);
        CloseCounterWindow();
    }

    public override void Update() {
        base.Update();

        elapsedTime += Time.deltaTime * tc.localTimeScale;

        if (!counterWindowOpened && elapsedTime >= stoneGuard.meleeCounterWindowStart) {
            counterWindowOpened = true;
            stoneGuard.EnterAttackWindow();
            stoneGuard.vfx.SetAttackAlert(true);
        }

        if (!attackPerformed && elapsedTime >= stoneGuard.meleeHitTime) {
            attackPerformed = true;
            CloseCounterWindow();
            stoneGuard.combat.PerformAttack();
        }

        if (elapsedTime >= stoneGuard.meleeDuration) {
            stateMachine.ChangeState(stoneGuard.battleState);
        }
    }

    public override void Exit() {
        CloseCounterWindow();

        base.Exit();
    }

    private void CloseCounterWindow() {
        stoneGuard.ExitAttackWindow();
        stoneGuard.vfx.SetAttackAlert(false);
    }
}
