using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyBossPhase {
    PhaseOne,
    SummonTransition,
    PhaseTwo,
    PhaseThree
}

public enum BossPhaseTransitionTarget {
    Summon,
    PhaseThree
}

public enum EnemyBossPhaseOneAction {
    Combo,
    EnergyWave,
    HarvestDash,
    Recovery
}

public enum EnemyBossPhaseTwoAction {
    Combo,
    EnergyWave,
    RecoveryAfterEnergyWave,
    GroundFissure,
    HarvestDash,
    RecoveryAfterHarvestDash
}

public enum EnemyBossPhaseThreeAction {
    Combo,
    EnergyWave,
    RecoveryAfterEnergyWave,
    BidirectionalHarvest,
    DoubleGroundFissure,
    RecoveryAfterGroundFissure
}

public class EnemyBoss : Enemy, IEnemyPhaseTransition {
    private const string PhaseTransitionImmunitySource = "BossPhaseTransition";

    public EnemyPhaseTransitionState phaseTransitionState;
    public EnemyBossDashState dashState;
    public EnemyBossSpellCastState spellCastState;
    public EnemyBossSummonState summonState;
    public EnemyBossComboState comboState;
    public EnemyBossGroundFissureState groundFissureState;
    public EnemyBossBidirectionalHarvestState bidirectionalHarvestState;

    [Header("Phase info")]
    [Range(0, 1)]
    [SerializeField] private float summonPhaseHPThreshold = 0.7f;
    [Range(0, 1)]
    [SerializeField] private float phaseThreeHPThreshold = 0.35f;
    [FormerlySerializedAs("teleportDelay")]
    [SerializeField] private float phaseTransitionDuration = 1.2f;
    [SerializeField] private float transitionOffstageOffset = 5f;

    private BossPhaseTransitionTarget phaseTransitionTarget;
    private bool summonTransitionTriggered;
    private bool phaseThreeTransitionTriggered;

    public EnemyBossPhase CurrentPhase { get; private set; } = EnemyBossPhase.PhaseOne;
    public BossPhaseTransitionTarget PhaseTransitionTarget => phaseTransitionTarget;

    [Header("Dash info")]
    public float dashSpeed = 12;
    public float minDashDistance = 12;
    [Min(0.65f)] public float harvestDashWindup = 0.65f;
    [Min(0.1f)] public float harvestDashVisualLeadTime = 0.15f;
    [Min(0.05f)] public float harvestDashDamageDelay = 0.2f;
    [Min(0.1f)] public float harvestDashDuration = 0.55f;
    [Min(0)] public float phaseOneRecoveryDuration = 1f;
    [Min(0)] public float phaseTwoRecoveryDuration = 1f;
    [Min(0)] public float phaseThreeRecoveryDuration = 1f;

    [Header("Bidirectional harvest info")]
    [Min(0.1f)]
    public float bidirectionalHarvestPauseDuration = 0.45f;
    [Min(0.1f)]
    public float bidirectionalHarvestCounterStunDuration = 1.2f;

    [Header("Energy wave info")]
    [SerializeField] private GameObject energyWavePrefab;
    [SerializeField] private Transform energyWaveStartPoint;
    [SerializeField] private float energyWaveSpeed = 10;
    [SerializeField] private float energyWaveCooldown = 2;

    private bool energyWavePerformed;
    public bool EnergyWavePerformed => energyWavePerformed;

    [Header("Ground fissure info")]
    [FormerlySerializedAs("spellPrefab")]
    [SerializeField] private GameObject groundFissurePrefab;
    [Min(0.8f)]
    [SerializeField] private float groundFissureWarningDuration = 1.2f;
    [Min(0)]
    [SerializeField] private float groundFissureArenaMargin = 3f;
    [SerializeField] private float groundFissureVerticalOffset = 2.5f;
    [Min(0.1f)]
    [SerializeField] private float groundFissureRoundInterval = 0.7f;

    private Coroutine groundFissureCo;
    private readonly List<Coroutine> groundFissureRoundCos =
        new List<Coroutine>();
    private bool groundFissurePerformed;
    private int requestedGroundFissureRounds = 1;
    private readonly List<GameObject> activeGroundFissures =
        new List<GameObject>();
    public bool GroundFissurePerformed => groundFissurePerformed;

