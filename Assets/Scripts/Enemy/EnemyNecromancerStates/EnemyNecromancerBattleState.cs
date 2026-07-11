using UnityEngine;

public class EnemyNecromancerBattleState : EnemyBattleState {
    private EnemyNecromancer enemyNecromancer;

    private float lastTimeSpellCast;

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
        if (enemyNecromancer.ForcedPhaseActive) {
            enemyNecromancer.Move(0, 0);
            return;
        }
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    private void UpdateSpellCastTimer() {
        lastTimeSpellCast = tc.time;
    }

    private bool CanUseSpellCast() {
        return tc.time > lastTimeSpellCast + enemyNecromancer.spellCastCooldown;
    }
}
