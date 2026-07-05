using UnityEngine;

public interface ISlowable {
    public void SlowDown(float slowDownPercentage);
    public void RecoverSpeed();
    public void RecoverSpeedWithDelay(float delay);
}