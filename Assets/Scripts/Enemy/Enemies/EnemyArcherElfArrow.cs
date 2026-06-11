using UnityEngine;

public class EnemyArcherElfArrow : MonoBehaviour, ICounterable, ISlowable{
    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;

    private EntityCombat combat;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float stuckDuration = 3;
    [SerializeField] private GameObject destroyVFXPrefab;

    private float arrowSpeed;

    public void SetUpArrow(float arrowSpeed, EntityCombat combat) {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        this.combat = combat;

        this.arrowSpeed = arrowSpeed;
        rb.linearVelocity = new Vector2(arrowSpeed, 0);

        if (rb.linearVelocity.x < 0) {
            transform.Rotate(0, 180, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0) {
            combat.PerforAttackOnTarget(collision.transform);
            StuckIntoTarget(collision.transform);
        }
    }

    private void StuckIntoTarget(Transform target) {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;
        anim.enabled = false; 
        transform.parent = target;
        Destroy(gameObject, stuckDuration);
    }

    public bool CanBeCountered() {
        return true;
    }

    public void HandleCounter() {
        Destroy(gameObject);
        Instantiate(destroyVFXPrefab, transform.position, Quaternion.identity);
    }

    public void SlowDown(float slowDownPercentage) {
        float xVelocity = arrowSpeed * (1 - slowDownPercentage);
        rb.linearVelocity = new Vector2(xVelocity, 0);
    }

    public void RecoverSpeed() {
        rb.linearVelocity = new Vector2(arrowSpeed, 0);
    }
}
