using UnityEngine;

public class EnemyBossTeleportAwayState : EnemyState {
    private EnemyBoss enemyBoss;
    private Transform player;

    public EnemyBossTeleportAwayState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Untargetable");

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        enemyBoss.SetVelocity(0, rb.linearVelocity.y);
        enemyBoss.SetTeleportPerformed(false);
        enemyBoss.StartTeleportAway();
    }

    public override void Update() {
        base.Update();

        if (enemyBoss.TeleportPerformed) {
            enemyBoss.HandleFlip(DirectionToPlayer());
            anim.SetBool("TeleportPerformed", true);
        }

        if (triggerCalled) {
            stateMachine.ChangeState(enemyBoss.spellCastState);
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
