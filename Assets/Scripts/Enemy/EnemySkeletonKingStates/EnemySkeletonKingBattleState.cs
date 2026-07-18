using UnityEngine;

public class EnemySkeletonKingBattleState : EnemyBattleState {
    private EnemySkeletonKing enemySkeletonKing;

    private int attackCounter = 0;
    private bool hasSummoned = false;

    public EnemySkeletonKingBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemySkeletonKing = enemy as EnemySkeletonKing;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        if (!hasSummoned && IsLowHealth()) {
            hasSummoned = true;
            stateMachine.ChangeState(enemySkeletonKing.summonState);
        }

        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (WithInAttackRange() && enemySkeletonKing.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            UpdateAttackCounter();

            if (IsShockwave()) {
                ApplyShockwave();
            } else {
                ApplyBasicAttack();
            }
        } else {
            if (!enemySkeletonKing.PlayerDetected()) {
                enemySkeletonKing.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemySkeletonKing.wallDetected || hasSummoned) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemySkeletonKing.battleMoveSpeed;
            enemySkeletonKing.Move(xVelocity, rb.linearVelocity.y);
        }
    }

    private void ApplyShockwave() {
        enemySkeletonKing.SetAttack(
                enemySkeletonKing.shockwaveWindup, 
                enemySkeletonKing.shockwaveCooldown, 
                enemySkeletonKing.shockwaveState
            );
        stateMachine.ChangeState(enemy.preAttackState); 
    }

    private void UpdateAttackCounter() {
        attackCounter += 1;

        if (attackCounter >= 3) {
            attackCounter = 0;
        }
    }

    private bool IsShockwave() {
        return attackCounter == enemySkeletonKing.shockwaveIndex;
    }

    private bool IsLowHealth() {
        return enemy.health.GetHPPercentage() < enemySkeletonKing.HPThreshold;
    }
}