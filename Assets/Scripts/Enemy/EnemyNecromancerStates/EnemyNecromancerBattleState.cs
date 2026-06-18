using UnityEngine;

public class EnemyNecromancerBattleState : EnemyBattleState {
    private EnemyNecromancer enemyNecromancer;

    private float lastTimeUsedSpellCast;

    public EnemyNecromancerBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyNecromancer = enemy as EnemyNecromancer;

        lastTimeUsedSpellCast = float.NegativeInfinity;
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
        lastTimeUsedSpellCast = Time.time;
    }

    private bool CanUseSpellCast() {
        return Time.time > lastTimeUsedSpellCast + enemyNecromancer.spellCastAttackCooldown;
    }
}
