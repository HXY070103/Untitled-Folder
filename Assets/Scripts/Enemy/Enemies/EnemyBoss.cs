using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy {
    public EnemyBossDashState dashState;
    public EnemyBossTeleportBehindState teleportBehindState;
    public EnemyBossTeleportAwayState teleportAwayState;
    public EnemyBossSpellCastState spellCastState;
    public EnemyBossSummonState summonState;

    [Header("Boss specific info")]
    public float specialAttackCooldown = 5;
    public List<EnemyState> specialAttackStates;
    public List<System.Action> spellCastActions;

    [Header("Arena phases")]
    [SerializeField] private BossArenaController arenaController;
    [SerializeField] private float phaseTwoThreshold = 0.7f;
    [SerializeField] private float phaseThreeThreshold = 0.4f;
    [SerializeField] private float floorAttackCooldown = 8f;
    [SerializeField] private int phaseTwoBrokenZones = 1;
    [SerializeField] private int phaseThreeBrokenZones = 2;
    [SerializeField] private int phaseTwoComboHits = 2;
    [SerializeField] private int phaseThreeComboHits = 3;

    private float lastFloorAttack = float.NegativeInfinity;
    private bool arenaAttackActive;
    private int comboHitsRemaining;
    public bool ArenaAttackActive => arenaAttackActive;

    [Header("Dash info")]
    public float dashSpeed = 12;
    public float minDashDistance = 12;

    [Header("Energy wave info")]
    [SerializeField] private GameObject energyWavePrefab;
    [SerializeField] private Transform energyWaveStartPoint;
    [SerializeField] private float energyWaveSpeed = 10;
    [SerializeField] private float energyWaveCooldown = 2;

    [Header("Spell cast info")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private int amountToCast = 3;
    [SerializeField] private float spellCastCooldown = 0.8f;
    [SerializeField] private float spellPositionOffset = 4;

    private Coroutine spellCastCo;
    private bool spellCastPerformed;
    public bool SpellCastPerformed => spellCastPerformed;

    [Header("Summon info")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private Transform summonStartPoint;
    [SerializeField] private int amountOfSkeletonToSummon = 3;
    [SerializeField] private float summonCooldown = 0.5f;
    [SerializeField] private GameObject[] phaseTwoSummonPrefabs;
    [SerializeField] private GameObject[] phaseThreeSummonPrefabs;
    [SerializeField] private int maxActiveSummons = 6;

    private readonly List<Enemy> activeSummons = new List<Enemy>();

    private Coroutine summonCo;
    private bool summonPerformed;
    public bool SummonPerformed => summonPerformed;

    [Header("Teleport info")]
    [SerializeField] private float teleportDelay = 1.2f;
    [SerializeField] private float teleportBehindOffset = 5;
    [SerializeField] private float teleportAwayOffset = 12;

    private bool teleportPerformed;
    public bool TeleportPerformed => teleportPerformed;

    private float leftMoveBound;
    private float rightMoveBound;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyBossBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        dashState = new EnemyBossDashState(this, stateMachine, "Battle");
        teleportBehindState = new EnemyBossTeleportBehindState(this, stateMachine, "Teleport");
        teleportAwayState = new EnemyBossTeleportAwayState(this, stateMachine, "Teleport");
        spellCastState = new EnemyBossSpellCastState(this, stateMachine, "SpellCast");
        summonState = new EnemyBossSummonState(this, stateMachine, "Battle");

        specialAttackStates = new List<EnemyState> {
                teleportBehindState,
                teleportAwayState, 
                summonState
            };

        spellCastActions = new List<System.Action> {
                StartSpellCast, 
                StartEnergyWave
            };

        anim.SetBool("HasStunnedRecovery", false);
    }

    protected override void Start() {
        base.Start();

        leftMoveBound = transform.position.x - playerCheckDistance;
        rightMoveBound = transform.position.x + playerCheckDistance;
        if (arenaController == null) arenaController = FindFirstObjectByType<BossArenaController>();

        stateMachine.Initialize(idleState);
    }

    protected override void Update() {
        base.Update();
    }

    public override void SpecialAttack() {

        spellCastActions[Random.Range(0, spellCastActions.Count)]();
    }

    public void SetSpellCastPerformed(bool performed) {
        spellCastPerformed = performed;
    }

    private void StartEnergyWave() {
        sfx?.PlaySpecialAttack2();
        GameObject energyWave = 
            Instantiate(energyWavePrefab, energyWaveStartPoint.position, Quaternion.identity);
        energyWave.GetComponent<EnemyBossEnergyWave>().SetUpEnergyWave(facingDir * energyWaveSpeed, combat);
        Invoke(nameof(OnSpellCastFinished), energyWaveCooldown);
    }

    private void OnSpellCastFinished() {
        SetSpellCastPerformed(true);
    }

    private void StartSpellCast() {
        if (spellCastCo != null) {
            StopCoroutine(spellCastCo);
        }
        sfx?.PlaySpecialAttack1();
        spellCastCo = StartCoroutine(SpellCastCo());
    }

    private IEnumerator SpellCastCo() {
        for (int i = 0; i < amountToCast; i += 1) {
            Vector3 offset = Vector3.up * spellPositionOffset;
            Vector3 position = player.position + offset;
            GameObject lightning = Instantiate(spellPrefab, position, Quaternion.identity);
            lightning.GetComponent<EnemyBossLightning>().SetUpLightning(combat);
            yield return new WaitForSeconds(spellCastCooldown);
        }

        SetSpellCastPerformed(true);
    }

    public void SetSummonPerformed(bool performed) {
        summonPerformed = performed;
    }

    public void StartSummon() {
        if (summonCo != null) {
            StopCoroutine(summonCo);
        }

        summonCo = StartCoroutine(SummonCo());
    }

    private IEnumerator SummonCo() {
        CleanupSummons();
        int amount = Mathf.Min(amountOfSkeletonToSummon, maxActiveSummons - activeSummons.Count);
        LevelClearManager.instance?.AddPendingEnemies(amount);
        for (int i = 0; i < amount; i += 1) {
            GameObject prefab = GetSummonPrefab(i);
            if (prefab == null) continue;
            Transform[] points = arenaController != null ? arenaController.SummonPoints : null;
            Vector3 position = points != null && points.Length > 0 && points[i % points.Length] != null
                ? points[i % points.Length].position : summonStartPoint.position;
            GameObject summonedObject = Instantiate(prefab, position, Quaternion.identity);
            Enemy summonedEnemy = summonedObject.GetComponent<Enemy>();
            if (summonedEnemy == null) continue;
            activeSummons.Add(summonedEnemy);
            summonedEnemy.HandleFlip(facingDir);
            summonedEnemy.TryToEnterBattleState(player);

            Color color = Color.red;
            vfx.GenerateParticleEffect(color);

            yield return new WaitForSeconds(summonCooldown);
        }

        SetSummonPerformed(true);
    }

    private GameObject GetSummonPrefab(int index) {
        GameObject[] pool = GetPhase() >= 2 ? phaseThreeSummonPrefabs
            : GetPhase() >= 1 ? phaseTwoSummonPrefabs : null;
        if (pool == null || pool.Length == 0) return skeletonPrefab;
        return pool[index % pool.Length] != null ? pool[index % pool.Length] : skeletonPrefab;
    }

    private void CleanupSummons() {
        activeSummons.RemoveAll(enemy => enemy == null || enemy.health.GetCurrentHP() <= 0);
    }

    public int GetPhase() {
        float hp = health.GetHPPercentage();
        if (hp <= phaseThreeThreshold) return 2;
        if (hp <= phaseTwoThreshold) return 1;
        return 0;
    }

    public bool TryStartFloorAttack() {
        if (GetPhase() == 0 || arenaAttackActive || arenaController == null ||
            Time.time < lastFloorAttack + floorAttackCooldown) return false;
        int zoneCount = GetPhase() >= 2 ? phaseThreeBrokenZones : phaseTwoBrokenZones;
        if (!arenaController.TryStartFloorAttack(this, zoneCount)) return false;
        lastFloorAttack = Time.time;
        StartCoroutine(ArenaFloorAttackCo());
        return true;
    }

    private IEnumerator ArenaFloorAttackCo() {
        arenaAttackActive = true;
        Move(0, 0);
        RigidbodyConstraints2D previousConstraints = rb.constraints;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
        yield return null;
        while (arenaController != null && arenaController.HazardActive) yield return null;
        rb.constraints = previousConstraints;
        arenaAttackActive = false;
    }

    public void PrepareCombo() {
        if (comboHitsRemaining > 0) return;
        int phase = GetPhase();
        comboHitsRemaining = phase >= 2 ? phaseThreeComboHits : phase >= 1 ? phaseTwoComboHits : 1;
    }

    public bool TryConsumeComboHit() {
        if (comboHitsRemaining <= 0) return false;
        comboHitsRemaining--;
        return true;
    }

    public void Teleport() {
        if (stateMachine.currentState == teleportBehindState) {
            TeleportBehind();
        }

        if (stateMachine.currentState == teleportAwayState) {
            TeleportAway();
        }
    }

    private void TeleportBehind() {
        Vector3 newPosition = player.position + new Vector3(facingDir * teleportBehindOffset, 0);
        
        if (newPosition.x < leftMoveBound) {
            newPosition.x = leftMoveBound;
        }

        if (newPosition.x > rightMoveBound) {
            newPosition.x = rightMoveBound;
        }

        transform.position = newPosition;
        SetTeleportPerformed(true);
    }

    private void TeleportAway() {
        float midPointInMoveRange = (leftMoveBound + rightMoveBound) / 2;
        int dir = player.position.x < midPointInMoveRange ? 1 : -1;
        Vector3 newPosition = player.position + new Vector3(dir * teleportAwayOffset, 0);

        transform.position = newPosition;
        SetTeleportPerformed(true);
    }

    public void SetTeleportPerformed(bool performed) {
        teleportPerformed = performed;
    }

    public bool OutOfMoveRange() {
        return 
            transform.position.x < leftMoveBound || transform.position.x > rightMoveBound;
    }

    protected override Transform DetectPlayer() {
        Collider2D player = 
            Physics2D.OverlapCircle(playerCheck.position, playerCheckDistance, whatIsPlayer);

        if (player == null) {
            return default;
        }

        return player.transform;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCheck.position, playerCheckDistance);
    }
}
