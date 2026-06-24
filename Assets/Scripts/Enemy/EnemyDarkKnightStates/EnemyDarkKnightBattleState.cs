using UnityEngine;

public class EnemyDarkKnightBattleState : EnemyBattleState {
    private EnemyDarkKnight enemyDarkKnight;

    private bool reachDeadEnd;
    private int deadEndDir;

    private int basicAttackCounter;
    private bool isBasicAttack;

    private int dashAttackCounter;
    private bool isDashAttack;

    private float originAttackCooldown;

    public EnemyDarkKnightBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyDarkKnight = enemy as EnemyDarkKnight;

        reachDeadEnd = false;

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
        if (!enemyDarkKnight.groundDetected || enemyDarkKnight.wallDetected) {
            reachDeadEnd = true;
            deadEndDir = enemyDarkKnight.facingDir;
        }

        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (WithInAttackRange() && enemyDarkKnight.PlayerDetected() && CanAttack()) {
            if (isBasicAttack) {
                enemyDarkKnight.attackCooldown = originAttackCooldown;
                UpdateAttackTimer();
                UpdateBasicAttackCounter();
                stateMachine.ChangeState(enemyDarkKnight.attackState);
                return;
            }

            if (isDashAttack) {
                enemyDarkKnight.attackCooldown = enemyDarkKnight.dashAttackCooldown;
                UpdateAttackTimer();
                UpdateDashAttackCounter();
                stateMachine.ChangeState(enemyDarkKnight.dashAttackState);
                return;
            }
        } else {
            if (CanWalkAway() && InWalkAwayRange()) {
                float xVeloity = -DirectionToPlayer() * enemyDarkKnight.BattleMoveSpeed;
                enemyDarkKnight.SetVelocity(xVeloity, rb.linearVelocity.y);
            } else {
                enemyDarkKnight.SetVelocity(0, rb.linearVelocity.y);

                if (!enemyDarkKnight.PlayerDetected()) {
                    enemy.HandleFlip(DirectionToPlayer());
                }
            }
        }
    }

    private bool CanWalkAway() {
        return !reachDeadEnd || -DirectionToPlayer() != deadEndDir;
    }

    private bool InWalkAwayRange() {
        return DistanceToPlayer() < enemyDarkKnight.attackDistance * 0.8f;
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
