using Unity.VisualScripting;
using UnityEngine;

public class EnemyDarkKnight : Enemy {
    public EnemyDarkKnightDashAttackState dashAttackState;

    [Header("Dark knight specific info")]
    [SerializeField] private GameObject swordWavePrefab;
    [SerializeField] private Transform swordWaveStartPoint;
    [SerializeField] private float swordWaveSpeed = 8;

    [Space]
    public int numOfBasicAttack = 2;
    public int numOfDashAttack = 1;
    public float dashAttackCooldown = 2f;
    public float dashAttackMaxDistance = 15;
    public float dashAttackSpeed = 15;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        battleState = new EnemyDarkKnightBattleState(this, stateMachine, "Battle");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        dashAttackState = new EnemyDarkKnightDashAttackState(this, stateMachine, "DashAttack");
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        GameObject swordWave = 
            Instantiate(swordWavePrefab, swordWaveStartPoint.position, Quaternion.identity);
        swordWave.GetComponent<EnemyDarkKnightSwordWave>().SetUpSwordWave(facingDir * swordWaveSpeed, combat);
    }
}
