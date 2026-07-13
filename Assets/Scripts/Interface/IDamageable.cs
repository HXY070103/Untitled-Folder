using UnityEngine;

public interface IDamageable {
    public void TakeDamage(Damage damage, Transform damageDealer);
    public void TryToExecute(float HPThreshold, float HPPercentageDamage);
}
