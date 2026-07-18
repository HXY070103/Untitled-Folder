using UnityEngine;

public class EnemyPhaseTransitionState : EnemyState {
    private readonly IEnemyPhaseTransition phaseTransitionEnemy;

    private bool transitionCompleted;

    public EnemyPhaseTransitionState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        phaseTransitionEnemy = enemy as IEnemyPhaseTransition;

        if (phaseTransitionEnemy == null) {
            throw new System.ArgumentException($"{enemy.name} must implement IEnemyPhaseTransition to use EnemyPhaseTransitionState.");
        }
    }

    public override void Enter() {
        base.Enter();

        stateTimer = Mathf.Max(0, phaseTransitionEnemy.GetPhaseTransitionDuration());
        transitionCompleted = false;
        phaseTransitionEnemy.BeginPhaseTransition();
    }

    public override void Update() {
        base.Update();

        if (transitionCompleted || stateTimer > 0) {
            return;
        }

        transitionCompleted = true;
        phaseTransitionEnemy.CompletePhaseTransition();

        EntityState nextState = phaseTransitionEnemy.GetPhaseTransitionNextState();

        if (nextState == null) {
            Debug.LogError($"{enemy.name} phase transition requires a next state.", enemy);
            return;
        }

        stateMachine.ChangeState(nextState);
    }

    public override void Exit() {
        phaseTransitionEnemy.EndPhaseTransition();
        base.Exit();
    }
}
