public interface IEnemyPhaseTransition {
    public float GetPhaseTransitionDuration();
    public EntityState GetPhaseTransitionNextState();
    public void BeginPhaseTransition();
    public void CompletePhaseTransition();
    public void EndPhaseTransition();
}
