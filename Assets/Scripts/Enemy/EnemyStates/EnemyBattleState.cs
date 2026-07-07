using UnityEngine;

public class EnemyBattleState : EnemyState {
    protected Transform player;

    private float lastTimeBattle;
    private float lastTimeAttack;

    public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        UpdateBattleTimer();

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        if (ShouldRetreat()) {
            QuickRetreat();
        }
    }

    public override void Update() {
        base.Update();

        if (enemy.PlayerDetected()) {
            UpdateBattleTimer();
        }

        if (BattleTimeIsOver()) {
            stateMachine.ChangeState(enemy.idleState);
        }

        HandleBattle();
    }

    public override void Exit() {
        base.Exit();
    }

    protected void UpdateBattleTimer() {
        lastTimeBattle = tc.time;
    }

    protected bool BattleTimeIsOver() {
        return tc.time > lastTimeBattle + enemy.battleDuration;
    }

    protected void UpdateAttackTimer() {
        lastTimeAttack = tc.time;
    }

    protected bool CanAttack() {
        return tc.time > lastTimeAttack + enemy.attackCooldown;
    }

    protected void QuickRetreat() {
        float xVelocity = -DirectionToPlayer() * enemy.quickRetreatSpeed.x;
        float yVelocity = enemy.quickRetreatSpeed.y;
        enemy.MoveRaw(xVelocity, yVelocity);
        enemy.HandleFlip(DirectionToPlayer());
    }

    protected virtual void HandleBattle() {
        if (WithInAttackRange() && enemy.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            stateMachine.ChangeState(enemy.preAttackState); 
        } else {
            if (!enemy.PlayerDetected()) {
                enemy.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemy.wallDetected) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemy.battleMoveSpeed;
            enemy.Move(xVelocity, rb.linearVelocity.y);
            enemy.sfx?.TryPlayMove(0.2f);
        }
    }

    protected bool WithInAttackRange() {
        return DistanceToPlayer() < enemy.attackDistance;
    }

    protected bool ShouldRetreat() {
        return DistanceToPlayer() < enemy.minRetreatDistance;
    }

    protected float DistanceToPlayer() {
        if (player == null) {
            return float.MaxValue;
        }

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    protected int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}