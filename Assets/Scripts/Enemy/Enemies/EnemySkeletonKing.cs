using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeletonKing : Enemy {
    public EnemySkeletonKingSummonState summonState;

    [Header("Skeleton king specific info")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private Transform summonStartPoint;
    [SerializeField] private int amountOfSkeletonToSummon = 3;
    [SerializeField] private float summonCooldown = 0.5f;
    [SerializeField] private int maxActiveSkeletons = 6;
    public float healthThresholdForSummon = 0.5f;
    public float summonAttackCooldown = 5;
    public float summonAttackDistance = 10;

    [Header("Horizontal sweep")]
    [SerializeField] private GameObject horizontalSweepPrefab;
    [SerializeField] private float sweepWarningDuration = 0.8f;
    [SerializeField] private float sweepCooldown = 6f;
    [SerializeField] private float lowHealthSweepCooldown = 4f;
    [SerializeField] private float sweepSpeed = 11f;
    [SerializeField] private float arenaHalfWidth = 16f;
    [SerializeField] private float sweepHeightOffset = 0.35f;

    private readonly List<EnemySkeleton> activeSkeletons = new List<EnemySkeleton>();
    private float lastSweepTime = float.NegativeInfinity;
    private float lastSummonTime = float.NegativeInfinity;
    private bool arenaAttackInProgress;
    private float arenaCenterX;
    public bool ArenaAttackInProgress => arenaAttackInProgress;

    private Coroutine summonCo;
    private bool summonPerformed;
    public bool SummonPerformed => summonPerformed;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemySkeletonKingBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        summonState = new EnemySkeletonKingSummonState(this, stateMachine, "Battle");

        anim.SetBool("HasStunnedRecovery", false);
    }

    protected override void Start() {
        base.Start();

        arenaCenterX = transform.position.x;

        stateMachine.Initialize(idleState);
    }

    public void SetSummonPerformed(bool performed) {
        summonPerformed = performed;
    }

    public void StartSummon() {
        if (summonCo != null) {
            StopCoroutine(summonCo);
        }

        CleanupSummons();
        int amount = Mathf.Min(amountOfSkeletonToSummon, maxActiveSkeletons - activeSkeletons.Count);
        if (amount <= 0) {
            SetSummonPerformed(true);
            return;
        }
        LevelClearManager.instance?.AddPendingEnemies(amount);
        lastSummonTime = Time.time;
        summonCo = StartCoroutine(SummonCo(amount));
    }

    private IEnumerator SummonCo(int amount) {
        for (int i = 0; i < amount; i += 1) {
            GameObject skeleton = 
                Instantiate(skeletonPrefab, summonStartPoint.position, Quaternion.identity);
            EnemySkeleton spawnedSkeleton = skeleton.GetComponent<EnemySkeleton>();
            activeSkeletons.Add(spawnedSkeleton);
            spawnedSkeleton.HandleFlip(facingDir);
            spawnedSkeleton.TryToEnterBattleState(player);

            Color color = Color.purple;
            vfx.GenerateParticleEffect(color);

            yield return new WaitForSeconds(summonCooldown);
        }

        SetSummonPerformed(true);
    }

    public bool ShouldSummon() {
        CleanupSummons();
        return activeSkeletons.Count < maxActiveSkeletons && Time.time >= lastSummonTime + summonAttackCooldown;
    }

    public bool CanUseHorizontalSweep() {
        float cooldown = health.GetHPPercentage() < healthThresholdForSummon
            ? lowHealthSweepCooldown : sweepCooldown;
        return !arenaAttackInProgress && horizontalSweepPrefab != null && Time.time >= lastSweepTime + cooldown;
    }

    public void StartHorizontalSweep() {
        if (!CanUseHorizontalSweep()) return;
        lastSweepTime = Time.time;
        StartCoroutine(HorizontalSweepCo());
    }

    private IEnumerator HorizontalSweepCo() {
        arenaAttackInProgress = true;
        Move(0, rb.linearVelocity.y);
        vfx.SetAttackAlert(true);
        yield return new WaitForSeconds(sweepWarningDuration);
        vfx.SetAttackAlert(false);

        Vector3 spawnPosition = new Vector3(
            arenaCenterX - arenaHalfWidth,
            transform.position.y + sweepHeightOffset,
            transform.position.z);
        GameObject sweep = Instantiate(horizontalSweepPrefab, spawnPosition, Quaternion.identity);
        EnemyBossEnergyWave wave = sweep.GetComponent<EnemyBossEnergyWave>();
        if (wave != null) wave.SetUpEnergyWave(sweepSpeed, combat, arenaHalfWidth * 2 + 2);
        arenaAttackInProgress = false;
    }

    private void CleanupSummons() {
        activeSkeletons.RemoveAll(skeleton => skeleton == null || skeleton.health.GetCurrentHP() <= 0);
    }
}
