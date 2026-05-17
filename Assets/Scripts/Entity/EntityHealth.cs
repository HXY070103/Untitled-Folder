using UnityEngine.UI;
using UnityEngine;

public class EntityHealth : MonoBehaviour, IDamageable {
    private Slider healthBar;
    private EntityVFX entityVFX;
    private Entity entity;
    private EntityStats entityStats;

    [Header("Health info")]
    [SerializeField] protected bool isDead = false;
    [SerializeField] private float currentHP;

    [Header("On damage knockback info")]
    [SerializeField] private Vector2 onDamageKnockback = new Vector2(1.5f, 2.5f);
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("On heavy damage knockback info")]
    [Range(0, 1)]
    [SerializeField] private float heavyDamageThreshold = 0.3f;
    [SerializeField] private Vector2 onHeavyDamageKnockback = new Vector2(7, 7);
    [SerializeField] private float heavyKnockbackDuration = 0.5f;
    

    protected virtual void Awake() {
        healthBar = GetComponentInChildren<Slider>();
        entityVFX = GetComponent<EntityVFX>();
        entity = GetComponent<Entity>();
        entityStats = GetComponent<EntityStats>();

        currentHP = entityStats.GetMaxHP();
        UpdateHealthBar();
    }

    public virtual void TakeDamage(float damage, Transform damageDealer) {
        if (isDead) {
            return;
        }

        float mitigation = entityStats.GetArmorMitigation();
        float finalDamage = damage * (1 - mitigation);

        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);

        if (entity != null) {
            entity.ReceiveKnockback(knockback, duration);
        }

        if (entityVFX != null) {
            entityVFX.PlayOnDamageVFX();
        }

        ReduceHP(finalDamage);
    }

    protected void ReduceHP(float damage) {
        currentHP -= damage;
        UpdateHealthBar();
        
        if (currentHP <= 0) {
            Die();
        }
    }

    private void Die() {
        isDead = true;
        entity.EntityDeath();
    }

    private void UpdateHealthBar() {
        if (healthBar == null) {
            return;
        }

        healthBar.value = currentHP / entityStats.GetMaxHP();
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer) {
        int knockbackDir = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? onHeavyDamageKnockback : onDamageKnockback;

        return new Vector2(knockbackDir * knockback.x, knockback.y);
    }

    private float CalculateDuration(float damage) {
        return IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    }

    private bool IsHeavyDamage(float damage) {
        float healthPercentageTaken = damage / entityStats.GetMaxHP();
        return healthPercentageTaken > heavyDamageThreshold;
    }
}
