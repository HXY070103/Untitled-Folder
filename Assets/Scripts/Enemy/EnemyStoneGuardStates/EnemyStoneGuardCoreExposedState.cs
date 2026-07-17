public class EnemyStoneGuardCoreExposedState : EnemyState {
    private readonly EnemyStoneGuard stoneGuard;

    public EnemyStoneGuardCoreExposedState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        stateTimer = stoneGuard.coreExposedDuration;
        stoneGuard.MoveRaw(0, rb.linearVelocity.y);
        stoneGuard.ExitAttackWindow();
        stoneGuard.vfx.SetAttackAlert(false);
        stoneGuard.health.AddDamageTakenMultiplier(stoneGuard.coreDamageTakenMultiplier, EnemyStoneGuard.CoreDamageSource);
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
            stateMachine.ChangeState(stoneGuard.coreRecoveryState);
        }
    }

    public override void Exit() {
        stoneGuard.health.RemoveDamageTakenMultiplier(EnemyStoneGuard.CoreDamageSource);

        base.Exit();
    }
}
