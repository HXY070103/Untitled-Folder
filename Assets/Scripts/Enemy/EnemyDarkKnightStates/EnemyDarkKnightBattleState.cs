using UnityEngine;

public class EnemyDarkKnightBattleState : EnemyBattleState {
    private EnemyDarkKnight enemyDarkKnight;

    private int basicAttackCounter;
    private bool isBasicAttack;

    private int dashAttackCounter;
    private bool isDashAttack;

    private float originAttackCooldown;

    public EnemyDarkKnightBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyDarkKnight = enemy as EnemyDarkKnight;

        basicAttackCounter = 0;
        isBasicAttack = true;

        dashAttackCounter = 0;
        isDashAttack = false;

        originAttackCooldown = enemyDarkKnight.attackCooldown;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (enemyDarkKnight.ArenaAttackInProgress) {
            enemyDarkKnight.Move(0, rb.linearVelocity.y);
            return;
        }

        if (WithInAttackRange() && enemyDarkKnight.PlayerDetected() && CanAttack()) {
            if (isBasicAttack) {
                ApplyBasicAttack();
                return;
            }

            if (isDashAttack) {
                ApplyDashAttack();
                return;
            }
        } else {
            if (!enemyDarkKnight.PlayerDetected()) {
                enemyDarkKnight.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemyDarkKnight.wallDetected) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemyDarkKnight.battleMoveSpeed;
            enemyDarkKnight.Move(xVelocity, rb.linearVelocity.y);
        }
    }

    private void ApplyBasicAttack() {
        enemyDarkKnight.attackCooldown = originAttackCooldown;
        UpdateAttackTimer();
        UpdateBasicAttackCounter();
        stateMachine.ChangeState(enemyDarkKnight.preAttackState);
    }

    private void ApplyDashAttack() {
        enemyDarkKnight.attackCooldown = enemyDarkKnight.dashAttackCooldown;
        UpdateAttackTimer();
        UpdateDashAttackCounter();
        stateMachine.ChangeState(enemyDarkKnight.dashAttackState);
    }

    private void UpdateBasicAttackCounter() {
        basicAttackCounter += 1;

        if (basicAttackCounter == enemyDarkKnight.numOfBasicAttack) {
            basicAttackCounter = 0;
            isBasicAttack = false;
            isDashAttack = true;
        }
    }

    private void UpdateDashAttackCounter() {
        dashAttackCounter += 1;

        if (dashAttackCounter == enemyDarkKnight.numOfDashAttack) {
            dashAttackCounter = 0;
            isDashAttack = false;
            isBasicAttack = true;
        }
    }
}
