using UnityEngine;

public class EntityCombat : MonoBehaviour {
    private EntityVFX vfx;
    private EntityStats stats;

    [Header("Damage info")]
    public float damage = 10;

    private bool canApplyInstantKill = false;

    [Header("Target detection info")]
    [SerializeField] private float targetCheckRadius = 1;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private Transform targetCheck;

    private void Awake() {
        vfx = GetComponent<EntityVFX>();
        stats = GetComponent<EntityStats>();
    }

    public void PerformAttack() {
        foreach (var target in GetDetectedColliders(whatIsTarget)) {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (canApplyInstantKill) {
                damageable.TryApplyInstantKill();
            }
            
            if (damageable == null) {
                continue;
            }

            Damage damage = stats.GetDamage();
            damageable.TakeDamage(damage, transform);

            if (!damageable.IsInvulnerable()) {
                vfx.CreateOnHitVFX(target.transform, damage.isCrit);
            }
        }

        canApplyInstantKill = false;
    } 

    public void PerforAttackOnTarget(Transform target) {
        IDamageable damageable = target.GetComponent<IDamageable>();

        if (canApplyInstantKill) {
            damageable.TryApplyInstantKill();
        }
            
        if (damageable == null) {
            return;
        }

        Damage damage = stats.GetDamage();
        damageable.TakeDamage(damage, transform);

        if (!damageable.IsInvulnerable()) {
            vfx.CreateOnHitVFX(target.transform, damage.isCrit);
        }
    }

    public void SetNextAttackToInstantKill() {
        canApplyInstantKill = true;
    }

    protected Collider2D[] GetDetectedColliders(LayerMask whatToDetect) {
        return 
            Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatToDetect);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
