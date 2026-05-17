using UnityEngine;

public class EnemyStunnedState : EnemyState {
    private EnemyVFX enemyVFX;

    public EnemyStunnedState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyVFX = enemy.GetComponent<EnemyVFX>();
    }

    public override void Enter() {
        base.Enter();

        enemy.SetCounterWindow(false);
        enemyVFX.SetAttackAlert(false);

        stateTimer = enemy.stunnedDuration;
        rb.linearVelocity = 
            new Vector2(-enemy.facingDir * enemy.stunnedSpeed.x, enemy.stunnedSpeed.y);
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
