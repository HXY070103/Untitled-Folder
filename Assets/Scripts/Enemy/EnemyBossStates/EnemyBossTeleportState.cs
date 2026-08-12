using UnityEngine;

public class EnemyBossTeleportState : EnemyState {
    private EnemyBoss enemyBoss;

    public EnemyBossTeleportState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Untargetable");

        enemyBoss.MoveRaw(0, 0);
        stateTimer = enemyBoss.teleportDuration;
        anim.SetBool("TeleportPerformed", false);
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
            anim.SetBool("TeleportPerformed", true);
        }

        if (triggerCalled) {
            stateMachine.ChangeState(enemyBoss.teleportNextState);
        }
    }

    public override void Exit() {
        base.Exit();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}
