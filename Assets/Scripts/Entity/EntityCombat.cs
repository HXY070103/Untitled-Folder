using UnityEngine;

public class EntityCombat : MonoBehaviour {
    private EntityVFX vfx;
    private EntitySFX sfx;
    private EntityStats stats;

    [Header("Target detection info")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float targetCheckRadius = 1;
    public LayerMask whatIsTarget;

    private void Awake() {
        vfx = GetComponent<EntityVFX>();
        sfx = GetComponent<EntitySFX>();
        stats = GetComponent<EntityStats>();
    }

    public virtual void PerformAttack() {
        bool attackHit = false;
        foreach (var target in GetDetectedColliders(whatIsTarget)) {
            PerformAttackOnTarget(target.transform);
            attackHit = true;
        }
        if (!attackHit)
        {
            sfx?.PlayAttackMiss();
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
        sfx?.PlayAttackHit();
    }

    public Collider2D[] GetDetectedColliders(LayerMask whatToDetect) {
        return 
            Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatToDetect);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
