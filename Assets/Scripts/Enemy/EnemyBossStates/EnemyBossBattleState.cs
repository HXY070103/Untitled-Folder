using UnityEngine;

public class EnemyBossBattleState : EnemyBattleState {
    private EnemyBoss enemyBoss;

    private float time;

    public EnemyBossBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.HandleFlip(DirectionToPlayer());

        if (CanUseSpecialAttack()) {
            UpdateSpecialAttackTimer();
            EnemyState nextSpecialAttackState = 
                enemyBoss.specialAttackStates[Random.Range(0, enemyBoss.specialAttackStates.Count)];
            stateMachine.ChangeState(nextSpecialAttackState);
        }
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (WithInAttackRange() && enemy.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            stateMachine.ChangeState(enemyBoss.attackState); 
        } else {
            if (!enemy.PlayerDetected()) {
                enemy.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemyBoss.OutOfMoveRange()) {
                return;
            }

            if (WithInDashRange()) {
                stateMachine.ChangeState(enemyBoss.dashState);
                return;
            }

            float xVelocity = DirectionToPlayer() * enemy.BattleMoveSpeed;
            enemy.SetVelocity(xVelocity, rb.linearVelocity.y);
        }
    }

    private bool WithInDashRange() {
        return DistanceToPlayer() > enemyBoss.minDashDistance;
    }

    private void UpdateSpecialAttackTimer() {
        enemyBoss.specialAttackTimer = enemyBoss.specialAttackCooldown;
    }

    private bool CanUseSpecialAttack() {
        return enemyBoss.specialAttackTimer < 0;
    }
}
