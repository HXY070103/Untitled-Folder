using UnityEngine;

public class StateMachine {
    public EntityState currentState { get; private set; }
    private bool isActive = true;

    public void Initialize(EntityState startState) {
        currentState = startState;
        currentState.Enter();
        isActive = true;
    } 

    public void ChangeState(EntityState newState) {
        if (!isActive) {
            return;
        }

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateActiveState() {
        if (!isActive) {
            return;
        }

        currentState.Update();
    }

    public void SwitchOn() {
        isActive = true;
    }

    public void SwitchOff() {
        isActive = false;
    }
}
