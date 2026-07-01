using UnityEngine;

public class EnemyBossTeleportBehindState : EnemyState {
    private EnemyBoss enemyBoss;
    private Transform player;

    public EnemyBossTeleportBehindState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Untargetable");

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        enemyBoss.Move(0, rb.linearVelocity.y);
        enemyBoss.SetTeleportPerformed(false);
    }

    public override void Update() {
        base.Update();

        if (enemyBoss.TeleportPerformed) {
            enemyBoss.HandleFlip(DirectionToPlayer());
            anim.SetBool("TeleportPerformed", true);
        }

        if (triggerCalled) {
            stateMachine.ChangeState(enemyBoss.attackState);
        }
    }

    public override void Exit() {
        base.Exit();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Enemy");

        anim.SetBool("TeleportPerformed", false);
    }

    private int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
