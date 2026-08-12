using UnityEngine;

public class EnemyPreAttackState : EnemyState {
    private Transform player;

    public EnemyPreAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {}

    public override void Enter() {
        base.Enter();

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        enemy.HandleFlip(DirectionToPlayer());
        stateTimer = enemy.currentWindup;
        //enemy.vfx.PlayPreAttackVFX();
    }

    public override void Update() {
        base.Update();

        if (stateTimer < 0) {
            stateMachine.ChangeState(enemy.currentAttack);
        }
    }

    public override void Exit() {
        base.Exit();
    }

    protected int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
