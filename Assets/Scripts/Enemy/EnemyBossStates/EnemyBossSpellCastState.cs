using UnityEngine;

public class EnemyBossSpellCastState : EnemyState {
    private EnemyBoss enemyBoss;

    public EnemyBossSpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.Move(0, rb.linearVelocity.y);
        enemyBoss.SetEnergyWavePerformed(false);
    }

    public override void Update() {
        base.Update();

        if (enemyBoss.EnergyWavePerformed) {
            anim.SetBool("SpellCastPerformed", true);
        }

        if (triggerCalled) {
            enemyBoss.CompleteEnergyWaveAction();
            stateMachine.ChangeState(enemyBoss.battleState);
        }
    }

    public override void Exit() {
        enemyBoss.ClearAttackPresentation();
        base.Exit();

        anim.SetBool("SpellCastPerformed", false);
    }
}
