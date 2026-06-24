using UnityEngine;

public class EnemyNecromancerBattleState : EnemyBattleState {
    private EnemyNecromancer enemyNecromancer;

    public EnemyNecromancerBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyNecromancer = enemy as EnemyNecromancer;
    }

    public override void Enter() {
        base.Enter();

        if (CanUseSpellCast()) {
            UpdateSpellCastTimer();
            stateMachine.ChangeState(enemyNecromancer.spellCastState);
        }
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    private void UpdateSpellCastTimer() {
        enemyNecromancer.spellCastTimer = enemyNecromancer.spellCastAttackCooldown;
    }

    private bool CanUseSpellCast() {
        return enemyNecromancer.spellCastTimer < 0;
    }
}
