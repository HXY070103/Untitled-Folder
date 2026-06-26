using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemySlime: Enemy {
    [Header("Slime specific info")]
    [SerializeField] private GameObject slimeToCreatePrefab;
    [SerializeField] private int amountOfSlimesToCreate = 2;
    [SerializeField] private Vector2 newSlimePositionOffset = new Vector2(2, 1);

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        battleState = new EnemyBattleState(this, stateMachine, "Battle");
        deadState = new EnemySlimeDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");

        anim.SetBool("HasStunnedRecovery", true);
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public void CreateSlimeOnDeath() {
        if (slimeToCreatePrefab == null) {
            return;
        }

        for (int i = 0; i < amountOfSlimesToCreate; i += 1) {
            GameObject newSlime = 
                Instantiate(slimeToCreatePrefab, transform.position, Quaternion.identity);
            newSlime.GetComponent<EnemySlime>().SetUpRespawnSlime(newSlimePositionOffset, player);
        }
    }

    public void SetUpRespawnSlime(Vector2 positionOffset, Transform player) {
        ApplyRespwanPosition(positionOffset);
        HandleFlip(facingDir);
        TryToEnterBattleState(player);
    }

    private void ApplyRespwanPosition(Vector2 positionOffset) {
        float xPosition = transform.position.x + positionOffset.x * Random.Range(-1f, 1f);
        float yPosition = transform.position.y + positionOffset.y * Random.Range(1f, 2f);
        transform.position = new Vector2(xPosition, yPosition);
    }
}