public class EnemyStoneGuardBattleState : EnemyBattleState {
    private EnemyStoneGuard enemyStoneGuard;

    private int rangeAttackIndex = 1;
    private float lastTimeUsedShield = float.NegativeInfinity;

    public EnemyStoneGuardBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyStoneGuard = enemy as EnemyStoneGuard;
    }

    public override void Enter() {
        base.Enter();

        if (enemyStoneGuard.isPhaseTwo && CanUseShield() && enemyStoneGuard.ReachedShieldStayDuration()) { 
            stateMachine.ChangeState(enemyStoneGuard.shieldState);
            UpdateUseShieldTimer();
        }
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    private bool CanUseShield() {
        return tc.time > lastTimeUsedShield + enemyStoneGuard.shieldCooldown;
    }

    private void UpdateUseShieldTimer() {
        lastTimeUsedShield = tc.time;
    }

    protected override void HandleBattle() {
        if (WithInAttackRange() && enemyStoneGuard.PlayerDetected() && CanAttack()) {
            Attack();
            UpdateAttackTimer();
        } else {
            if (!enemyStoneGuard.PlayerDetected()) {
                enemyStoneGuard.HandleFlip(DirectionToPlayer());
            }
        }
    }

    private bool IsRangeAttack() {
        return DistanceToPlayer() > enemyStoneGuard.rangeAttackDistance;
    }

    protected override bool WithInAttackRange() {
        if (IsRangeAttack()) {
            return WithInRangeAttackRange();
        } else {
            return DistanceToPlayer() < enemyStoneGuard.attackDistance;
        }
    }

    private bool WithInRangeAttackRange() {
        switch (rangeAttackIndex) {
            case 1: 
                return DistanceToPlayer() < enemyStoneGuard.shootDistance;
            case 2: 
                return DistanceToPlayer() < enemyStoneGuard.laserDistance;
            default: 
                return false;
        }
    }

    private void Attack() {
        if (IsRangeAttack()) {
            ApplyRangeAttack();
        } else {
            ApplyBasicAttack();
        }
    }

    private void ApplyRangeAttack() {
        if (enemyStoneGuard.isPhaseTwo) {
            ApplyShoot();
        } else {
            HandleRangeAttackLoop();
            UpdateRangeAttackCounter();
        }
    }

    private void HandleRangeAttackLoop() {
        switch (rangeAttackIndex) {
            case 1: 
                ApplyShoot();
                break;
            case 2: 
                ApplyLaser();
                break;
        }
    }

    private void ApplyShoot() {
        enemyStoneGuard.SetAttack(
                enemyStoneGuard.shootWindup, 
                enemyStoneGuard.shootCooldown, 
                enemyStoneGuard.shootState
            );
        stateMachine.ChangeState(enemyStoneGuard.preAttackState);
    }

    private void ApplyLaser() {
        enemyStoneGuard.SetAttack(
                enemyStoneGuard.laserWindup, 
                enemyStoneGuard.laserCooldown, 
                enemyStoneGuard.laserState
            );
        stateMachine.ChangeState(enemyStoneGuard.preAttackState);
    }

    private void UpdateRangeAttackCounter() {
        rangeAttackIndex += 1;

        if (rangeAttackIndex > enemyStoneGuard.numOfRangeAttack) {
            rangeAttackIndex = 1;
        }
    }
}
