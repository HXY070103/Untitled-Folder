public class EnemyStoneGuardPhaseTransitionState : EnemyState {
    private readonly EnemyStoneGuard stoneGuard;

    public EnemyStoneGuardPhaseTransitionState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        stateTimer = stoneGuard.phaseTransitionDuration;
        stoneGuard.MoveRaw(0, rb.linearVelocity.y);
        stoneGuard.BeginPhaseTransition();
    }

    public override void Update() {
        base.Update();

        if (stateTimer <= 0) {
            stoneGuard.CompletePhaseTransition();
            stateMachine.ChangeState(stoneGuard.battleState);
        }
    }

    public override void Exit() {
        stoneGuard.EndPhaseTransitionImmunity();
        base.Exit();
    }
}
