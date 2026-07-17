public class EnemyStoneGuardIdleState : EnemyIdleState {
    public EnemyStoneGuardIdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
    }

    public override void Enter() {
        base.Enter();

        stateTimer = float.PositiveInfinity;
    }
}
