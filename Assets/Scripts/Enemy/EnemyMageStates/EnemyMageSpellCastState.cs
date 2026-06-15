using UnityEngine;

public class EnemyMageSpellCastState : EnemyState {
    private EnemyMage enemyMage;

    public EnemyMageSpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyMage = enemy as EnemyMage;
    }

    public override void Enter() {
        base.Enter();

        enemyMage.SetVelocity(0, rb.linearVelocity.y);
        enemyMage.SetSpellCastPerformed(false);
    }

    public override void Update() {
        base.Update();

        if (enemyMage.SpellCastPerformed) {
            anim.SetBool("SpellCastPerformed", true);
        }

        if (triggerCalled) {
            stateMachine.ChangeState(enemyMage.battleState);
        }
    }

    public override void Exit() {
        base.Exit();

        anim.SetBool("SpellCastPerformed", false);
    }
}
