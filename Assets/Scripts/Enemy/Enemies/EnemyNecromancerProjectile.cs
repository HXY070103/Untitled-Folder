using UnityEngine;

public class EnemyNecromancerProjectile : MonoBehaviour, ICounterable, ISlowable {
    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;

    private EnemyCombat combat;

    private Transform target;
    private float speedMultiplier;
    private Vector2 dir;
    private float chaseSpeed;
    private bool canChase;
    private bool isTracking;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatCanCollideWith;
    [SerializeField] private float destroyDelay = 2;
    [SerializeField] private GameObject destroyVFXPrefab;
    [SerializeField] private float waitingTime = 0.8f;

    public void SetUpProjectile(Vector2 dir, float chaseSpeed, Transform target, EnemyCombat combat, bool isTracking) {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        this.target = target;
        this.combat = combat;

        speedMultiplier = 1;
        this.dir = dir;
        this.chaseSpeed = chaseSpeed;

        canChase = false;
        this.isTracking = isTracking;
        Invoke(nameof(StartMoving), waitingTime);

        anim.enabled = false;
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
        Vector2 dir =(target.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * chaseSpeed * speedMultiplier;
    }

    private void StartMoving() {
        rb.linearVelocity = dir * chaseSpeed;

        if (isTracking) {
            canChase = true;
        }
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
