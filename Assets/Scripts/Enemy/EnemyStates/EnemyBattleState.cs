using UnityEngine;

public class EnemyBattleState : EnemyState {
    private Transform player;
    private float lastTimeWasInBattle;
    private float lastTimeAttacked;

    public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        lastTimeWasInBattle = float.NegativeInfinity;
        lastTimeAttacked = float.NegativeInfinity;
    }

    public override void Enter() {
        base.Enter();

        UpdateBattleTimer();

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        if (ShouldRetreat()) {
            float xVelocity = -DirectionToPlayer() * enemy.retreatSpeed.x;
            rb.linearVelocity = new Vector2(xVelocity, enemy.retreatSpeed.y);
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
        }
        
        HandleBattle();
    }

    public override void Exit() {
        base.Exit();
    }

    protected void UpdateBattleTimer() {
        lastTimeWasInBattle = Time.time;
    }

    protected void UpdateAttackTimer() {
        lastTimeAttacked = Time.time;
    }

    protected virtual void HandleBattle() {
        if (WithInAttackRange() && enemy.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            stateMachine.ChangeState(enemy.attackState);            
        } else {
            if (!enemy.PlayerDetected()) {
                enemy.HandleFlip(DirectionToPlayer());
            }

            if (enemy.canChasePlayer) {
                float xVelocity = DirectionToPlayer() * enemy.GetBattleMoveSpeed();
                enemy.SetVelocity(xVelocity, rb.linearVelocity.y);
            }
        }
    }
    
    protected bool CanAttack() {
        return Time.time > lastTimeAttacked + enemy.attackCooldown;
    }

    protected bool BattleTimeIsOver() {
        return Time.time > lastTimeWasInBattle + enemy.battleDuration;
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