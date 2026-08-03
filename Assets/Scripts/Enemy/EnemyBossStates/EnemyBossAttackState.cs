using UnityEngine;

public class EnemyBossAttackState : EnemyAttackState {
    private EnemyBoss enemyBoss;

    private int comboIndex = 1;

    public EnemyBossAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();

        UpdateComboIndex();
    }

    protected override void HandleStateExit() {
        if (ComboQueued()) {
            anim.SetBool("Attack", false);
            enemyBoss.EnterAttackStateWithDelay();
        } else {
            stateMachine.ChangeState(enemyBoss.battleState);
        }
    }

    private bool ComboQueued() {
        return comboIndex < enemyBoss.comboLimit[enemyBoss.phaseIndex];
    }

    private void UpdateComboIndex() {
        comboIndex += 1;

        if (comboIndex > enemyBoss.comboLimit[enemyBoss.phaseIndex]) {
            comboIndex = 1;
        }
    }
}
