using UnityEngine;

public class EnemyBossEnergyWaveState : EnemyState {
    private EnemyBoss enemyBoss;

    public EnemyBossEnergyWaveState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.MoveRaw(0, 0);
        enemyBoss.SetEnergyWavePerformed(false);
        anim.SetBool("SpellCastPerformed", false);
    }

    public override void Update() {
        base.Update();

        if (enemyBoss.EnergyWavePerformed) {
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
