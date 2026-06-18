using System.Collections;
using UnityEngine;

public class EnemySkeletonKing : Enemy {
    public EnemySkeletonKingSummonState summonState;

    [Header("Skeleton king specific info")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private Transform summonStartPoint;
    [SerializeField] private int amountOfSkeletonToSummon = 3;
    [SerializeField] private float summonCooldown = 0.5f;
    public float healthThresholdForSummon = 0.5f;
    public float summonAttackCooldown = 5;
    public float summonAttackDistance = 10;

    private Coroutine summonCo;
    private bool summonPerformed;
    public bool SummonPerformed => summonPerformed;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        battleState = new EnemySkeletonKingBattleState(this, stateMachine, "Battle");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        summonState = new EnemySkeletonKingSummonState(this, stateMachine, "Idle");

        anim.SetBool("HasStunnedRecovery", false);
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
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

            Color color = Color.purple;
            vfx.GenerateParticleEffect(color);

            yield return new WaitForSeconds(summonCooldown);
        }

        SetSummonPerformed(true);
    }
}