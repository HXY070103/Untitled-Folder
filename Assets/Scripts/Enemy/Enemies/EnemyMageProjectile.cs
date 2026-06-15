using UnityEngine;

public class EnemyMageProjectile : MonoBehaviour, ICounterable, ISlowable {
    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;

    private EntityCombat combat;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatCanCollideWith;
    [SerializeField] private float arcHeight = 2;
    [SerializeField] private float destroyDelay = 2;
    [SerializeField] private GameObject destroyVFXPrefab;

    private float slowDownPercentage;

    public void SetUpProjectile(Transform target, EntityCombat combat) {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        this.combat = combat;

        anim.enabled = false;
        rb.linearVelocity = CalculateBallisticVelocity(transform.position, target.position);
    }

    private Vector2 CalculateBallisticVelocity(Vector2 start, Vector2 end) {
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        float xDisplacement = end.x - start.x;
        float yDisplacement = end.y - start.y;
        float peakHeight = Mathf.Max(arcHeight, end.y - start.y + 0.1f);

        float timeToPeak = Mathf.Sqrt(2 * peakHeight / gravity);
        float timeFromPeak = Mathf.Sqrt(2 * (peakHeight - yDisplacement) / gravity);
        float totalTime = timeToPeak + timeFromPeak;

        float xVelocity = xDisplacement / totalTime;
        float yVelocity = Mathf.Sqrt(2 * gravity * peakHeight);

        return new Vector2(xVelocity, yVelocity);
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

    public bool CanBeCountered() {
        return true;
    }

    public void HandleCounter() {
        Destroy(gameObject);
        Instantiate(destroyVFXPrefab, transform.position, Quaternion.identity);
    }

    public void SlowDown(float slowDownPercentage) {
        rb.linearVelocity *= 1 - slowDownPercentage;
        rb.gravityScale *= (1 - slowDownPercentage) * (1 - slowDownPercentage);
        this.slowDownPercentage = slowDownPercentage;
    }

    public void RecoverSpeed() {
        rb.linearVelocity /= 1 - slowDownPercentage;
        rb.gravityScale /= (1 - slowDownPercentage) * (1 - slowDownPercentage);
    }
}
