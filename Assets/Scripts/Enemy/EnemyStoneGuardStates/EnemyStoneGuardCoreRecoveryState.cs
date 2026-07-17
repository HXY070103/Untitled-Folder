public class EnemyStoneGuardCoreRecoveryState : EnemyState {
    private readonly EnemyStoneGuard stoneGuard;

    public EnemyStoneGuardCoreRecoveryState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        stateTimer = stoneGuard.coreRecoveryDuration;
        stoneGuard.MoveRaw(0, rb.linearVelocity.y);
        stoneGuard.ExitAttackWindow();
        stoneGuard.vfx.SetAttackAlert(false);
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
            stateMachine.ChangeState(stoneGuard.battleState);
        }
    }
}
