using UnityEngine;

public class EntityCombat : MonoBehaviour {
    private EntityVFX vfx;
    private EntityStats stats;

    [Header("Target detection info")]
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] protected LayerMask whatIsTarget;
    [SerializeField] private Transform targetCheck;

    private void Awake() {
        vfx = GetComponent<EntityVFX>();
        stats = GetComponent<EntityStats>();
    }

    public virtual void PerformAttack() {
        foreach (var target in GetDetectedColliders(whatIsTarget)) {
            PerformAttackOnTarget(target.transform);
        }
    } 

    public virtual void PerformAttackOnTarget(Transform target) {
        IDamageable damageable = target.GetComponent<IDamageable>();

        if (damageable == null) {
            return;
        }

        Damage damage = stats.GetDamage();
        damageable.TakeDamage(damage, transform);
        vfx.CreateOnHitVFX(target.transform, damage.isCrit);
    }

    protected Collider2D[] GetDetectedColliders(LayerMask whatToDetect) {
        return 
            Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatToDetect);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
