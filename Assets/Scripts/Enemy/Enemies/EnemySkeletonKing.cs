using System.Collections;
using UnityEngine;

public class EnemySkeletonKing : Enemy {
    public EnemySkeletonKingShockwaveState shockwaveState;
    public EnemySkeletonKingSummonState summonState;

    [Header("Skeleton king specific info")]
    public float HPThreshold = 0.5f;

    [Header("Shockwave info")]
    [SerializeField] private float shockwavePrefab;
    [SerializeField] private Transform shockwaveStartPoint;
    [SerializeField] private float shockwaveDamage = 10;
    public float shockwaveWindup = 0.7f;
    public  float shockwaveCooldown = 2.5f;
    public int shockwaveIndex = 2;

    [Header("Summon info")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private Transform summonStartPoint;
    [SerializeField] private int amountOfSkeletonToSummon = 3;
    [SerializeField] private float summonCooldown = 0.5f;

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
        shockwaveState = new EnemySkeletonKingShockwaveState(this, stateMachine, "Shockwave");
        summonState = new EnemySkeletonKingSummonState(this, stateMachine, "Battle");

        anim.SetBool("HasStunnedRecovery", false);
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void SpecialAttack() {
        base.SpecialAttack();
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

            Color color = Color.purple;
            vfx.GenerateParticleEffect(color);

            yield return new WaitForSeconds(summonCooldown);
        }

        SetSummonPerformed(true);
    }
}