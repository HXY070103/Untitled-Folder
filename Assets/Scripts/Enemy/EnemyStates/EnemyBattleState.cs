using UnityEngine;

public class EnemyBattleState : EnemyState {
    private Transform player;
    private float lastTimeWasInBattle;

    public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        UpdateBattleTimer();

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        if (ShouldRetreat()) {
            rb.linearVelocity = new Vector2(
                    -DirectionToPlayer() * enemy.retreatSpeed.x, 
                    enemy.retreatSpeed.y
                );
            enemy.HandleFlip(DirectionToPlayer());
        }
    }

    public override void Update() {
        base.Update();

        if (enemy.PlayerDetected()) {
            UpdateBattleTimer();
        }

        if (BattleTimeIsOver()) {
            stateMachine.ChangeState(enemy.idleState);
        } else {
            if (WithInAttackRange() && enemy.PlayerDetected()) {
                stateMachine.ChangeState(enemy.attackState);
            } else {
                enemy.SetVelocity(DirectionToPlayer() * enemy.battleMoveSpeed, rb.linearVelocity.y);
            }
        }
    }

    public override void Exit() {
        base.Exit();
    }

    private void UpdateBattleTimer() {
        lastTimeWasInBattle = Time.time;
    }

    private bool BattleTimeIsOver() {
        return Time.time > lastTimeWasInBattle + enemy.battleDuration;
    }

    private bool WithInAttackRange() {
        return DistanceToPlayer() < enemy.attackDistance;
    }

    private bool ShouldRetreat() {
        return DistanceToPlayer() < enemy.minRetreatDistance;
    }

    private float DistanceToPlayer() {
        if (player == null) {
            return float.MaxValue;
        }

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    private int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}