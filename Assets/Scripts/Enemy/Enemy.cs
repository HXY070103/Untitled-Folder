using System.Collections;
using UnityEngine;

public class Enemy : Entity, ICounterable {
    public EntityStats stats { get; private set; }
    public EnemyHealth health { get; private set; }
    public EnemyCombat combat { get; private set; }
    public EnemyVFX vfx { get; private set; }
    public EnemySFX sfx { get; private set; }

    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyBattleState battleState;
    public EnemyPreAttackState preAttackState;
    public EnemyAttackState attackState;
    public EnemyDeadState deadState;
    public EnemyStunnedState stunnedState;

    [Header("Battle info")]
    public float battleMoveSpeed = 3;
    public float attackDistance = 2;
    public float attackWindup = 0.5f;
    public float attackCooldown = 0.5f;
    public float battleDuration = 5;
    public float minRetreatDistance = 1;
    public Vector2 quickRetreatSpeed = new Vector2(5, 3);

    public float currentWindup { get; private set; }
    public float currentCooldown { get; private set; }
    public EnemyState currentAttack { get; private set; }

    [Header("Stunned info")]
    public Vector2 stunnedSpeed = new Vector2(7, 7);
    public float stunnedDuration = 1;

    [Header("Movement info")]
    public float idleDurarion = 2;
    public float moveSpeed = 1.4f;
    [Range(0, 2)]
    public float moveAnimSpeedMultiplier = 1;

    [Header("Player Detection info")]
    [SerializeField] protected float playerCheckDistance = 10;
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected Transform playerCheck;

    public Transform player { get; private set; }

    private bool inAttackWindow = false;
    public bool InAttackWindow => inAttackWindow;

    private Coroutine enterBattleStateWithDelayCo;

    protected override void Awake() {
        base.Awake();

        stats = GetComponent<EntityStats>();
        health = GetComponent<EnemyHealth>();
        combat = GetComponent<EnemyCombat>();
        vfx = GetComponent<EnemyVFX>();
        sfx = GetComponent<EnemySFX>();
    }

    protected override void Start() {
        base.Start();

        if (LevelClearManager.instance != null) {
            LevelClearManager.instance.RegisterSpawnedEnemy();
        }
    }

    protected override void Update() {
        base.Update();
    }

    public void SetAttack(float windup, float cooldown, EnemyState attack) {
        currentWindup = windup;
        currentCooldown = cooldown;
        currentAttack = attack;
    }

    public virtual void SpecialAttack() {
        sfx?.PlaySpecialAttack1();
    }

    public virtual bool CanBeCountered() {
        return InAttackWindow;
    }
    
    public virtual void HandleCounter() {
        stateMachine.ChangeState(stunnedState);
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
        if (stateMachine.currentState == idleState || stateMachine.currentState == moveState) {
            this.player = player;
            EnterBattleStateWithDelay();
        }
    }

    private void EnterBattleStateWithDelay() {
        if (enterBattleStateWithDelayCo != null) {
            StopCoroutine(enterBattleStateWithDelayCo);
        }

        enterBattleStateWithDelayCo = StartCoroutine(EnterBattleStateWithDelayCo());
    }

    private IEnumerator EnterBattleStateWithDelayCo() {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(battleState);
    }

    public void DestoryGameObjectWithDelay(float delay) {
        Destroy(gameObject, delay);
    }

    public Transform GetPlayerReference() {
        if (player == null) {
            player = DetectPlayer();
        }

        return player;
    }

    protected virtual Transform DetectPlayer() {
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

        return hit.transform;
    }

    public bool PlayerDetected() {
        return DetectPlayer() != null;
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
