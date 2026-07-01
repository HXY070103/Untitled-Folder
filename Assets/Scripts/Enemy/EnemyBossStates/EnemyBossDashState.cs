using UnityEngine;

public class EnemyBossDashState : EnemyState {
    private EnemyBoss enemyBoss;
    private Transform player;
    private int dashDir;

    public EnemyBossDashState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Untargetable");
        enemyBoss.vfx.DoImageEchoEffect(float.PositiveInfinity);

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        dashDir = enemyBoss.facingDir;
    }

    public override void Update() {
        base.Update();

        float xVelocity = dashDir * enemyBoss.dashSpeed;
        enemyBoss.Move(xVelocity, rb.linearVelocity.y);

        if (enemyBoss.OutOfMoveRange() || ReachedPlayer()) {
            stateMachine.ChangeState(enemyBoss.attackState);
        }
    }

    public override void Exit() {
        base.Exit();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Enemy");
        enemyBoss.vfx.StopImageEchoEffect();
        enemyBoss.Move(0, 0);
    }

    private bool ReachedPlayer() {
        return DistanceToPlayer() < enemyBoss.attackDistance;
    }

    private float DistanceToPlayer() {
        if (player == null) {
            return float.MaxValue;
        }

        return Mathf.Abs(player.position.x - enemyBoss.transform.position.x);
    }
}
