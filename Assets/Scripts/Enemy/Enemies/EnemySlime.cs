using UnityEngine;

public class EnemySlime: Enemy, ICounterable {
    [Header("Slime specific info")]
    [SerializeField] private GameObject slimeToCreatePrefab;
    [SerializeField] private int amountOfSlimesToCreate = 2;
    [SerializeField] private Vector2 newSlimePositionOffset = new Vector2(2, 1);
    [SerializeField] private bool hasStunnedRecovery = true;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        battleState = new EnemyBattleState(this, stateMachine, "Battle");
        deadState = new EnemySlimeDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");

        anim.SetBool("HasStunnedRecovery", hasStunnedRecovery);
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public bool CanBeCountered() {
        return InAttackWindow;
    }
    
    public void HandleCounter() {
        stateMachine.ChangeState(stunnedState);
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
        StartBattleStateCheck(player);
    }

    private void ApplyRespwanPosition(Vector2 positionOffset) {
        float xPosition = transform.position.x + positionOffset.x * Random.Range(-1f, 1f);
        float yPosition = transform.position.y + positionOffset.y * Random.Range(1f, 2f);
        transform.position = new Vector2(xPosition, yPosition);
    }

    private void StartBattleStateCheck(Transform player) {
        TryToEnterBattleState(player);
        InvokeRepeating(nameof(ReEnterBattleState), 0, 0.3f);
    }

    private void ReEnterBattleState() {
        if (stateMachine.currentState == battleState 
            || stateMachine.currentState == attackState) {
            CancelInvoke(nameof(ReEnterBattleState));
            return;
        }

        stateMachine.ChangeState(battleState);
    }
}