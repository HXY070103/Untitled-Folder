using UnityEngine;

public class EntityCombat : MonoBehaviour {
    private EntityVFX vfx;
    private EntityStats stats;

    [Header("Damage info")]
    public float damage = 10;

    [Header("Target detection info")]
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private Transform targetCheck;

    private void Awake() {
        vfx = GetComponent<EntityVFX>();
        stats = GetComponent<EntityStats>();
    }

    public void PerformAttack() {
        foreach (var target in GetDetectedColliders()) {
            IDamageable damageable = target.GetComponent<IDamageable>();
            
            if (damageable == null) {
                continue;
            }

            Damage damage = stats.GetDamage();
            damageable.TakeDamage(damage, transform);

            if (!damageable.IsInvulnerable()) {
                vfx.CreateOnHitVFX(target.transform, damage.isCrit);
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