    [Header("Summon info")]
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject archerElfPrefab;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject evilWizardPrefab;
    [SerializeField] private float summonCooldown = 0.5f;

    private Coroutine summonCo;
    private EnemyBossUpgrade bossUpgrade;
    private bool summonPerformed;
    private bool summonSpawnCompleted;
    private readonly List<Enemy> activeSummonedEnemies = new List<Enemy>();
    public bool SummonPerformed => summonPerformed;

    private float leftMoveBound;
    private float rightMoveBound;
    private float arenaGroundY;
    private readonly List<GameObject> activeBossAttackObjects = new List<GameObject>();
    private readonly Dictionary<GameObject, int> phaseTransitionLayers =
        new Dictionary<GameObject, int>();

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyBossBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        comboState = new EnemyBossComboState(this, stateMachine, "Attack");
        attackState = comboState;
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        phaseTransitionState = new EnemyPhaseTransitionState(this, stateMachine, "Teleport");
        dashState = new EnemyBossDashState(this, stateMachine, "Battle");
        spellCastState = new EnemyBossSpellCastState(this, stateMachine, "SpellCast");
        summonState = new EnemyBossSummonState(this, stateMachine, "Battle");
        groundFissureState = new EnemyBossGroundFissureState(
            this,
            stateMachine,
            "Battle"
        );
        bidirectionalHarvestState =
            new EnemyBossBidirectionalHarvestState(
                this,
                stateMachine,
                "Battle"
            );
        bossUpgrade = GetComponent<EnemyBossUpgrade>();

