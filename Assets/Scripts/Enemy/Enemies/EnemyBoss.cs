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

    public float specialAttackTimer;

    public float DashSpeed => dashSpeed * LocalTimeScale;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        battleState = new EnemyBossBattleState(this, stateMachine, "Battle");
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

        specialAttackTimer = 0;
    }

    protected override void Start() {
        base.Start();

        leftMoveBound = transform.position.x - playerCheckDistance;
        rightMoveBound = transform.position.x + playerCheckDistance;

        stateMachine.Initialize(idleState);
    }

    protected override void Update() {
        base.Update();

        specialAttackTimer -= Time.deltaTime * LocalTimeScale;
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        spellCastActions[Random.Range(0, spellCastActions.Count)]();
    }

    public void SetSpellCastPerformed(bool performed) {
        spellCastPerformed = performed;
    }

    private void StartEnergyWave() {
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
        for (int i = 0; i < amountOfSkeletonToSummon; i += 1) {
            GameObject skeleton = 
                Instantiate(skeletonPrefab, summonStartPoint.position, Quaternion.identity);
            skeleton.GetComponent<EnemySkeleton>().HandleFlip(facingDir);
            skeleton.GetComponent<EnemySkeleton>().TryToEnterBattleState(player);

            Color color = Color.red;
            vfx.GenerateParticleEffect(color);

            yield return new WaitForSeconds(summonCooldown);
        }

        SetSummonPerformed(true);
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
