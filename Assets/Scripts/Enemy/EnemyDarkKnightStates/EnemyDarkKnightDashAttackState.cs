using UnityEngine;

public class EnemyDarkKnightDashAttackState : EnemyState {
    private EnemyDarkKnight enemyDarkKnight;
    private Vector3 startPosition;
    private Transform player;
    private int dashAttackDir;
    private bool dashPerformed;

    public EnemyDarkKnightDashAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyDarkKnight = enemy as EnemyDarkKnight;
    }

    public override void Enter() {
        base.Enter();

        enemyDarkKnight.gameObject.layer = LayerMask.NameToLayer("Untargetable");

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        startPosition = enemyDarkKnight.transform.position;
        dashAttackDir = DirectionToPlayer();
        dashPerformed = false;

        enemyDarkKnight.vfx.DoImageEchoEffect(float.PositiveInfinity);

        anim.SetBool("DashAttackPerformed", false);
    }

    public override void Update() {
        base.Update();

        if (triggerCalled) {
            stateMachine.ChangeState(enemyDarkKnight.battleState);
        }

        if (dashPerformed) {
            return;
        }

        float xVelocity = dashAttackDir * enemyDarkKnight.dashSpeed;
        enemyDarkKnight.Move(xVelocity, rb.linearVelocity.y);

        if (ReachedPlayer() || ReachedMaxDistance() || enemyDarkKnight.wallDetected) {
            enemyDarkKnight.Move(0, 0);
            enemyDarkKnight.vfx.StopImageEchoEffect();
            dashPerformed = true;
            anim.SetBool("DashAttackPerformed", true);
        } 
    }

    public override void Exit() {
        base.Exit();

        enemyDarkKnight.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    private bool ReachedPlayer() {
        return 
            Vector2.Distance(enemyDarkKnight.transform.position, player.transform.position) 
            < 1.5f;
    }

    private bool ReachedMaxDistance() {
        return 
            Vector2.Distance(enemyDarkKnight.transform.position, startPosition) 
            > enemyDarkKnight.dashAttackMaxDistance;
    }

    private int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
