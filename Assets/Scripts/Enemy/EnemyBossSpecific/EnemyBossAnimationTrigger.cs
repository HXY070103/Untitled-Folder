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

    private void BossAttackTrigger() {
        enemyBoss.PerformCurrentComboAttack();
    }

    public override void EnableCounterWindow() {
        if (enemyBoss.IsBidirectionalHarvestCounterWindowActive()) {
            base.EnableCounterWindow();
            return;
        }

        if (!enemyBoss.IsComboAttackActive()) {
            enemyBoss.ExitAttackWindow();
            enemyBoss.vfx.SetAttackAlert(false);
            return;
        }

        base.EnableCounterWindow();
    }

    public override void DisableCounterWindow() {
        if (enemyBoss.IsBidirectionalHarvestCounterWindowActive()) {
            return;
        }

        base.DisableCounterWindow();
    }
}
