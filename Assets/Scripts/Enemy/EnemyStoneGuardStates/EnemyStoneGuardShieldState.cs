using UnityEngine;

public class EnemyStoneGuardShieldState : EnemyState {
    private EnemyStoneGuard enemyStoneGuard;

    private Transform player;

    public EnemyStoneGuardShieldState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyStoneGuard = enemy as EnemyStoneGuard;
    }

    public override void Enter() {
        base.Enter();

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        PushPlayerAway();
    }

    public override void Update() {
        base.Update();

        if (triggerCalled) {
            stateMachine.ChangeState(enemyStoneGuard.battleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }

    private void PushPlayerAway() {
        Vector2 pushVelocity = new Vector2(
                DirectionToPlayer() * enemyStoneGuard.shieldPushVelocity.x, 
                enemyStoneGuard.shieldPushVelocity.y
            );
        player.GetComponent<Player>().ReceiveKnockback(pushVelocity, enemyStoneGuard.shieldPushDuration);
    }

    private int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
