public class EnemyStoneGuardStunnedState : EnemyStunnedState {
    private EnemyStoneGuard enemyStoneGuard;

    public EnemyStoneGuardStunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyStoneGuard = enemy as EnemyStoneGuard;
    }

    public override void Enter() {
        base.Enter();

        enemyStoneGuard.health.AddDamageTakenMultiplier(
                enemyStoneGuard.coreDamageTakenMultiplier, 
                EnemyStoneGuard.CoreDamageSource
            );
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();

        enemyStoneGuard.health.RemoveDamageTakenMultiplier(
                EnemyStoneGuard.CoreDamageSource
            );
    }
}
