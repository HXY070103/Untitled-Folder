using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDarkKnight : Enemy {
    public EnemyDarkKnightDashAttackState dashAttackState;

    [Header("Dark knight specific info")]

    [Header("Sword wave info")]
    [SerializeField] private GameObject swordWavePrefab;
    [SerializeField] private Transform swordWaveStartPoint;
    [SerializeField] private float swordWaveSpeed = 8;
    [SerializeField] private int swordWaveCount = 1;
    [SerializeField] private int lowHealthSwordWaveCount = 3;
    [SerializeField] private float swordWaveInterval = 0.25f;
    [SerializeField] private float lowHealthSwordWaveSpeed = 11f;

    [Header("Ceiling sword wave")]
    [SerializeField] private float phaseThreshold = 0.5f;
    [SerializeField] private float ceilingAttackCooldown = 6f;
    [SerializeField] private float ceilingWarningDuration = 0.9f;
    [SerializeField] private float ceilingHeight = 8f;
    [SerializeField] private float arenaHalfWidth = 14f;
    [SerializeField] private int ceilingLaneCount = 5;
    [SerializeField] private int ceilingWaveCount = 4;
    [SerializeField] private float ceilingWaveSpeed = 12f;

    private float lastCeilingAttack = float.NegativeInfinity;
    private bool arenaAttackInProgress;
    private float arenaCenterX;
    public bool ArenaAttackInProgress => arenaAttackInProgress;

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

        arenaCenterX = transform.position.x;

        stateMachine.Initialize(idleState);
    }

    protected override void Update() {
        base.Update();

        if (health.GetHPPercentage() < phaseThreshold && player != null &&
            !arenaAttackInProgress && Time.time >= lastCeilingAttack + ceilingAttackCooldown) {
            lastCeilingAttack = Time.time;
            StartCoroutine(CeilingAttackCo());
        }
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        if (!arenaAttackInProgress) StartCoroutine(SwordWaveVolleyCo());
    }

    private IEnumerator SwordWaveVolleyCo() {
        arenaAttackInProgress = true;
        bool lowHealth = health.GetHPPercentage() < phaseThreshold;
        int count = lowHealth ? lowHealthSwordWaveCount : swordWaveCount;
        float speed = lowHealth ? lowHealthSwordWaveSpeed : swordWaveSpeed;
        for (int i = 0; i < count; i++) {
            GameObject swordWave = Instantiate(swordWavePrefab, swordWaveStartPoint.position, Quaternion.identity);
            swordWave.GetComponent<EnemyDarkKnightSwordWave>()?.SetUpSwordWave(facingDir * speed, combat);
            if (i < count - 1) yield return new WaitForSeconds(swordWaveInterval);
        }
        arenaAttackInProgress = false;
    }

    private IEnumerator CeilingAttackCo() {
        arenaAttackInProgress = true;
        Move(0, rb.linearVelocity.y);
        vfx.SetAttackAlert(true);
        yield return new WaitForSeconds(ceilingWarningDuration);
        vfx.SetAttackAlert(false);

        int lanes = Mathf.Max(2, ceilingLaneCount);
        int safeLane = Random.Range(0, lanes);
        int spawned = 0;
        for (int lane = 0; lane < lanes && spawned < Mathf.Min(ceilingWaveCount, lanes - 1); lane++) {
            if (lane == safeLane) continue;
            float t = lanes == 1 ? 0.5f : (float)lane / (lanes - 1);
            float x = arenaCenterX - arenaHalfWidth + t * arenaHalfWidth * 2;
            Vector3 spawnPosition = new Vector3(x, transform.position.y + ceilingHeight, transform.position.z);
            GameObject swordWave = Instantiate(swordWavePrefab, spawnPosition, Quaternion.identity);
            swordWave.GetComponent<EnemyDarkKnightSwordWave>()?.SetUpSwordWave(Vector2.down * ceilingWaveSpeed, combat);
            spawned++;
        }
        arenaAttackInProgress = false;
    }
}
