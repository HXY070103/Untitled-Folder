using System;
using UnityEngine;

public class EnemyHealth : EntityHealth {
    public static event Action<float> OnDamageTaken;
    public static event Action OnEnemyDeath;

    private Enemy enemy;

    private bool isFirstAttack = false;
    private float damageMultiplier;

    protected override void Awake() {
        base.Awake();

        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(Damage damage, Transform damageDealer) {
        if (isFirstAttack) {
            damage = new Damage(damage.value * damageMultiplier, damage.isCrit);
            isFirstAttack = false;
        }

        base.TakeDamage(damage, damageDealer);

        if (isDead) {
            OnEnemyDeath?.Invoke();
        }

        float finalDamage = CalculateFinalDamage(damage);
        OnDamageTaken?.Invoke(finalDamage);

        if (damageDealer.GetComponent<Player>() != null) {
            enemy.TryToEnterBattleState(damageDealer);
        }
    }

    private float CalculateFinalDamage(Damage damage) {
        float mitigation = stats.GetArmorMitigation();
        return damage.value * (1 - mitigation);
    }

    private void SetFirstAttack(float damageMultiplier) {
        isFirstAttack = true;
        this.damageMultiplier = damageMultiplier;
    }

    private void ResetFirstAttack() {
        isFirstAttack = false;
    }

    private void OnEnable() {
        SkillTimeControl.OnSetFirstAttack += SetFirstAttack;
        SkillTimeControl.OnResetFirstAttack += ResetFirstAttack;
    }

    private void OnDisable() {
        SkillTimeControl.OnSetFirstAttack -= SetFirstAttack;
        SkillTimeControl.OnResetFirstAttack -= ResetFirstAttack;
    }
}