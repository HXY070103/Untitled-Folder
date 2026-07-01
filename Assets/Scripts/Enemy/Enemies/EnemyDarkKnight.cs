using Unity.VisualScripting;
using UnityEngine;

public class EnemyDarkKnight : Enemy {
    public EnemyDarkKnightDashAttackState dashAttackState;

    [Header("Dark knight specific info")]

    [Header("Sword wave info")]
    [SerializeField] private GameObject swordWavePrefab;
    [SerializeField] private Transform swordWaveStartPoint;
    [SerializeField] private float swordWaveSpeed = 8;

    [Header("Dash attack info")]
    public int numOfBasicAttack = 2;
    public int numOfDashAttack = 1;
    public float dashAttackCooldown = 2f;
    public float dashAttackMaxDistance = 15;
    public float dashSpeed = 15;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyDarkKnightBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        dashAttackState = new EnemyDarkKnightDashAttackState(this, stateMachine, "DashAttack");

        anim.SetBool("HasStunnedRecovery", false);
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
