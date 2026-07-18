public interface IEnemyPhaseTransition {
    float GetPhaseTransitionDuration();
    EntityState GetPhaseTransitionNextState();
    void BeginPhaseTransition();
    void CompletePhaseTransition();
    void EndPhaseTransition();
}
