using UnityEngine;

public interface IDamageable {
    public void TakeDamage(Damage damage, Transform damageDealer);
    public bool IsInvulnerable();
}
