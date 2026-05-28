using System;
using UnityEngine;

public class EnemyHealth : EntityHealth {
    public static event Action<float> OnDamageTaken;

    private Enemy enemy;

    protected override void Awake() {
        base.Awake();

        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(Damage damage, Transform damageDealer) {
        base.TakeDamage(damage, damageDealer);

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