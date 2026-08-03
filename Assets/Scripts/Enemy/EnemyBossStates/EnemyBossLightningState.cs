using UnityEngine;

public class EnemyBossLightningState : EnemyState {
    private readonly EnemyBoss enemyBoss;

    public EnemyBossLightningState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.MoveRaw(0, 0);
        enemyBoss.SetLightningPerformed(false);
        anim.SetBool("SpellCastPerformed", false);
    }

    public override void Update() {
        base.Update();

        if (enemyBoss.LightningPerformed) {
            anim.SetBool("SpellCastPerformed", true);
        }

        if (triggerCalled) {
            stateMachine.ChangeState(enemyBoss.battleState);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
