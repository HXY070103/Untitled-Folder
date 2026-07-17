using UnityEngine;

public class EnemyStoneGuardDeadState : EnemyDeadState {
    private readonly EnemyStoneGuard stoneGuard;

    public EnemyStoneGuardDeadState(EnemyStoneGuard stoneGuard, StateMachine stateMachine, string animBoolName) 
        : base(stoneGuard, stateMachine, animBoolName) {
        this.stoneGuard = stoneGuard;
    }

    public override void Enter() {
        anim.SetBool(animBoolName, true);

        stoneGuard.DestroyActiveAttacks();
        enemy.ExitAttackWindow();
        enemy.vfx.SetAttackAlert(false);
        enemy.sfx?.PlayDeath();
        enemy.StopAllCoroutines();

        Collider2D bodyCollider = enemy.GetComponent<Collider2D>();

        if (bodyCollider != null) {
            bodyCollider.enabled = false;
        }

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        enemy.health.EnableHealthBar(false);

        tc.ResetLocalTimeScale();
        anim.speed = 1;
        stateMachine.SwitchOff();
        tc.SwitchOff();

        enemy.DestoryGameObjectWithDelay(10);
    }
}
