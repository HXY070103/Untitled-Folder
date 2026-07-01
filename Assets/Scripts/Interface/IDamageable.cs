using UnityEngine;

public interface IDamageable {
    public void TakeDamage(Damage damage, Transform damageDealer);
    public void TryApplyInstantKill();
}
