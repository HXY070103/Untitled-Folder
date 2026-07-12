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
            return;
        }

        if (damageDealer.GetComponent<Player>() != null) {
            enemy.TryToEnterBattleState(damageDealer);
        }
    }

    protected override float CalculateReducedHP(Damage damage) {
        float reducedHP = base.CalculateReducedHP(damage);
        OnDamageTaken?.Invoke(reducedHP);
        return reducedHP;
    }
}