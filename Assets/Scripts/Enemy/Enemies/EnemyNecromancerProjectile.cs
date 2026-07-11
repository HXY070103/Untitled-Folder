using UnityEngine;

public class EnemyNecromancerProjectile : MonoBehaviour, ICounterable, ISlowable {
    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;

    private EnemyCombat combat;

    private Transform target;
    private float speedMultiplier;
    private float chaseSpeed;
    private bool canChase;
    private bool useFixedDirection;
    private Vector2 fixedDirection;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatCanCollideWith;
    [SerializeField] private float destroyDelay = 2;
    [SerializeField] private GameObject destroyVFXPrefab;
    [SerializeField] private float waitingTime = 0.8f;

    public void SetUpProjectile(float chaseSpeed, Transform target, EnemyCombat combat) {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        this.target = target;
        this.combat = combat;

        speedMultiplier = 1;
        this.chaseSpeed = chaseSpeed;

        canChase = false;
        useFixedDirection = false;
        Invoke(nameof(StartChasing), waitingTime);

        anim.enabled = false;
    }

    public void SetUpProjectileDirection(float speed, Vector2 direction, EnemyCombat combat) {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        this.combat = combat;
        chaseSpeed = speed;
        speedMultiplier = 1;
        fixedDirection = direction.normalized;
        useFixedDirection = true;
        canChase = true;
        if (anim != null) {
            anim.enabled = false;
        }
    }

    private void Update() {
        if (canChase) {
            HandleChasing();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0) {
            combat.PerformAttackOnTarget(collision.transform);
        }

        if (((1 << collision.gameObject.layer) & whatCanCollideWith) != 0) {
            Explosion();
        }
    }

    private void Explosion() {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        anim.enabled = true;
        col.enabled = false;
        Destroy(gameObject, destroyDelay);
    }

    private void HandleChasing() {
        if (useFixedDirection) {
            rb.linearVelocity = fixedDirection * chaseSpeed * speedMultiplier;
            return;
        }

        if (target == null) {
            return;
        }
        Vector2 dir =(target.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * chaseSpeed * speedMultiplier;
    }

    private void StartChasing() {
        canChase = true;
    }

    public bool CanBeCountered() {
        return true;
    }

    public void HandleCounter() {
        Destroy(gameObject);
        Instantiate(destroyVFXPrefab, transform.position, Quaternion.identity);
    }

    public void SlowDown(float slowDownPercentage) {
        CancelInvoke(nameof(RecoverSpeed));

        speedMultiplier = 1 - slowDownPercentage;
    }

    public void RecoverSpeed() {
        speedMultiplier = 1;
    }

    public void RecoverSpeedWithDelay(float delay) {
        Invoke(nameof(RecoverSpeed), delay);
    }
}
