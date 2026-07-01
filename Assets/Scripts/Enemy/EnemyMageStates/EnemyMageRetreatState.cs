using UnityEngine;

public class EnemyMageRetreatState : EnemyState {
    private EnemyMage enemyMage;
    private Transform player;
    private Vector3 startPosition;
    private int retreatDir;

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
        retreatDir = -DirectionToPlayer() ;
        
        enemyMage.HandleFlip(DirectionToPlayer());
        enemyMage.vfx.DoImageEchoEffect(float.PositiveInfinity);
    }

    public override void Update() {
        base.Update();

        float xVelocity = retreatDir * enemyMage.retreatSpeed;
        enemyMage.MoveRaw(xVelocity, rb.linearVelocity.y);

        if (ReachedMaxDistance() || enemyMage.CannotMoveBackwards()) {
            stateMachine.ChangeState(enemyMage.spellCastState);
        }
    }

    public override void Exit() {
        base.Exit();

        enemyMage.gameObject.layer = LayerMask.NameToLayer("Enemy");
        enemyMage.Move(0, 0);
        enemyMage.vfx.StopImageEchoEffect();
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

        return player.position.x > enemyMage.transform.position.x ? 1 : -1;
    }
}
