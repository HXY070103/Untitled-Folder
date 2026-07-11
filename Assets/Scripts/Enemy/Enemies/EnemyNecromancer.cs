using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNecromancer : Enemy {
    public EnemyNecromancerSpellCastState spellCastState;

    [Header("Necromancer specific info")]
    
    [Header("Fire info")]
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private Transform fireStartPoint;
    [SerializeField] private float fallingFireHeight = 7f;
    [SerializeField] private float fallingFireSpeed = 9f;

    [Header("Spell cast info")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellStartPoint;
    [SerializeField] private float spellGenerationRadius = 0.5f;
    [SerializeField] private int amountToCast = 3;
    [SerializeField] private float spellCastInterval = 0.3f;
    [SerializeField] private float spellChaseSpeed = 10;
    public float spellCastCooldown = 5;

    [Header("Flight and forced phase")]
    [SerializeField] private NecromancerArenaController arenaController;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private float flightHeight = 3.5f;
    [SerializeField] private float verticalFlightSpeed = 3f;
    [SerializeField] private float flightAnchorInterval = 3f;
    [SerializeField] private float forcedPhaseThreshold = 0.5f;
    [SerializeField] private float forcedPhaseMinDuration = 6f;
    [SerializeField] private int forcedSummonCount = 3;
    [SerializeField] private int barrageWaves = 4;
    [SerializeField] private int barrageProjectileCount = 12;
    [SerializeField] private float barrageProjectileSpeed = 7f;
    [SerializeField] private float barrageWaveInterval = 0.8f;

    private readonly List<EnemySkeleton> forcedSummons = new List<EnemySkeleton>();
    private bool forcedPhaseTriggered;
    private bool forcedPhaseActive;
    private float flightTargetY;
    private int flightAnchorIndex;
    private float nextAnchorChange;
    public bool ForcedPhaseActive => forcedPhaseActive;

    private Coroutine spellCastCo;
    private bool spellCastPerformed;
    public bool SpellCastPerformed => spellCastPerformed;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyNecromancerBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        spellCastState = new EnemyNecromancerSpellCastState(this, stateMachine, "SpellCast");

        anim.SetBool("HasStunnedRecovery", true);
    }

    protected override void Start() {
        base.Start();

        if (arenaController == null) arenaController = FindFirstObjectByType<NecromancerArenaController>();
        flightTargetY = transform.position.y + flightHeight;
        flightAnchorIndex = 0;
        nextAnchorChange = Time.time + flightAnchorInterval;
        rb.gravityScale = 0;

        stateMachine.Initialize(idleState);
    }

    protected override void Update() {
        base.Update();

        if (!forcedPhaseTriggered && health.GetHPPercentage() <= forcedPhaseThreshold) {
            forcedPhaseTriggered = true;
            StartCoroutine(ForcedPhaseCo());
        }
    }

    private void LateUpdate() {
        if (forcedPhaseActive || health.GetCurrentHP() <= 0) return;
        Transform[] anchors = arenaController != null ? arenaController.FlightAnchors : null;
        if (anchors != null && anchors.Length > 0) {
            if (Time.time >= nextAnchorChange) {
                flightAnchorIndex = Random.Range(0, anchors.Length);
                nextAnchorChange = Time.time + flightAnchorInterval;
            }
            Transform anchor = anchors[Mathf.Clamp(flightAnchorIndex, 0, anchors.Length - 1)];
            if (anchor != null) {
                transform.position = Vector3.MoveTowards(
                    transform.position, anchor.position, verticalFlightSpeed * Time.deltaTime);
                return;
            }
        }
        Vector3 position = transform.position;
        position.y = Mathf.MoveTowards(position.y, flightTargetY, verticalFlightSpeed * Time.deltaTime);
        transform.position = position;
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        if (stateMachine.currentState == attackState) {
            GenerateFire();
        }

        if (stateMachine.currentState == spellCastState) {
            StartSpellCast();
        }
    }

    private void GenerateFire() {
        Vector3 targetPosition = player != null ? player.position : fireStartPoint.position;
        Vector3 spawnPosition = targetPosition + Vector3.up * fallingFireHeight;
        GameObject fire = Instantiate(firePrefab, spawnPosition, Quaternion.identity);
        fire.GetComponent<EnemyNecromancerFire>()?.SetUpFallingFire(
            combat, Vector2.down * fallingFireSpeed, fallingFireHeight / fallingFireSpeed + 1f);
    }

    public void SetSpellCastPerformed(bool performed) {
        spellCastPerformed = performed;
    }

    private void StartSpellCast() {
        if (spellCastCo != null) {
            StopCoroutine(spellCastCo);
        }

        spellCastCo = StartCoroutine(SpellCastCo());
    }

    private IEnumerator SpellCastCo() {
        for (int i = 0; i < amountToCast; i += 1) {
            Vector2 dir = CalculateDirection(i);
            Vector3 position = GeneratePosition(dir);
            GameObject projectile = 
                Instantiate(spellPrefab, position, Quaternion.identity);
            projectile.GetComponent<EnemyNecromancerProjectile>().SetUpProjectile(spellChaseSpeed, player, combat);
            yield return new WaitForSeconds(spellCastInterval);
        }

        SetSpellCastPerformed(true);
    }

    private Vector2 CalculateDirection(int i) {
        float angle = i * 360f / amountToCast;
        float rad = angle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    private Vector3 GeneratePosition(Vector2 dir) {
        Vector3 offset = dir * spellGenerationRadius;;
        return spellStartPoint.position + offset;
    }

    private IEnumerator ForcedPhaseCo() {
        forcedPhaseActive = true;
        Move(0, 0);
        gameObject.layer = LayerMask.NameToLayer("Untargetable");
        health.SetInvulnerable(true);

        if (arenaController != null) yield return arenaController.RemovePlatformsSequence();
        SpawnForcedSummons();

        float phaseStart = Time.time;
        for (int wave = 0; wave < barrageWaves; wave++) {
            SpawnBarrageWave();
            yield return new WaitForSeconds(barrageWaveInterval);
        }

        while (Time.time < phaseStart + forcedPhaseMinDuration || HasLivingForcedSummons()) {
            yield return null;
        }

        arenaController?.RestorePlatforms();
        health.SetInvulnerable(false);
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        forcedPhaseActive = false;
    }

    private void SpawnForcedSummons() {
        if (skeletonPrefab == null || player == null) return;
        int count = Mathf.Max(0, forcedSummonCount);
        LevelClearManager.instance?.AddPendingEnemies(count);
        Transform[] points = arenaController != null ? arenaController.SummonPoints : null;
        for (int i = 0; i < count; i++) {
            Vector3 position = points != null && points.Length > 0 && points[i % points.Length] != null
                ? points[i % points.Length].position
                : transform.position + Vector3.right * ((i % 2 == 0 ? -1 : 1) * 4);
            EnemySkeleton skeleton = Instantiate(skeletonPrefab, position, Quaternion.identity)
                .GetComponent<EnemySkeleton>();
            if (skeleton == null) continue;
            forcedSummons.Add(skeleton);
            skeleton.TryToEnterBattleState(player);
        }
    }

    private bool HasLivingForcedSummons() {
        forcedSummons.RemoveAll(skeleton => skeleton == null || skeleton.health.GetCurrentHP() <= 0);
        return forcedSummons.Count > 0;
    }

    private void SpawnBarrageWave() {
        if (spellPrefab == null) return;
        int count = Mathf.Max(6, barrageProjectileCount);
        int safeStart = Random.Range(0, count);
        for (int i = 0; i < count; i++) {
            if (i == safeStart || i == (safeStart + 1) % count) continue;
            float angle = i * 360f / count;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 position = transform.position + (Vector3)(direction * spellGenerationRadius);
            GameObject projectile = Instantiate(spellPrefab, position, Quaternion.identity);
            projectile.GetComponent<EnemyNecromancerProjectile>()?.SetUpProjectileDirection(
                barrageProjectileSpeed, direction, combat);
        }
    }

    private void OnDestroy() {
        arenaController?.RestorePlatforms();
    }
}
