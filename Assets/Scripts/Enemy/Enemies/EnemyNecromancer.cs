using System.Collections;
using UnityEngine;

public class EnemyNecromancer : Enemy {
    public EnemyNecromancerSpellCastState spellCastState;

    [Header("Necromancer specific info")]
    
    [Header("Fire info")]
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private Transform fireStartPoint;

    [Header("Spell cast info")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellStartPoint;
    [SerializeField] private float spellGenerationRadius = 0.5f;
    [SerializeField] private int amountToCast = 3;
    [SerializeField] private float spellCastCooldown = 0.3f;
    [SerializeField] private float spellChaseSpeed = 10;
    public float spellCastAttackCooldown = 5;

    private Coroutine spellCastCo;
    private bool spellCastPerformed;
    public bool SpellCastPerformed => spellCastPerformed;

    public float spellCastTimer;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        battleState = new EnemyNecromancerBattleState(this, stateMachine, "Battle");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");
        spellCastState = new EnemyNecromancerSpellCastState(this, stateMachine, "SpellCast");

        anim.SetBool("HasStunnedRecovery", true);

        spellCastTimer = 0;
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update() {
        base.Update();

        spellCastTimer -= Time.deltaTime * LocalTimeScale;
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
        GameObject fire = 
            Instantiate(firePrefab, fireStartPoint.position, Quaternion.identity);
        fire.GetComponent<EnemyNecromancerFire>().SetUpFire(combat);
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
            yield return new WaitForSeconds(spellCastCooldown);
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
}
