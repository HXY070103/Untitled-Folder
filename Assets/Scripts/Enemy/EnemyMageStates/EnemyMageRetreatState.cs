using UnityEngine;

public class EnemyMageRetreatState : EnemyState {
    private EnemyMage enemyMage;
    private Vector3 startPosition;
    private Transform player;

    public EnemyMageRetreatState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyMage = enemy as EnemyMage;
    }

    public override void Enter() {
        base.Enter();

        enemyMage.gameObject.layer = LayerMask.NameToLayer("Untargetable");

        if (player == null) {
            player = enemy.GetPlayerReference();
        }

        startPosition = enemy.transform.position;

        float xVelocity = -DirectionToPlayer() * enemyMage.retreatSpeed;
        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);
        enemy.HandleFlip(DirectionToPlayer());

        enemy.vfx.DoImageEchoEffect(float.PositiveInfinity);
    }

    public override void Update() {
        base.Update();

        if (ReachedMaxDistance() || enemyMage.CannotMoveBackwards()) {
            rb.linearVelocity = Vector2.zero;
            enemyMage.vfx.StopImageEchoEffect();
            stateMachine.ChangeState(enemyMage.spellCastState);
        }
    }

    public override void Exit() {
        base.Exit();

        enemyMage.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    private bool ReachedMaxDistance() {
        return 
            Vector2.Distance(enemyMage.transform.position, startPosition) 
            > enemyMage.retreatMaxDistance;
    }

    private int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
