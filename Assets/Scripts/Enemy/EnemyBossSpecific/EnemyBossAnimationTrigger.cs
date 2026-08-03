using UnityEngine;

public class EnemyBossAnimationTrigger : EnemyAnimationTrigger {
    private EnemyBoss enemyBoss;

    protected override void Awake() {
        base.Awake();

        enemyBoss = GetComponentInParent<EnemyBoss>();
    }

    private void Teleport() {
        enemyBoss.Teleport();
    }
}
