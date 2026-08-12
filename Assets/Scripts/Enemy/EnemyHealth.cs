using System;
using UnityEngine;

public class EnemyHealth : EntityHealth {
    public static event Action<float, Transform> OnDamageTaken;
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
            return;
        }

        if (damageDealer.GetComponent<Player>() != null) {
            enemy.TryToEnterBattleState(damageDealer);
        }
    }

    public override void ReduceHP(float reducedHP) {
        bool wasDead = isDead;
        base.ReduceHP(reducedHP);

        if (!wasDead && isDead) {
            OnEnemyDeath?.Invoke();
            OnEnemyKilled?.Invoke(enemy);
        }
    }

    protected override float CalculateReducedHP(Damage damage, Transform damageDealer) {
        float reducedHP = base.CalculateReducedHP(damage, damageDealer);
        OnDamageTaken?.Invoke(reducedHP, damageDealer);
        return reducedHP;
    }
}
