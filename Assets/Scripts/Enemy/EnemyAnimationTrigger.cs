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
        enemy.EnterAttackWindow();
        enemyVFX.SetAttackAlert(true);
    }

    private void DisableCounterWindow() {
        enemy.ExitAttackWindow();
        enemyVFX.SetAttackAlert(false);
    }
}
