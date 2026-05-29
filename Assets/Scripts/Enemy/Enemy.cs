using UnityEngine;

public class Enemy : Entity, ISlowable{
    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyAttackState attackState;
    public EnemyBattleState battleState;
    public EnemyDeadState deadState;
    public EnemyStunnedState stunnedState;

    [Header("Battle info")]
    public float battleMoveSpeed = 3;
    public float attackDistance = 2;
    public float battleDuration = 5;
    public float minRetreatDistance = 1;
    public Vector2 retreatSpeed = new Vector2(5, 3);

    [Header("Stunned info")]
    public Vector2 stunnedSpeed = new Vector2(7, 7);
    public float stunnedDuration = 1;

    [Header("Movement info")]
    public float idleDurarion = 2;
    public float moveSpeed = 1.4f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1;

    [Header("Player Detection info")]
    [SerializeField] private float playerCheckDistance = 10;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;

    public Transform player { get; private set; }

    private bool inAttackWindow = false;
    public bool InAttackWindow => inAttackWindow;

    public float GetBattleMoveSpeed() {
        return battleMoveSpeed * speedMultiplier;
    }

    public float GetMoveSpeed() {
        return moveSpeed * speedMultiplier;
    }

    public void SlowDown(float slowDownPercentage) {
        float newSpeedMultiplier = 1 - slowDownPercentage;
        speedMultiplier = newSpeedMultiplier;
        anim.speed = newSpeedMultiplier;
    }

    public void RecoverSpeed() {
        speedMultiplier = 1;
        anim.speed = 1;
    }

    public void EnterAttackWindow() {
        inAttackWindow = true;
    }

    public void ExitAttackWindow() {
        inAttackWindow = false;
    }

    public override void EntityDeath() {
        base.EntityDeath();

        stateMachine.ChangeState(deadState);
    }

    private void HandlePlayerDeath() {
        stateMachine.ChangeState(idleState);
    }

    public void TryToEnterBattleState(Transform player) {
        if (stateMachine.currentState == battleState 
            || stateMachine.currentState == attackState) {
            return;
        }

        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerReference() {
        if (player == null) {
            player = PlayerDetected().transform;
        }

        return player;
    }

    public RaycastHit2D PlayerDetected() {
        RaycastHit2D hit = Physics2D.Raycast(
                playerCheck.position, 
                Vector2.right, 
                facingDir * playerCheckDistance, 
                whatIsPlayer | whatIsGround
            );

        if (hit.collider == null 
            || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return default;
        }

        return hit;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            playerCheck.position, 
            playerCheck.position + new Vector3(facingDir * playerCheckDistance, 0)
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            playerCheck.position, 
            playerCheck.position + new Vector3(facingDir * attackDistance, 0)
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            playerCheck.position, 
            playerCheck.position + new Vector3(facingDir * minRetreatDistance, 0)
        );
    }

    private void OnEnable(){
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable() {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }
}
