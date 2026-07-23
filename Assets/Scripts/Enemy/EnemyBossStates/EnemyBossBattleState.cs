using UnityEngine;

public class EnemyBossBattleState : EnemyBattleState {
    private EnemyBoss enemyBoss;

    private float lastTimeSpecialAttack;

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
        if (WithInAttackRange() && enemyBoss.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            stateMachine.ChangeState(enemyBoss.preAttackState); 
        } else {
            if (!enemyBoss.PlayerDetected()) {
                enemyBoss.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemyBoss.OutOfMoveRange()) {
                return;
            }

            if (WithInDashRange()) {
                stateMachine.ChangeState(enemyBoss.dashState);
                return;
            }

            float xVelocity = DirectionToPlayer() * enemyBoss.battleMoveSpeed;
            enemyBoss.Move(xVelocity, rb.linearVelocity.y);
            enemyBoss.sfx?.TryPlayMove(0.2f);
        }
    }

    private bool WithInDashRange() {
        return DistanceToPlayer() > enemyBoss.minDashDistance;
    }

    private void UpdateSpecialAttackTimer() {
        lastTimeSpecialAttack = tc.time;
    }

    private bool CanUseSpecialAttack() {
        return tc.time > lastTimeSpecialAttack + enemyBoss.specialAttackCooldown;
    }
}
