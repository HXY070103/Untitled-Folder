using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;

public class EntityHealth : MonoBehaviour, IDamageable {
    public event Action OnHealthUpdate;

    private Slider healthBar;
    private EntityVFX vfx;
    private Entity entity;
    protected EntityStats stats;

    [Header("Health info")]
    [SerializeField] protected bool isDead = false;
    [SerializeField] private float currentHP;
    [SerializeField] private float initialMaxHP = 100f;
    [SerializeField] private bool isExecutable = false;
    private readonly Dictionary<string, float> damageTakenMultipliers = new Dictionary<string, float>();
    private readonly HashSet<string> damageImmunitySources = new HashSet<string>();

    [Header("On damage knockback info")]
    [SerializeField] private Vector2 onDamageKnockback = new Vector2(1.5f, 2.5f);
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("On heavy damage knockback info")]
    [Range(0, 1)]
    [SerializeField] private float heavyDamageThreshold = 0.3f;
    [SerializeField] private Vector2 onHeavyDamageKnockback = new Vector2(7, 7);
    [SerializeField] private float heavyKnockbackDuration = 0.5f;

    public bool miniHealthBarActive;

    public bool hasRevived { get; private set; } = false;
    private bool canRevival = false;
    private float reviveHPMultiplier;
    
    protected virtual void Awake() {
        healthBar = GetComponentInChildren<Slider>();
        vfx = GetComponent<EntityVFX>();
        entity = GetComponent<Entity>();
        stats = GetComponent<EntityStats>();

        currentHP = stats.GetMaxHP();
        OnHealthUpdate += UpdateHealthBar;
        UpdateHealthBar();
    }

    public void TryToExecute(float HPThreshold, float bonusDamage) {
        if (!isExecutable) {
            ReduceHP(bonusDamage);
            return;
        }

        if (GetHPPercentage() <= HPThreshold) {
            ReduceHP(currentHP);
        }
    }
 
    public virtual void TakeDamage(Damage damage, Transform damageDealer) {
        if (isDead || IsDamageImmune()) {
            return;
        }

        float reducedHP = CalculateReducedHP(damage, damageDealer);
        ReduceHP(reducedHP);

        Vector2 knockback = CalculateKnockback(reducedHP, damageDealer);
        float duration = CalculateDuration(reducedHP);
        entity.ReceiveKnockback(knockback, duration);

        vfx.PlayOnDamageVFX();
    }

    public void AddDamageTakenMultiplier(float multiplier, string source) {
        if (string.IsNullOrEmpty(source)) {
            Debug.LogWarning("Damage taken multiplier requires a source.");
            return;
        }

        damageTakenMultipliers[source] = Mathf.Max(0, multiplier);
    }

    public void RemoveDamageTakenMultiplier(string source) {
        if (string.IsNullOrEmpty(source)) {
            return;
        }

        damageTakenMultipliers.Remove(source);
    }

    public float GetDamageTakenMultiplier() {
        float finalMultiplier = 1;

        foreach (float multiplier in damageTakenMultipliers.Values) {
            finalMultiplier *= multiplier;
        }

        return finalMultiplier;
    }

    public void AddDamageImmunity(string source) {
        if (string.IsNullOrEmpty(source)) {
            Debug.LogWarning("Damage immunity requires a source.");
            return;
        }

        damageImmunitySources.Add(source);
    }

    public void RemoveDamageImmunity(string source) {
        if (string.IsNullOrEmpty(source)) {
            return;
        }

        damageImmunitySources.Remove(source);
    }

    public bool IsDamageImmune() {
        return damageImmunitySources.Count > 0;
    }

    protected virtual float CalculateReducedHP(Damage damage, Transform damageDealer) {
        float mitigation = stats.GetArmorMitigation();
        float finalDamage = damage.value * (1 - mitigation);
        return finalDamage * GetDamageTakenMultiplier();
    }

    private Vector2 CalculateKnockback(float reducedHP, Transform damageDealer) {
        int knockbackDir = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(reducedHP) ? onHeavyDamageKnockback : onDamageKnockback;

        return new Vector2(knockbackDir * knockback.x, knockback.y);
    }

    private float CalculateDuration(float reducedHP) {
        return IsHeavyDamage(reducedHP) ? heavyKnockbackDuration : knockbackDuration;
    }

    private bool IsHeavyDamage(float reducedHP) {
        float healthPercentageTaken = reducedHP / stats.GetMaxHP();
        return healthPercentageTaken > heavyDamageThreshold;
    }

    public float Healing(float healingHP) {
        if (isDead) {
            return 0;
        }

        float overflowingHP = 0;
        currentHP += healingHP;

        if (currentHP > stats.GetMaxHP()) {
            overflowingHP = currentHP - stats.GetMaxHP();
            currentHP = stats.GetMaxHP();
        }
    
        OnHealthUpdate?.Invoke();

        return overflowingHP;
    }

    public virtual void ReduceHP(float reducedHP) {
        if (isDead || IsDamageImmune()) {
            return;
        }

        currentHP -= reducedHP;

        if (currentHP <= 0.5) {
            currentHP = 0;
            Die();
        }
        
        OnHealthUpdate?.Invoke();
    }

    private void Die() {
        if (canRevival) {
            Revival();
            return;
        }

        isDead = true;
        entity.EntityDeath();
    }

    public void StartDeathPrevention(float reviveHPMultiplier) {
        hasRevived = false;
        canRevival = true;
        this.reviveHPMultiplier = reviveHPMultiplier;
    }

    public void StopDeathPrevention() {
        canRevival = false;
    }

    private void Revival() {
        currentHP = stats.GetMaxHP() * reviveHPMultiplier;
        hasRevived = true;
        canRevival = false;
        OnHealthUpdate?.Invoke();
    }

    public void IncreaseMaxHP(float bonusMaxHP, string source) {
        stats.maxHP.AddModifier(bonusMaxHP, source);
        currentHP += bonusMaxHP;
        if (currentHP > stats.GetMaxHP()) {
            currentHP = stats.GetMaxHP();
        }
        OnHealthUpdate?.Invoke();
    }

    public void DecreaseMaxHP(float reducedMaxHP, string source) {
        stats.maxHP.AddModifier(-reducedMaxHP, source);
        if (currentHP > stats.GetMaxHP()) {
            currentHP = stats.GetMaxHP();
        }
        if (currentHP <= 0) {
            currentHP = 0;
        }
        OnHealthUpdate?.Invoke();
    }

    private void UpdateHealthBar() {
        if (healthBar == null || !healthBar.gameObject.activeSelf) {
            return;
        }

        healthBar.value = currentHP / stats.GetMaxHP();
    }

    public void EnableHealthBar(bool enable) {
        healthBar?.gameObject.SetActive(enable);
    }

    public float GetCurrentHP() {
        return currentHP;
    }

    public float GetHPPercentage() {
        return currentHP / stats.GetMaxHP();
    }

    public float GetInitialMaxHP() {
        return initialMaxHP;
    }

    public void SetCurrentHP(float HP) {
        currentHP = Mathf.Clamp(HP, 0, stats.GetMaxHP());
        isDead = currentHP <= 0;
        OnHealthUpdate?.Invoke();
    }
}
