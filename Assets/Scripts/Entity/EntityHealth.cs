using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

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
    [SerializeField] private bool isOnShotKillable = false;

    private bool isInvulnerable = false;

    [Header("On damage knockback info")]
    [SerializeField] private Vector2 onDamageKnockback = new Vector2(1.5f, 2.5f);
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("On heavy damage knockback info")]
    [Range(0, 1)]
    [SerializeField] private float heavyDamageThreshold = 0.3f;
    [SerializeField] private Vector2 onHeavyDamageKnockback = new Vector2(7, 7);
    [SerializeField] private float heavyKnockbackDuration = 0.5f;

    public bool miniHealthBarActive;

    private Coroutine dotCo;
    private bool isInDot = false;

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

    public void TryApplyInstantKill() {
        if (isOnShotKillable) {
            currentHP = 0;
            OnHealthUpdate?.Invoke();
        }
    }

    public void EnterInvulnerableState() {
        isInvulnerable = true;
    }

    public void ExitInvulnerableState() {
        isInvulnerable = false;
    }

    public bool IsInvulnerable() {
        return isInvulnerable;
    }

    public void StartDeathPrevention(float reviveHPMultiplier) {
        canRevival = true;
        this.reviveHPMultiplier = reviveHPMultiplier;
    }

    public void EndDeathPrevention() {
        canRevival = false;
    }

    public void ApplyDot(float damage, float dotInterval) {
        if (dotCo != null) {
            StopCoroutine(dotCo);
        }

        isInDot = true;
        dotCo = StartCoroutine(DotCo(damage, dotInterval));
    }

    public void EndDot() {
        isInDot = false;
    }

    private IEnumerator DotCo(float damage, float dotInterval) {
        while (isInDot) {
            ReduceHP(damage);
            vfx.PlayOnDamageVFX();
            yield return new WaitForSeconds(dotInterval);
        }
    }
 
    public virtual void TakeDamage(Damage damage, Transform damageDealer) {
        if (isDead || isInvulnerable) {
            return;
        }

        float finalDamage = CalculateFinalDamage(damage);
        ReduceHP(finalDamage);

        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);
        entity.ReceiveKnockback(knockback, duration);

        vfx.PlayOnDamageVFX();
    }

    private float CalculateFinalDamage(Damage damage) {
        float mitigation = stats.GetArmorMitigation();
        return damage.value * (1 - mitigation);
    }

    public void IncreaseMaxHP(float bonusMaxHP, string source) {
        stats.maxHP.AddModifier(bonusMaxHP, source);
        currentHP += bonusMaxHP;
        if (currentHP > stats.GetMaxHP()) {
            currentHP = stats.GetMaxHP();
        }
        OnHealthUpdate?.Invoke();
    }

    public void DecreaseMaxHP(float reduceMaxHP, string source) {
        stats.maxHP.AddModifier(-reduceMaxHP, source);
        if (currentHP > stats.GetMaxHP()) {
            currentHP = stats.GetMaxHP();
        }
        if (currentHP <= 0) {
            currentHP = 0;
        }
        OnHealthUpdate?.Invoke();
    }

    public void Healing(float healingHP) {
        currentHP += healingHP;

        if (currentHP > stats.GetMaxHP()) {
            currentHP = stats.GetMaxHP();
        }
        OnHealthUpdate?.Invoke();
    }

    protected void ReduceHP(float damage) {
        currentHP -= damage;
        OnHealthUpdate?.Invoke();
        if (currentHP <= 0) {
            Die();
        }
        
    }

    private void Die() {
        if (canRevival) {
            currentHP = stats.GetMaxHP() * reviveHPMultiplier;
            OnHealthUpdate?.Invoke();
            canRevival = false;
            return;
        }

        isDead = true;
        entity.EntityDeath();
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

    private Vector2 CalculateKnockback(float damage, Transform damageDealer) {
        int knockbackDir = transform.position.x > damageDealer.position.x ? 1 : -1;
        Vector2 knockback = IsHeavyDamage(damage) ? onHeavyDamageKnockback : onDamageKnockback;

        return new Vector2(knockbackDir * knockback.x, knockback.y);
    }

    private float CalculateDuration(float damage) {
        return IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    }

    private bool IsHeavyDamage(float damage) {
        float healthPercentageTaken = damage / stats.GetMaxHP();
        return healthPercentageTaken > heavyDamageThreshold;
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

    public void SetCurrentHP(float HP)
    {
        currentHP = Mathf.Clamp(HP, 0, stats.GetMaxHP());
        isDead = currentHP <= 0;
        OnHealthUpdate?.Invoke();
    }
}