        anim.SetBool("HasStunnedRecovery", false);
    }

    protected override void Start() {
        base.Start();

        leftMoveBound = transform.position.x - playerCheckDistance;
        rightMoveBound = transform.position.x + playerCheckDistance;
        arenaGroundY = transform.position.y;

        stateMachine.Initialize(idleState);
        health.OnHealthUpdate += HandleHealthUpdate;
        EnemyHealth.OnEnemyKilled += HandleSummonedEnemyKilled;
        TryStartPhaseTransition();
    }

    protected override void Update() {
        base.Update();
    }

    public override void EntityDeath() {
        CancelActiveBossAttacks();
        ClearAttackPresentation();
        base.EntityDeath();
    }

    private void OnDestroy() {
        if (health != null) {
            health.OnHealthUpdate -= HandleHealthUpdate;
        }

        EnemyHealth.OnEnemyKilled -= HandleSummonedEnemyKilled;
    }

    private void HandleHealthUpdate() {
        TryStartPhaseTransition();
    }

    public override void HandleCounter() {
        if (!CanBeCountered()) {
            return;
        }

        if (stateMachine.currentState == bidirectionalHarvestState
            && bidirectionalHarvestState.IsSecondDashCounterable) {
            Debug.Log("Boss bidirectional harvest second dash countered");
            bidirectionalHarvestState.CancelForCounter();
            CompleteBidirectionalHarvestAction();

            float defaultStunnedDuration = stunnedDuration;
            stunnedDuration =
                bidirectionalHarvestCounterStunDuration;
            base.HandleCounter();
            stunnedDuration = defaultStunnedDuration;
            return;
        }

        Debug.Log(
            $"Boss combo countered on hit " +
            $"{comboState.CurrentAttackIndex + 1}/{comboState.ComboLength}"
        );
        comboState.CancelCombo();
        base.HandleCounter();
    }

    public void PerformCurrentComboAttack() {
        comboState.PerformCurrentHit();
    }

    public bool IsComboAttackActive() {
        return stateMachine.currentState == comboState;
    }

    public bool IsBidirectionalHarvestActive() {
        return stateMachine.currentState == bidirectionalHarvestState;
    }

    public bool IsBidirectionalHarvestCounterWindowActive() {
        return IsBidirectionalHarvestActive()
            && bidirectionalHarvestState.IsSecondDashCounterable;
    }

    public void CompletePhaseOneAction(EnemyBossPhaseOneAction completedAction) {
        if (CurrentPhase != EnemyBossPhase.PhaseOne) {
            return;
        }

        ((EnemyBossBattleState)battleState).CompletePhaseOneAction(
            completedAction
        );
    }

    public void CompleteComboAction() {
        ((EnemyBossBattleState)battleState).CompleteComboAction();
    }

    public void CompleteEnergyWaveAction() {
        ((EnemyBossBattleState)battleState).CompleteEnergyWaveAction();
    }

    public void CompleteHarvestDashAction() {
        ((EnemyBossBattleState)battleState).CompleteHarvestDashAction();
    }

    public void CompleteGroundFissureAction() {
        ((EnemyBossBattleState)battleState).CompleteGroundFissureAction();
    }

    public void CompleteBidirectionalHarvestAction() {
        ((EnemyBossBattleState)battleState)
            .CompleteBidirectionalHarvestAction();
    }

    private bool TryStartPhaseTransition() {
        if (health.GetCurrentHP() <= 0 || stateMachine.currentState == deadState) {
            return false;
        }

        if (CurrentPhase == EnemyBossPhase.PhaseOne
            && !summonTransitionTriggered
            && health.GetHPPercentage() <= summonPhaseHPThreshold) {
            summonTransitionTriggered = true;
            CurrentPhase = EnemyBossPhase.SummonTransition;
            StartPhaseTransition(BossPhaseTransitionTarget.Summon);
            return true;
        }

        if (CurrentPhase == EnemyBossPhase.PhaseTwo
            && !phaseThreeTransitionTriggered
            && health.GetHPPercentage() <= phaseThreeHPThreshold) {
            phaseThreeTransitionTriggered = true;
            StartPhaseTransition(BossPhaseTransitionTarget.PhaseThree);
            return true;
        }

        return false;
    }

    private void StartPhaseTransition(BossPhaseTransitionTarget target) {
        phaseTransitionTarget = target;
        Debug.Log($"Boss phase transition started: {target}");
        stateMachine.ChangeState(phaseTransitionState);
    }

    public float GetPhaseTransitionDuration() {
        return phaseTransitionDuration;
    }

    public EntityState GetPhaseTransitionNextState() {
        return summonState;
    }

    public void BeginPhaseTransition() {
        CancelActiveBossAttacks();
        MoveRaw(0, 0);
        ClearAttackPresentation();
        vfx.DoFadeEffect();
        health.AddDamageImmunity(PhaseTransitionImmunitySource);
        SetBossUntargetable();
    }

    public void CompletePhaseTransition() {
        TeleportForPhaseTransition();
    }

    public void EndPhaseTransition() {
        // Both transitions now continue into an offstage summon phase.
    }

    public void CompleteSummonPhase() {
        bool enteringPhaseThree =
            phaseTransitionTarget
            == BossPhaseTransitionTarget.PhaseThree;
        CurrentPhase = enteringPhaseThree
            ? EnemyBossPhase.PhaseThree
            : EnemyBossPhase.PhaseTwo;
        Debug.Log(
            $"Boss phase entered: {CurrentPhase}"
        );
        transform.position = new Vector3(
            (leftMoveBound + rightMoveBound) / 2f,
            arenaGroundY,
            transform.position.z
        );
        RestoreAfterPhaseTransition();

        if (enteringPhaseThree) {
            stateMachine.ChangeState(battleState);
            return;
        }

        if (!TryStartPhaseTransition()) {
            stateMachine.ChangeState(battleState);
        }
    }

    private void RestoreAfterPhaseTransition() {
        MoveRaw(0, 0);
        health.RemoveDamageImmunity(PhaseTransitionImmunitySource);
        RestoreBossLayers();
        vfx.ResetTransparency();
        ResetPhaseTransitionAnimation();
        Debug.Log(
            $"Boss phase restored: layer {gameObject.layer}, " +
            $"damage immune {health.IsDamageImmune()}, " +
            $"position {transform.position}"
        );

        if (player != null) {
            HandleFlip(player.position.x > transform.position.x ? 1 : -1);
        }
    }

    private void SetBossUntargetable() {
        phaseTransitionLayers.Clear();
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int untargetableLayer = LayerMask.NameToLayer("Untargetable");

        foreach (Transform child in GetComponentsInChildren<Transform>(true)) {
            GameObject childObject = child.gameObject;

            if (childObject.layer != enemyLayer) {
                continue;
            }

            phaseTransitionLayers[childObject] = childObject.layer;
            childObject.layer = untargetableLayer;
        }
    }

    private void RestoreBossLayers() {
        foreach (KeyValuePair<GameObject, int> layerEntry in phaseTransitionLayers) {
            if (layerEntry.Key != null) {
                layerEntry.Key.layer = layerEntry.Value;
            }
        }

        phaseTransitionLayers.Clear();
    }

    private void ResetPhaseTransitionAnimation() {
        anim.SetBool("TeleportPerformed", false);
        anim.SetBool("Teleport", false);
        anim.Play("BossBattle-IdleMove", 0, 0f);
    }

    private void CancelActiveBossAttacks() {
        CancelInvoke(nameof(OnEnergyWaveFinished));
        CancelGroundFissure();

        if (summonCo != null) {
            StopCoroutine(summonCo);
            summonCo = null;
        }

        foreach (GameObject attackObject in activeBossAttackObjects) {
            if (attackObject != null) {
                Destroy(attackObject);
            }
        }

        activeBossAttackObjects.Clear();
        energyWavePerformed = false;
        summonPerformed = false;
    }

    private void TrackBossAttackObject(GameObject attackObject) {
        if (attackObject != null) {
            activeBossAttackObjects.Add(attackObject);
        }
    }

    private void TeleportForPhaseTransition() {
        float targetX =
            rightMoveBound + transitionOffstageOffset;

        transform.position = new Vector3(
            targetX,
            arenaGroundY,
            transform.position.z
        );
    }

    public override void SpecialAttack() {
        StartEnergyWave();
    }

    public void SetEnergyWavePerformed(bool performed) {
        energyWavePerformed = performed;
    }

    private void StartEnergyWave() {
        sfx?.PlaySpecialAttack2();
        GameObject energyWave = 
            Instantiate(energyWavePrefab, energyWaveStartPoint.position, Quaternion.identity);
        TrackBossAttackObject(energyWave);
        energyWave.GetComponent<EnemyBossEnergyWave>().SetUpEnergyWave(
            facingDir * energyWaveSpeed,
            combat,
            14f / 18f
        );
        Invoke(nameof(OnEnergyWaveFinished), energyWaveCooldown);
    }

    private void OnEnergyWaveFinished() {
        SetEnergyWavePerformed(true);
    }

    public void StartGroundFissure(int roundCount = 1) {
        CancelGroundFissure();
        requestedGroundFissureRounds = Mathf.Max(1, roundCount);
        groundFissurePerformed = false;
        groundFissureCo = StartCoroutine(GroundFissureCo());
    }

    public void CancelGroundFissure() {
        if (groundFissureCo != null) {
            StopCoroutine(groundFissureCo);
            groundFissureCo = null;
        }

        foreach (Coroutine roundCoroutine in groundFissureRoundCos) {
            if (roundCoroutine != null) {
                StopCoroutine(roundCoroutine);
            }
        }

        groundFissureRoundCos.Clear();

        foreach (GameObject fissure in activeGroundFissures) {
            if (fissure != null) {
                Destroy(fissure);
            }
        }

        activeGroundFissures.Clear();
        groundFissurePerformed = false;
    }

    private IEnumerator GroundFissureCo() {
        int previousPatternIndex = -1;

        for (
            int roundIndex = 0;
            roundIndex < requestedGroundFissureRounds;
            roundIndex += 1
        ) {
            int patternIndex = GetGroundFissurePatternIndex(
                previousPatternIndex
            );
            previousPatternIndex = patternIndex;
            Coroutine roundCoroutine = StartCoroutine(
                GroundFissureRoundCo(
                    patternIndex,
                    roundIndex + 1,
                    requestedGroundFissureRounds
                )
            );
            groundFissureRoundCos.Add(roundCoroutine);

            if (roundIndex + 1 < requestedGroundFissureRounds) {
                yield return new WaitForSeconds(
                    groundFissureRoundInterval
                );
            }
        }

        yield return new WaitForSeconds(
            groundFissureWarningDuration + 0.55f
        );

        groundFissureCo = null;
        groundFissureRoundCos.Clear();
        activeGroundFissures.Clear();
        groundFissurePerformed = true;
    }

    private IEnumerator GroundFissureRoundCo(
        int patternIndex,
        int roundNumber,
        int totalRounds
    ) {
        const float lightningAttackEventTime = 0.3f;
        const float regionWidthPadding = 0.98f;

        int[] dangerousRegions =
            EnemyBossGroundFissurePatterns.All[patternIndex];
        float minX = leftMoveBound + groundFissureArenaMargin;
        float maxX = rightMoveBound - groundFissureArenaMargin;
        float regionWidth =
            (maxX - minX) / EnemyBossGroundFissurePatterns.RegionCount;
        HashSet<Transform> roundHitTargets = new HashSet<Transform>();
        List<Animator> fissureAnimators = new List<Animator>();

        Debug.Log(
            $"Boss ground fissure round {roundNumber}/{totalRounds}: " +
            $"{string.Join("+", dangerousRegions)}"
        );

        foreach (int regionIndex in dangerousRegions) {
            float regionCenterX =
                minX + regionWidth * (regionIndex + 0.5f);
            Vector3 position = new Vector3(
                regionCenterX,
                arenaGroundY + groundFissureVerticalOffset,
                0
            );
            GameObject fissure = Instantiate(
                groundFissurePrefab,
                position,
                Quaternion.identity
            );
            activeGroundFissures.Add(fissure);
            TrackBossAttackObject(fissure);

            BoxCollider2D fissureCollider =
                fissure.GetComponent<BoxCollider2D>();

            if (fissureCollider != null
                && fissureCollider.size.x > 0) {
                Vector3 scale = fissure.transform.localScale;
                scale.x =
                    regionWidth * regionWidthPadding
                    / fissureCollider.size.x;
                fissure.transform.localScale = scale;
            }

            EnemyBossLightning lightning =
                fissure.GetComponent<EnemyBossLightning>();

            if (lightning == null) {
                Debug.LogError(
                    "Ground fissure prefab must contain " +
                    "EnemyBossLightning.",
                    fissure
                );
                continue;
            }

            lightning.SetUpLightning(
                combat,
                15f / 18f,
                roundHitTargets
            );

            Animator fissureAnimator =
                fissure.GetComponentInChildren<Animator>();

            if (fissureAnimator != null) {
                fissureAnimator.speed = 0;
                fissureAnimators.Add(fissureAnimator);
            }
        }

        float freezeDuration = Mathf.Max(
            0,
            groundFissureWarningDuration - lightningAttackEventTime
        );
        yield return new WaitForSeconds(freezeDuration);

        foreach (Animator fissureAnimator in fissureAnimators) {
            if (fissureAnimator != null) {
                fissureAnimator.speed = 1;
            }
        }
    }

    private int GetGroundFissurePatternIndex(
        int previousPatternIndex
    ) {
        int patternCount =
            EnemyBossGroundFissurePatterns.All.Length;

        if (previousPatternIndex < 0 || patternCount <= 1) {
            return Random.Range(0, patternCount);
        }

        int patternIndex = Random.Range(0, patternCount - 1);
        return patternIndex >= previousPatternIndex
            ? patternIndex + 1
            : patternIndex;
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

    private IEnumerator SummonCo() {
        GameObject[] summonGroup = GetSummonGroup();
        int repeatLevel = bossUpgrade == null
            ? 0
            : bossUpgrade.RepeatLevel;
        string[] summonNames =
            new string[summonGroup.Length];

        for (int i = 0; i < summonGroup.Length; i += 1) {
            summonNames[i] = summonGroup[i].name;
        }

        Debug.Log(
            $"Boss summon group for {phaseTransitionTarget} " +
            $"at repeat {repeatLevel}: " +
            $"{string.Join(" + ", summonNames)}"
        );

        for (int i = 0; i < summonGroup.Length; i += 1) {
            Vector3 summonPosition = GetArenaSummonPosition(
                i,
                summonGroup.Length
            );
            GameObject summonedObject = Instantiate(
                summonGroup[i],
                summonPosition,
                Quaternion.identity
            );
            Enemy summonedEnemy = summonedObject.GetComponent<Enemy>();

            if (summonedEnemy == null) {
                Debug.LogError(
                    $"{summonGroup[i].name} must contain an Enemy component.",
                    summonedObject
                );
                Destroy(summonedObject);
                continue;
            }

            activeSummonedEnemies.Add(summonedEnemy);
            bossUpgrade?.ApplySummonIncrease(summonedEnemy);

            int directionToPlayer =
                player != null && player.position.x < summonPosition.x
                    ? -1
                    : 1;
            summonedEnemy.HandleFlip(directionToPlayer);

            Color color = Color.red;
            vfx.GenerateParticleEffect(color);

            yield return null;

            if (summonedEnemy != null && player != null) {
                summonedEnemy.TryToEnterBattleState(player);
            }

            yield return new WaitForSeconds(summonCooldown);
        }

        summonCo = null;
        summonSpawnCompleted = true;
        RefreshSummonCompletion();
    }

    private GameObject[] GetSummonGroup() {
        if (phaseTransitionTarget
            == BossPhaseTransitionTarget.PhaseThree) {
            return GetRandomPhaseThreeSummons();
        }

        return GetSummonPair();
    }

    private GameObject[] GetRandomPhaseThreeSummons() {
        const int summonCount = 3;

        List<GameObject> candidates = new List<GameObject> {
            knightPrefab,
            archerElfPrefab,
            skeletonPrefab,
            magePrefab,
            evilWizardPrefab
        };
        candidates.RemoveAll(candidate => candidate == null);

        if (candidates.Count < summonCount) {
            Debug.LogError(
                "Phase three requires at least three summon prefabs.",
                this
            );
            return candidates.ToArray();
        }

        for (int i = 0; i < summonCount; i += 1) {
            int randomIndex = Random.Range(i, candidates.Count);
            GameObject selected = candidates[randomIndex];
            candidates[randomIndex] = candidates[i];
            candidates[i] = selected;
        }

        return candidates.GetRange(0, summonCount).ToArray();
    }

    private GameObject[] GetSummonPair() {
        int repeatLevel = bossUpgrade == null
            ? 0
            : bossUpgrade.RepeatLevel;

        if (repeatLevel <= 0) {
            return new[] { knightPrefab, archerElfPrefab };
        }

        int repeatPairIndex = (repeatLevel - 1) % 3;

        switch (repeatPairIndex) {
            case 0:
                return new[] { knightPrefab, archerElfPrefab };
            case 1:
                return new[] { skeletonPrefab, magePrefab };
            default:
                return new[] { knightPrefab, evilWizardPrefab };
        }
    }

    public void RefreshSummonCompletion() {
        activeSummonedEnemies.RemoveAll(summonedEnemy => summonedEnemy == null);
        summonPerformed = summonSpawnCompleted && activeSummonedEnemies.Count == 0;
    }

    private void HandleSummonedEnemyKilled(Enemy killedEnemy) {
        if (!activeSummonedEnemies.Remove(killedEnemy)) {
            return;
        }

        Debug.Log(
            $"Boss summons remaining: {activeSummonedEnemies.Count}"
        );
        RefreshSummonCompletion();
    }

    private Vector3 GetArenaSummonPosition(int index, int count) {
        const float arenaMargin = 3f;
        const float minPlayerDistance = 3f;

        float minX = leftMoveBound + arenaMargin;
        float maxX = rightMoveBound - arenaMargin;
        float t = (index + 1f) / (count + 1f);
        float x = Mathf.Lerp(minX, maxX, t);

        if (player != null && Mathf.Abs(x - player.position.x) < minPlayerDistance) {
            float rightCandidate = player.position.x + minPlayerDistance;
            float leftCandidate = player.position.x - minPlayerDistance;
            x = rightCandidate <= maxX ? rightCandidate : leftCandidate;
        }

        x = Mathf.Clamp(x, minX, maxX);
        return new Vector3(x, arenaGroundY, 0);
    }

    public void Teleport() {
        if (stateMachine.currentState == phaseTransitionState) {
            TeleportForPhaseTransition();
        }
    }

    public void ClearAttackPresentation() {
        ExitAttackWindow();
        vfx.SetAttackAlert(false);
        vfx.StopImageEchoEffect();
        anim.SetBool("Attack", false);
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
