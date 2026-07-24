using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNecromancer : Enemy, IEnemyPhaseTransition {
    public EnemyPhaseTransitionState phaseTransitionState;
    public EnemyNecromancerSpellCastState spellCastState;

    [Header("Necromancer specific info")]
    public int numOfAttack = 3;
    public float phaseTwoHPThreshold = 0.5f;
    public float phaseTransitionDuration = 2;

    public bool isPhaseTwo { get; private set; } = false;
    public int phaseIndex { get; private set; } = 0;
    private bool phaseTransitionTriggered = false;

    [Header("Position info")]
    public List<Transform> positions = new List<Transform>();
    public float xOffset = 3;
    public float yOffset = 0.5f;
    public float switchPositionCooldown = 10;

    [Header("Fire info")]
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private Transform fireStartPoint;
    [SerializeField] private int[] numOfFire = {1, 6};

    private Vector3 targetPosition;

    [Header("Spell cast info")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellStartPoint;
    [SerializeField] private float spellGenerationRadius = 0.5f;
    [SerializeField] private int[] numOfTrackingProjectile = {3, 4};
    [SerializeField] private int[] numOfBasicProjectile = {8, 16};
    [SerializeField] private float trackingSpellCastInterval = 0.3f;
    [SerializeField] private float basicSpellCastInterval = 0.1f;
    [SerializeField] private float spellChaseSpeed = 10;
    public int numOfSpellCast = 3;
    public float spellCastWindup = 1;
    public float spellCastCooldown = 3;

    private Coroutine spellCastCo;
    private bool isTracking;
    private bool spellCastPerformed;
    public bool SpellCastPerformed => spellCastPerformed;


    [Header("Summon info")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject archerElfPrefab;
    [SerializeField] private Transform summonStartPoint;
    [SerializeField] private float summonInterval = 0.5f;

    private Coroutine summonCo;
    private PlatformManager platformManager;
    private bool summonRegistrationActive;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyNecromancerBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyNecromancerPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        phaseTransitionState = new EnemyPhaseTransitionState(this, stateMachine, "Battle");
        spellCastState = new EnemyNecromancerSpellCastState(this, stateMachine, "SpellCast");

        anim.SetBool("HasStunnedRecovery", true);
    }

    protected override void Start() {
        base.Start();

        platformManager = FindAnyObjectByType<PlatformManager>();
        stateMachine.Initialize(idleState);
        health.OnHealthUpdate += TryStartPhaseTransition;
        TryStartPhaseTransition();

        rb.gravityScale = 0;
    }

    protected override void Update() {
        base.Update();
    }

    public float GetPhaseTransitionDuration() {
        return phaseTransitionDuration;
    }

    public EntityState GetPhaseTransitionNextState() {
        return battleState;
    }
        
    public void BeginPhaseTransition() {
        MoveRaw(0, 0);
        vfx.DoFadeEffect();
        gameObject.layer = LayerMask.NameToLayer("Untargetable");
    }

    public void CompletePhaseTransition() {
        isPhaseTwo = true;
        phaseIndex = 1;
    }

    public void EndPhaseTransition() {
        vfx.ResetTransparency();
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    private void TryStartPhaseTransition() {
        if (health.GetCurrentHP() <= 0 ||  stateMachine.currentState == deadState) {
            return;
        }

        if (phaseTransitionTriggered || !IsLowHP()) {
            return;
        }

        StopSpellCast();
        anim.SetBool("SpellCastPerformed", true);

        phaseTransitionTriggered = true;
        stateMachine.ChangeState(phaseTransitionState);
    }

    private bool IsLowHP() {
        return health.GetHPPercentage() < phaseTwoHPThreshold;
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

    public void UpdateTargetPosition() {
        targetPosition = player.position;
    }

    private void GenerateFire() {
        float baseAngle = CalculateAngleToTarget();
        
        for (int i = 0; i < numOfFire[phaseIndex]; i += 1) {
            float angle = baseAngle + i * 360f / numOfFire[phaseIndex];
            GameObject fire = Instantiate(firePrefab, fireStartPoint.position, Quaternion.Euler(0, 0, angle));
            fire.GetComponent<EnemyNecromancerFire>().SetUpFire(combat);
        }
    }

    private float CalculateAngleToTarget() {
        Vector2 dir = targetPosition - fireStartPoint.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public void SetIsTraking(bool isTracking) {
        this.isTracking = isTracking;
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

    private void StopSpellCast() {
        if (spellCastCo != null) {
            StopCoroutine(spellCastCo);
        }
    }

    private IEnumerator SpellCastCo() {
        int numOfProjectile = 
            isTracking ? numOfTrackingProjectile[phaseIndex] : numOfBasicProjectile[phaseIndex];

        for (int i = 0; i < numOfProjectile; i += 1) {
            Vector2 dir = CalculateDirection(i, numOfProjectile);
            Vector3 position = CalculatePosition(dir);
            Quaternion rotation = isTracking ? CalculateRotation(dir) : Quaternion.identity;

            GameObject projectile = Instantiate(spellPrefab, position, rotation);
            projectile.GetComponent<EnemyNecromancerProjectile>().SetUpProjectile(dir, spellChaseSpeed, player, combat, isTracking);

            float spellCastInterval = isTracking ? trackingSpellCastInterval : basicSpellCastInterval;
            yield return new WaitForSeconds(spellCastInterval);
        }

        SetSpellCastPerformed(true);
    }

    private Vector2 CalculateDirection(int i, int numOfProjectile) {
        float angle = i * 360f / numOfProjectile;
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    private Vector3 CalculatePosition(Vector2 dir) {
        Vector3 offset = dir * spellGenerationRadius;
        return spellStartPoint.position + offset;
    }

    private Quaternion CalculateRotation(Vector2 dir) {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle);
    }

    public void StartSummon() {
        if (summonCo != null) {
            StopCoroutine(summonCo);
            CompleteSummonRegistration();
        }

        platformManager?.BeginSummon();
        summonRegistrationActive = true;
        summonCo = StartCoroutine(SummonCo());
    }

    private IEnumerator SummonCo() {
        GameObject skeleton = Instantiate(skeletonPrefab, summonStartPoint.position, Quaternion.identity);
        EnemySkeleton skeletonEnemy = skeleton.GetComponent<EnemySkeleton>();
        skeletonEnemy.HandleFlip(facingDir);
        skeletonEnemy.TryToEnterBattleState(player);
        platformManager?.RegisterSummonedEnemy(skeletonEnemy);
        vfx.GenerateParticleEffect(Color.purple);

        yield return new WaitForSeconds(summonInterval);

        GameObject archerElf = Instantiate(archerElfPrefab, summonStartPoint.position, Quaternion.identity);
        EnemyArcherElf archerElfEnemy = archerElf.GetComponent<EnemyArcherElf>();
        archerElfEnemy.HandleFlip(facingDir);
        archerElfEnemy.TryToEnterBattleState(player);
        platformManager?.RegisterSummonedEnemy(archerElfEnemy);
        vfx.GenerateParticleEffect(Color.purple);

        CompleteSummonRegistration();
        summonCo = null;
    }

    private void CompleteSummonRegistration() {
        if (!summonRegistrationActive) {
            return;
        }

        summonRegistrationActive = false;
        platformManager?.CompleteSummon();
    }

    public override void EntityDeath() {
        CompleteSummonRegistration();
        base.EntityDeath();
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
