using UnityEngine;

public class EntityCombat : MonoBehaviour {
    private EntityVFX vfx;
    private EntitySFX sfx;
    private EntityStats stats;

    [Header("Target detection info")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    public LayerMask whatIsTarget;

    protected Transform TargetCheck => targetCheck;

    protected virtual void Awake() {
        vfx = GetComponent<EntityVFX>();
        sfx = GetComponent<EntitySFX>();
        stats = GetComponent<EntityStats>();
    }

    public virtual void PerformAttack() {
        PerformAttack(1f);
    }

    public virtual void PerformAttack(float damageMultiplier) {
        bool attackHit = false;
        foreach (var target in GetDetectedColliders(whatIsTarget)) {
            PerformAttackOnTarget(target.transform, damageMultiplier);
            attackHit = true;
        }

        if (!attackHit) {
            sfx.PlayAttackMiss();
        }
    } 

    public virtual void PerformAttackOnTarget(Transform target) {
        PerformAttackOnTarget(target, 1f);
    }

    public virtual void PerformAttackOnTarget(Transform target, float damageMultiplier) {
        IDamageable damageable = target.GetComponent<IDamageable>();

        if (damageable == null) {
            return;
        }

        Damage damage = stats.GetDamage(damageMultiplier);
        damageable.TakeDamage(damage, transform);
        vfx.CreateOnHitVFX(target.transform, damage.isCrit);
        sfx.PlayAttackHit();
    }

    public Collider2D[] GetDetectedColliders(LayerMask whatToDetect) {
        return GetDetectedColliders(whatToDetect, targetCheckRadius);
    }

    public Collider2D[] GetDetectedColliders(
        LayerMask whatToDetect,
        float detectionRadius
    ) {
        return Physics2D.OverlapCircleAll(
            targetCheck.position,
            Mathf.Max(0, detectionRadius),
            whatToDetect
        );
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
