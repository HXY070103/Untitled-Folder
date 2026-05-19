using System;
using UnityEngine;

public class EnemyHealth : EntityHealth {
    public static event Action<float> OnDamageTaken;

    private Enemy enemy;

    protected override void Awake() {
        base.Awake();

        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(float damage, Transform damageDealer) {
        base.TakeDamage(damage, damageDealer);

        float mitigation = entityStats.GetArmorMitigation();
        float finalDamage = damage * (1 - mitigation);

        if (OnDamageTaken != null) {
            OnDamageTaken.Invoke(finalDamage);
        }

        if (damageDealer.GetComponent<Player>() != null) {
            enemy.TryToEnterBattleState(damageDealer);
        }
    }
}