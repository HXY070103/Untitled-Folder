using UnityEngine;

public class EnemyMageBattleState : EnemyBattleState {
    private EnemyMage enemyMage;

    private float lastTimeUsedRetreat;

    public EnemyMageBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyMage = enemy as EnemyMage;

        lastTimeUsedRetreat = float.NegativeInfinity;
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
        lastTimeUsedRetreat = Time.time;
    }

    private bool CanUseRetreat() {
        return Time.time > lastTimeUsedRetreat + enemyMage.retreatCooldown;
    }
}
