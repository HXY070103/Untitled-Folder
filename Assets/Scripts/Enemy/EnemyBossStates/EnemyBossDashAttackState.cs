using UnityEngine;

public class EnemyBossDashAttackState : EnemyState {
    private EnemyBoss enemyBoss;
    private Transform player;

    private Vector3 startPosition;
    private bool dashPerformed;

    private float lastTimePlayerHit;
    private bool playerHit;
    private bool dashDamageApplied;

    private int dashAttackIndex = 1;

    public EnemyBossDashAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyBoss = enemy as EnemyBoss;
    }

    public override void Enter() {
        base.Enter();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Untargetable");

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        startPosition = enemyBoss.transform.position;
        dashPerformed = false;

        lastTimePlayerHit = float.NegativeInfinity;
        playerHit = false;
        dashDamageApplied = false;

        enemyBoss.vfx.DoImageEchoEffect(float.PositiveInfinity);
        anim.SetBool("DashAttackPerformed", false);
    }

    public override void Update() {
        base.Update();

        if (triggerCalled) {
            HandleStateExit();
        }

        if (dashPerformed) {
            return;
        }

        float xVelocity = enemyBoss.dashAttackDir * enemyBoss.dashSpeed;
        enemyBoss.Move(xVelocity, rb.linearVelocity.y);
        HandlePlayerHit();

        if (!dashDamageApplied && CanApplyDashDamage()) {
            enemyBoss.combat.PerformAttackOnTarget(player);
            dashDamageApplied = true;
        }

        if (ReachedMaxDistance() || enemyBoss.OutOfMoveRange()) {
            enemyBoss.MoveRaw(0, 0);
            enemyBoss.vfx.StopImageEchoEffect();
            dashPerformed = true;
            anim.SetBool("DashAttackPerformed", true);
        } 
    }

    public override void Exit() {
        base.Exit();

        enemyBoss.gameObject.layer = LayerMask.NameToLayer("Enemy");

        UpdateDashAttackIndex();
    }

    private void HandleStateExit() {
        if (DashAttackQueued()) {
            anim.SetBool("DashAttack", false);
            anim.SetBool("DashAttackPerformed", true);
            enemyBoss.SetDashAttackDir(-enemyBoss.dashAttackDir);
            enemyBoss.EnterDashAttackStateWithDelay();
        } else {
            stateMachine.ChangeState(enemyBoss.battleState);
        }
    }

    private bool DashAttackQueued() {
        return dashAttackIndex < enemyBoss.dashAttackLimit[enemyBoss.phaseIndex];
    }

    private void UpdateDashAttackIndex() {
        dashAttackIndex += 1;

        if (dashAttackIndex > enemyBoss.dashAttackLimit[enemyBoss.phaseIndex]) {
            dashAttackIndex = 1;
        }
    }

    private void HandlePlayerHit() {
        if (playerHit) {
            return;
        }

        if (!ReachedPlayer()) {
            return;
        }

        playerHit = true;
        lastTimePlayerHit = tc.time;
    }

    private bool CanApplyDashDamage() {
        return playerHit && tc.time > lastTimePlayerHit + enemyBoss.dashDamageDelay;
    }

    private bool ReachedPlayer() {
        Vector2 offset = player.transform.position - enemyBoss.transform.position;

        return Mathf.Abs(offset.x) <= enemyBoss.dashAttackRange.x / 2
            && Mathf.Abs(offset.y) <= enemyBoss.dashAttackRange.y / 2;
    }

    private bool ReachedMaxDistance() {
        return 
            Vector2.Distance(enemyBoss.transform.position, startPosition) 
            > enemyBoss.dashAttackMaxDistance;
    }
}
