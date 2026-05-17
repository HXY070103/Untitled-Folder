using UnityEngine;

public class EnemyAnimationTrigger : EntityAnimationTrigger {
    private Enemy enemy;
    private EnemyVFX enemyVFX;

    protected override void Awake() {
        base.Awake();

        enemy = GetComponentInParent<Enemy>();
        enemyVFX = GetComponentInParent<EnemyVFX>();
    }

    private void EnableCounterWindow() {
        enemy.SetCounterWindow(true);
        enemyVFX.SetAttackAlert(true);
    }

    private void DisableCounterWindow() {
        enemy.SetCounterWindow(false);
        enemyVFX.SetAttackAlert(false);
    }
}
