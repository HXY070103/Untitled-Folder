using System.Collections;
using UnityEngine;

public class EnemyMage : Enemy {
    public EnemyMageRetreatState retreatState;
    public EnemyMageSpellCastState spellCastState;

    [Header("Mage specific info")]

    [Header("Spell cast info")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellStartPoint;
    [SerializeField] private int amountToCast = 3;
    [SerializeField] private float spellCastCooldown = 0.3f;
    
    [Header("Retreat info")]
    [SerializeField] private Transform backCollisionCheck;
    [SerializeField] private float backCollisionDistance;
    public float retreatCooldown = 5;
    public float retreatMaxDistance = 8;
    public float retreatSpeed = 15;

    private Coroutine spellCastCo;
    private bool spellCastPerformed;
    public bool SpellCastPerformed => spellCastPerformed;

    public float RetreatSpeed => retreatSpeed * LocalTimeScale;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        battleState = new EnemyMageBattleState(this, stateMachine, "Battle");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        retreatState = new EnemyMageRetreatState(this, stateMachine, "Battle");
        spellCastState = new EnemyMageSpellCastState(this, stateMachine, "SpellCast");

        anim.SetBool("HasStunnedRecovery", true);
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void SpecialAttack() {
        base.SpecialAttack();

        StartSpellCast();
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
            GameObject projectile = 
                Instantiate(spellPrefab, spellStartPoint.position, Quaternion.identity);
            projectile.GetComponent<EnemyMageProjectile>().SetUpProjectile(player.transform, combat);
            yield return new WaitForSeconds(spellCastCooldown);
        }

        SetSpellCastPerformed(true);
    }

    public bool CannotMoveBackwards() {
        bool backWallDetected = Physics2D.Raycast(
                backCollisionCheck.position, 
                Vector2.right, 
                -facingDir * backCollisionDistance, 
                whatIsGround
            );

        bool backGroundDetected = Physics2D.Raycast(
                backCollisionCheck.position, 
                Vector2.down, 
                backCollisionDistance, 
                whatIsGround
            );

        return backWallDetected || !backGroundDetected;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.DrawLine(
                backCollisionCheck.position,
                backCollisionCheck.position + new Vector3(-facingDir * backCollisionDistance, 0)
            );

        Gizmos.DrawLine(
                backCollisionCheck.position,
                backCollisionCheck.position + new Vector3(0, -backCollisionDistance)
            );
    }
}