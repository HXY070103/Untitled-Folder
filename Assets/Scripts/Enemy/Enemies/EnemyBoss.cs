using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossPhaseTransition { PhaseOneToTwo, PhaseTwoToThree }

public class EnemyBoss : Enemy, IEnemyPhaseTransition {
    private const string PhaseTransitionImmunitySource = "BossPhaseTransition";

    public EnemyPhaseTransitionState phaseTransitionState;
    public EnemyBossDashAttackState dashAttackState;
    public EnemyBossEnergyWaveState energyWaveState;
    public EnemyBossSummonState summonState;
    public EnemyBossLightningState lightningState;
    public EnemyBossTeleportState teleportState;
    
    [Header("Boss specific")]
    public int[] numOfAttack = {4, 4, 4};

    private EnemyBossUpgrade bossUpgrade;

    [SerializeField] private float phaseOneToTwoHPThreshold = 0.7f;
    [SerializeField] private float phaseTwoToThreeHPThreshold = 0.35f;
    [SerializeField] private float phaseTransitionDuration = 1.2f;
    [SerializeField] private float transitionOffstageOffset = 5f;

    private BossPhaseTransition phaseTransition;
    private bool phaseOneToTwoTransitionTriggered;
    private bool phaseTwoToThreeTransitionTriggered;

    public int phaseIndex { get; private set; } = 0;

    [Header("Combo attack info")]
    public float[] comboAttackWindup = {0.5f, 0.35f, 0.2f};
    public float[] comboAttackCooldown = {1, 0.7f, 0.5f};
    public int[] comboLimit = {1, 2, 3};

    private Coroutine enterAttackStateWithDelayCo;

    [Header("Dash attak info")]
    public float dashSpeed = 20;
    public float dashAttackMaxDistance = 15;
    public float dashDamageDelay = 0.2f;
    public Vector2 dashAttackRange = new Vector2(2.5f, 3.5f);
    public float[] dashAttackWindup = {0.8f, 0.7f, 0.6f};
    public float[] dashAttackCooldown = {0.5f, 0.4f, 0.3f};
    public float[] dashAttackLimit = {1, 2, 2};
    public float dashAttackDistance = 14;

    private Coroutine enterDashAttackStateWithDelayCo;

    public int dashAttackDir { get; private set; } = 1;

    [Header("Energy wave info")]
    [SerializeField] private GameObject energyWavePrefab;
    [SerializeField] private Transform energyWaveStartPoint;
    [SerializeField] private float energyWaveSpeed = 10;
    public float[] energyWaveWindup = {0.2f, 0.15f, 0.1f};
    public float[] energyWaveCooldown = {1.5f, 1.2f, 0.9f};
    public float energyWaveDistance = 15;

    private bool energyWavePerformed;
    public bool EnergyWavePerformed => energyWavePerformed;

    [Header("Lightning info")]
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private float lightningOffsetY = 4;
    [SerializeField] private float lightningInterval = 0.7f;
    [SerializeField] private float lightningWarningDuration = 1.2f;
    public int[] numOfLightningRound = {0, 1, 2};
    public float[] lightningWindup = {0, 0.1f, 0.05f};
    public float[] lightningCooldown = {0, 2.5f, 2};
    public float lightningDistance = 20;

    private Coroutine lightningCo;

    private bool lightningPerformed;
    public bool LightningPerformed => lightningPerformed;

    [Header("Summon info")]
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject archerElfPrefab;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject evilWizardPrefab;
    [SerializeField] private float minPlayerDistance = 3f;
    [SerializeField] private float summonCooldown = 0.5f;

    private Coroutine summonCo;
    private List<Enemy> activeSummonedEnemies = new List<Enemy>();

    private bool summonSpawnCompleted;
    private bool summonPerformed;
    public bool SummonPerformed => summonPerformed;

    [Header("Teleport info")]
    public float teleportDuration = 1.5f;

    private Vector3 TeleportPosition;
    public EnemyState teleportNextState { get; private set; }

    [Header("Arena info")]
    [SerializeField] private float arenaRadius = 20f;
    [SerializeField] private float arenaMargin = 3f;

