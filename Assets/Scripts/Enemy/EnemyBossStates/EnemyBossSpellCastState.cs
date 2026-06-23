using UnityEngine;

public class EnemyBossSpellCastState : EnemyState {
    private EnemyBoss enemyBoss;

    public EnemyBossSpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.SetVelocity(0, rb.linearVelocity.y);
        enemyBoss.SetSpellCastPerformed(false);
    }

    public override void Update() {
        base.Update();

        if (enemyBoss.SpellCastPerformed) {
            anim.SetBool("SpellCastPerformed", true);
        }

        if (triggerCalled) {
            stateMachine.ChangeState(enemyBoss.battleState);
        }
    }

    public override void Exit() {
        base.Exit();

        anim.SetBool("SpellCastPerformed", false);
    }
}
