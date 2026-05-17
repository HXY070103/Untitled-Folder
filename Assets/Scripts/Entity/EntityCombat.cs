using UnityEngine;

public class EntityCombat : MonoBehaviour {
    private EntityVFX entityVFX;
    private EntityStats entityStats;

    [Header("Damage info")]
    public float damage = 10;

    [Header("Target detection info")]
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private Transform targetCheck;

    private void Awake() {
        entityVFX = GetComponent<EntityVFX>();
        entityStats = GetComponent<EntityStats>();
    }

    public void PerformAttack() {
        foreach (var target in GetDetectedColliders()) {
            IDamageable damageable = target.GetComponent<IDamageable>();
            
            if (damageable == null) {
                continue;
            }

            float damage = entityStats.GetDamage(out bool isCrit);
            damageable.TakeDamage(damage, transform);

            if (entityVFX != null) {
                entityVFX.CreateOnHitVFX(target.transform, isCrit);
            }
        }
    } 

    protected Collider2D[] GetDetectedColliders() {
        return 
            Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