    private float leftMoveBound;
    private float rightMoveBound;
    private float arenaGroundY;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyEliteIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyBossBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyBossPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyBossAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        phaseTransitionState = new EnemyPhaseTransitionState(this, stateMachine, "Battle");
        dashAttackState = new EnemyBossDashAttackState(this, stateMachine, "DashAttack");
        energyWaveState = new EnemyBossEnergyWaveState(this, stateMachine, "SpellCast");
        summonState = new EnemyBossSummonState(this, stateMachine, "Battle");
        lightningState = new EnemyBossLightningState(this, stateMachine, "SpellCast");
        teleportState = new EnemyBossTeleportState(this, stateMachine, "Teleport");

        bossUpgrade = GetComponent<EnemyBossUpgrade>();

        anim.SetBool("HasStunnedRecovery", false);
    }

    protected override void Start() {
        base.Start();

        leftMoveBound = transform.position.x - arenaRadius;
        rightMoveBound = transform.position.x + arenaRadius;
        arenaGroundY = transform.position.y;

        stateMachine.Initialize(idleState);

        health.OnHealthUpdate += TryStartPhaseTransition;
        EnemyHealth.OnEnemyKilled += HandleSummonedEnemyKilled;
        TryStartPhaseTransition();
    }

    private void OnDestroy() {
        health.OnHealthUpdate -= TryStartPhaseTransition;
        EnemyHealth.OnEnemyKilled -= HandleSummonedEnemyKilled;
    }

    private void TryStartPhaseTransition() {
        if (health.GetCurrentHP() <= 0 || stateMachine.currentState == deadState) {
            return;
        }

        switch (phaseIndex) {
            case 0: 
                HandlePhaseOneToTwoTransition();
                break;
            case 1:
                HandlePhaseTwoToThreeTransition();
                break;
        }
    }

    private void HandlePhaseOneToTwoTransition() {
        if (!phaseOneToTwoTransitionTriggered && IsPhaseOneToTwoLowHP()) {
            phaseOneToTwoTransitionTriggered = true;
            phaseTransition = BossPhaseTransition.PhaseOneToTwo;

            SetTeleportOffArena();
            stateMachine.ChangeState(teleportState);
            InterruptAllAttacks();
        }
    }

    private void HandlePhaseTwoToThreeTransition() {
        if (!phaseTwoToThreeTransitionTriggered && IsPhaseTwoToThreeLowHP()) {
            phaseTwoToThreeTransitionTriggered = true;
            phaseTransition = BossPhaseTransition.PhaseTwoToThree;

            SetTeleportOffArena();
            stateMachine.ChangeState(teleportState);
            InterruptAllAttacks();
        }
    }

    private bool IsPhaseOneToTwoLowHP() {
        return health.GetHPPercentage() <= phaseOneToTwoHPThreshold;
    }

    private bool IsPhaseTwoToThreeLowHP() {
        return health.GetHPPercentage() <= phaseTwoToThreeHPThreshold;
    }

    private void InterruptAllAttacks() {
        StopComboAttack();
        StopDashAttack();
        StopLightning();
        StopSummon();

        anim.SetBool("SpellCastPerformed", true);
        vfx.SetAttackAlert(false);
        ExitAttackWindow();
    }

    public float GetPhaseTransitionDuration() {
        return phaseTransitionDuration;
    }

    public EntityState GetPhaseTransitionNextState() {
        return summonState;
    }

    public void BeginPhaseTransition() {
        health.AddDamageImmunity(PhaseTransitionImmunitySource);
    }

    public void CompletePhaseTransition() {
        UpdatePhaseIndex();
    }

    public void EndPhaseTransition() {}

    private void UpdatePhaseIndex() {
        switch (phaseTransition) {
            case BossPhaseTransition.PhaseOneToTwo:
                phaseIndex = 1;
                break;
            case BossPhaseTransition.PhaseTwoToThree:
                phaseIndex = 2;
                break;
        }
    }

    public void CompleteSummon() {
        health.RemoveDamageImmunity(PhaseTransitionImmunitySource);
        SetTeleportInArena();
        stateMachine.ChangeState(teleportState);
    }

    private void SetTeleportOffArena() {
        float targetX = rightMoveBound + transitionOffstageOffset;
        Vector3 position = new Vector3(targetX, arenaGroundY, transform.position.z);
        SetTeleport(position, phaseTransitionState);
    }

    private void SetTeleportInArena() {
        float arenaCenterX = (leftMoveBound + rightMoveBound) / 2;
        Vector3 position = new Vector3(arenaCenterX, arenaGroundY, transform.position.z);
        SetTeleport(position, battleState);
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        if (stateMachine.currentState == energyWaveState) {
            StartEnergyWave();
        }

        if (stateMachine.currentState == lightningState) {
            StartLightning();
        }
    }

    public void EnterAttackStateWithDelay() {
        if (enterAttackStateWithDelayCo != null) {
            StopCoroutine(enterAttackStateWithDelayCo);
        }

        enterAttackStateWithDelayCo = StartCoroutine(EnterAtatckStateWithDelayCo());
    }

    private void StopComboAttack() {
        if (enterAttackStateWithDelayCo != null) {
            StopCoroutine(enterAttackStateWithDelayCo);
            enterAttackStateWithDelayCo = null;
        }
    }

    private IEnumerator EnterAtatckStateWithDelayCo() {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(attackState);
    }

    public void SetDashAttackDir(int dir) {
        dashAttackDir = dir;
    }

    public void EnterDashAttackStateWithDelay() {
        if (enterDashAttackStateWithDelayCo != null) {
            StopCoroutine(enterDashAttackStateWithDelayCo);
        }

        enterDashAttackStateWithDelayCo = StartCoroutine(EnterDashAttackStateWithDelayCo());
    }

    private void StopDashAttack() {
        if (enterDashAttackStateWithDelayCo != null) {
            StopCoroutine(enterDashAttackStateWithDelayCo);
            enterDashAttackStateWithDelayCo = null;
        }
    }

    private IEnumerator EnterDashAttackStateWithDelayCo() {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(dashAttackState);
    }

    public void SetEnergyWavePerformed(bool performed) {
        energyWavePerformed = performed;
    }

    private void StartEnergyWave() {
        sfx?.PlaySpecialAttack2();
        GameObject energyWave = 
            Instantiate(energyWavePrefab, energyWaveStartPoint.position, Quaternion.identity);
        energyWave.GetComponent<EnemyBossEnergyWave>().SetUpEnergyWave(
                facingDir * energyWaveSpeed, 
                combat
            );
        SetEnergyWavePerformed(true);
    }

    public void SetLightningPerformed(bool performed) {
        lightningPerformed = performed;
    }

    public void StartLightning() {
        if (lightningCo != null) {
            StopCoroutine(lightningCo);
        }

        lightningCo = StartCoroutine(LightningCo());
    }

    private void StopLightning() {
        if (lightningCo != null) {
            StopCoroutine(lightningCo);
            lightningCo = null;
        }
    }

    private IEnumerator LightningCo() {
        int patternIndex = -1;

        for (int i = 0; i < numOfLightningRound[phaseIndex]; i += 1) {
            patternIndex = GetLightningPatternIndex(patternIndex);
            LightningRound(patternIndex);
            yield return new WaitForSeconds(lightningInterval);
        }

        yield return new WaitForSeconds(lightningWarningDuration);

        SetLightningPerformed(true);
    }

    private void LightningRound(int patternIndex) {
        int[] dangerousRegions = EnemyBossLightningPattern.All[patternIndex];
        float minX = leftMoveBound + arenaMargin;
        float maxX = rightMoveBound - arenaMargin;
        float regionWidth = (maxX - minX) / EnemyBossLightningPattern.RegionCount;

        HashSet<Transform> hitTargets = new HashSet<Transform>();

        foreach (int regionIndex in dangerousRegions) {
            float regionCenterX = minX + regionWidth * (regionIndex + 0.5f);
            Vector3 position = new Vector3(regionCenterX, arenaGroundY + lightningOffsetY, 0);
            
            GameObject lightning = Instantiate(lightningPrefab, position, Quaternion.identity);

            Vector3 scale = lightning.transform.localScale;
            float lightningSizeX = lightning.GetComponent<BoxCollider2D>().size.x;
            scale.x = regionWidth * 0.98f / lightningSizeX;
            lightning.transform.localScale = scale;

            lightning.GetComponent<EnemyBossLightning>().SetUpLightning(
                    combat, 
                    hitTargets, 
                    lightningWarningDuration
                );
        }
    }

    private int GetLightningPatternIndex(int previousPatternIndex) {
        int patternCount = EnemyBossLightningPattern.All.Length;

        if (previousPatternIndex < 0 || patternCount <= 1) {
            return Random.Range(0, patternCount);
        }

        int patternIndex = Random.Range(0, patternCount - 1);
        return patternIndex >= previousPatternIndex ? patternIndex + 1 : patternIndex;
    }

    public void SetSummonPerformed(bool performed) {
        summonPerformed = performed;
    }

    public void StartSummon() {
        if (summonCo != null) {
            StopCoroutine(summonCo);
        }

        activeSummonedEnemies.Clear();
        summonSpawnCompleted = false;
        summonPerformed = false;
        summonCo = StartCoroutine(SummonCo());
    }

    private void StopSummon() {
        if (summonCo != null) {
            StopCoroutine(summonCo);
            summonCo = null;
        }
    }

    private IEnumerator SummonCo() {
        GameObject[] summonGroup = GetSummonGroup();

        for (int i = 0; i < summonGroup.Length; i += 1) {
            Vector3 summonPosition = GetSummonPosition(i, summonGroup.Length);
            GameObject summonedObject = 
                Instantiate(summonGroup[i], summonPosition, Quaternion.identity);
            Enemy summonedEnemy = summonedObject.GetComponent<Enemy>();

            activeSummonedEnemies.Add(summonedEnemy);
            bossUpgrade.ApplySummonIncrease(summonedEnemy);
            summonedEnemy.HandleFlip(DirectionToPlayer());
            summonedEnemy.TryToEnterBattleState(player);

            Color color = Color.red;
            vfx.GenerateParticleEffect(color);

            yield return new WaitForSeconds(summonCooldown);
        }

        summonSpawnCompleted = true;
        UpdateSummonCompletion();
    }

    private GameObject[] GetSummonGroup() {
        if (phaseTransition == BossPhaseTransition.PhaseOneToTwo) {
            return GetSummonPair();
        }

        if (phaseTransition == BossPhaseTransition.PhaseTwoToThree) {
            return GetSummonTriple();
        }

        return null;
    }

    private GameObject[] GetSummonTriple() {
        List<GameObject> candidates = new List<GameObject> 
            { knightPrefab, archerElfPrefab, skeletonPrefab, magePrefab, evilWizardPrefab };

        for (int i = 0; i < 3; i += 1) {
            int randomIndex = Random.Range(i, candidates.Count);
            GameObject selected = candidates[randomIndex];
            candidates[randomIndex] = candidates[i];
            candidates[i] = selected;
        }

        return candidates.GetRange(0, 3).ToArray();
    }

    private GameObject[] GetSummonPair() {
        if (bossUpgrade.RepeatLevel <= 0) {
            return new[] { knightPrefab, archerElfPrefab };
        }

        int repeatPairIndex = (bossUpgrade.RepeatLevel - 1) % 3;

        switch (repeatPairIndex) {
            case 0:
                return new[] { knightPrefab, archerElfPrefab };
            case 1:
                return new[] { skeletonPrefab, magePrefab };
            default:
                return new[] { knightPrefab, evilWizardPrefab };
        }
    }

    private void HandleSummonedEnemyKilled(Enemy killedEnemy) {
        if (!activeSummonedEnemies.Remove(killedEnemy)) {
            return;
        }

        UpdateSummonCompletion();
    }

    public void UpdateSummonCompletion() {
        activeSummonedEnemies.RemoveAll(summonedEnemy => summonedEnemy == null);
        summonPerformed = summonSpawnCompleted && activeSummonedEnemies.Count == 0;
    }

    private Vector3 GetSummonPosition(int index, int count) {
        float minX = leftMoveBound + arenaMargin;
        float maxX = rightMoveBound - arenaMargin;
        float t = (index + 1f) / (count + 1f);
        float x = Mathf.Lerp(minX, maxX, t);

        if (DistanceToPlayer() < minPlayerDistance) {
            float arenaCenterX = (minX + maxX) / 2;

            if (player.position.x < arenaCenterX) {
                x = player.position.x + minPlayerDistance;
            } else {
                x = player.position.x - minPlayerDistance;
            }
        }

        x = Mathf.Clamp(x, minX, maxX);
        return new Vector3(x, arenaGroundY, 0);
    }

    private int DirectionToPlayer() {
        if (player == null) {
            return 0;
        }

        return player.position.x > transform.position.x ? 1 : -1;
    }

    private float DistanceToPlayer() {
        if (player == null) {
            return float.MaxValue;
        }

        return Mathf.Abs(transform.position.x - player.position.x);
    }

    public void SetTeleport(Vector3 position, EnemyState nextState) {
        TeleportPosition = position;
        teleportNextState = nextState;
    }

    public void Teleport() {
        transform.position = TeleportPosition;
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
