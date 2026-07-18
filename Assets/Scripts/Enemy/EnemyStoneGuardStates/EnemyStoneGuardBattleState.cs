public class EnemyStoneGuardBattleState : EnemyBattleState {
    private readonly EnemyStoneGuard stoneGuard;

    public EnemyStoneGuardBattleState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        base.Enter();

        stoneGuard.SetAttack(stoneGuard.attackWindup, stoneGuard.attackCooldown, stoneGuard.attackState);
        UpdateAttackTimer();
    }

    protected override void HandleBattle() {
        enemy.MoveRaw(0, rb.linearVelocity.y);

        if (player == null) {
            return;
        }

        enemy.HandleFlip(DirectionToPlayer());

        if (stoneGuard.ShouldUseShield()) {
            stateMachine.ChangeState(stoneGuard.shieldState);
            return;
        }

        if (!CanAttack()) {
            return;
        }

        UpdateAttackTimer();

        if (WithInAttackRange()) {
            stateMachine.ChangeState(enemy.attackState);
        } else {
            stateMachine.ChangeState(stoneGuard.GetNextRangedAttackState());
        }
    }
}
