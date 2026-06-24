using UnityEngine;

public class EnemyMageBattleState : EnemyBattleState {
    private EnemyMage enemyMage;

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
        enemyMage.retreatTimer = enemyMage.retreatCooldown;
    }

    private bool CanUseRetreat() {
        return enemyMage.retreatTimer < 0;
    }
}
