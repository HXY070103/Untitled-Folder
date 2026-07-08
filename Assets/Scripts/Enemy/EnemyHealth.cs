using System;
using UnityEngine;

public class EnemyHealth : EntityHealth {
    public static event Action<float> OnDamageTaken;
    public static event Action OnEnemyDeath;
    public static event Action<Enemy> OnEnemyKilled;

    private Enemy enemy;

    protected override void Awake() {
        base.Awake();

        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(Damage damage, Transform damageDealer) {
        base.TakeDamage(damage, damageDealer);

        if (isDead) {
            OnEnemyDeath?.Invoke();
            OnEnemyKilled?.Invoke(enemy);
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
}