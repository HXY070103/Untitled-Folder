using UnityEngine;

public class EnemyBattleState : EnemyState {
    protected Transform player;
    private float battleTimer;
    private float attackTimer;

    public EnemyBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        
    }

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

        battleTimer -= Time.deltaTime * enemy.LocalTimeScale;
        attackTimer -= Time.deltaTime * enemy.LocalTimeScale;

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
        battleTimer = enemy.battleDuration;
    }

    protected bool BattleTimeIsOver() {
        return battleTimer < 0;
    }

    protected void UpdateAttackTimer() {
        attackTimer = enemy.attackCooldown;
    }

    protected bool CanAttack() {
        return attackTimer < 0;
    }

    protected void QuickRetreat() {
        float xVelocity = -DirectionToPlayer() * enemy.QuickRetreatSpeed.x;
        float yVelocity = enemy.QuickRetreatSpeed.y;
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        enemy.HandleFlip(DirectionToPlayer());
    }

    protected virtual void HandleBattle() {
        if (WithInAttackRange() && enemy.PlayerDetected() && CanAttack()) {
            UpdateAttackTimer();
            stateMachine.ChangeState(enemy.attackState); 
        } else {
            if (!enemy.PlayerDetected()) {
                enemy.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemy.wallDetected) {
                return;
            }

            float xVelocity = DirectionToPlayer() * enemy.BattleMoveSpeed;
            enemy.SetVelocity(xVelocity, rb.linearVelocity.y);
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