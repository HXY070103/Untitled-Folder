using UnityEngine;

public class EnemyDarkKnightAttackState : EnemyAttackState {
    private EnemyDarkKnight enemyDarkKnight;

    private int comboIndex = 1;

    public EnemyDarkKnightAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyDarkKnight = enemy as EnemyDarkKnight;
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
            enemyDarkKnight.EnterAttackStateWithDelay();
        } else {
            stateMachine.ChangeState(enemyDarkKnight.battleState);
        }
    }

    private bool ComboQueued() {
        return comboIndex < enemyDarkKnight.comboLimit[enemyDarkKnight.phaseIndex];
    }

    private void UpdateComboIndex() {
        comboIndex += 1;

        if (comboIndex > enemyDarkKnight.comboLimit[enemyDarkKnight.phaseIndex]) {
            comboIndex = 1;
        }
    }
}
