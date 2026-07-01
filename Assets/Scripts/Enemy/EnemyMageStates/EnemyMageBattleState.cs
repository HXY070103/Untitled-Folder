using UnityEngine;

public class EnemyMageBattleState : EnemyBattleState {
    private EnemyMage enemyMage;

    private float lastTimeRetreat;

    public EnemyMageBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyMage = enemy as EnemyMage;
    }

    public override void Enter() {
        base.Enter();

        if (ShouldRetreat() && CanUseRetreat()) {
            UpdateRetreatTimer();
            stateMachine.ChangeState(enemyMage.retreatState);
        }
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    private void UpdateRetreatTimer() {
        lastTimeRetreat = tc.time;
    }

    private bool CanUseRetreat() {
        return tc.time > lastTimeRetreat + enemyMage.retreatCooldown;
    }
}
