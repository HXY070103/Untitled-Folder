using UnityEngine.UI;
using UnityEngine;
using System;

public class EntityHealth : MonoBehaviour, IDamageable {

    public event Action OnHealthUpdate;
    private Slider healthBar;
    private EntityVFX entityVFX;
    private Entity entity;
    protected EntityStats entityStats;

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

    public bool miniHealthBarActive;
    

    protected virtual void Awake() {
        healthBar = GetComponentInChildren<Slider>();
        entityVFX = GetComponent<EntityVFX>();
        entity = GetComponent<Entity>();
        entityStats = GetComponent<EntityStats>();

        currentHP = entityStats.GetMaxHP();
        OnHealthUpdate += UpdateHealthBar;
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

    public void Healing(float healingHP) {
        currentHP += healingHP;

        if (currentHP > entityStats.GetMaxHP()) {
            currentHP = entityStats.GetMaxHP();
        }
        OnHealthUpdate?.Invoke();
    }

    private void ReduceHP(float damage) {
        currentHP -= damage;
        OnHealthUpdate?.Invoke();
        if (currentHP <= 0) {
            Die();
        }
        
    }

    private void Die() {
        isDead = true;
        entity.EntityDeath();
    }

    private void UpdateHealthBar() {
        if (healthBar == null && healthBar.gameObject.activeSelf) {
            return;
        }

        healthBar.value = currentHP / entityStats.GetMaxHP();
    }

    public void EnableHealthBar(bool enable)
    {
        healthBar?.gameObject.SetActive(enable);
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

    public float GetCurrentHP() {
        return currentHP;
    }

    public float GetHPPercentage() {
        return currentHP / entityStats.GetMaxHP();
    }
}
