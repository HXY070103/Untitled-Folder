using UnityEngine;

public class EnemyNecromancerSpellCastState : EnemyState {
private EnemyNecromancer enemyNecromancer;

    public EnemyNecromancerSpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyNecromancer = enemy as EnemyNecromancer;
    }

    public override void Enter() {
        base.Enter();

        enemyNecromancer.SetVelocity(0, rb.linearVelocity.y);
        enemyNecromancer.SetSpellCastPerformed(false);
    }

    public override void Update() {
        base.Update();

        if (enemyNecromancer.SpellCastPerformed) {
            anim.SetBool("SpellCastPerformed", true);
        }

        if (triggerCalled) {
            stateMachine.ChangeState(enemyNecromancer.battleState);
        }
    }

    public override void Exit() {
        base.Exit();

        anim.SetBool("SpellCastPerformed", false);
    }
}
