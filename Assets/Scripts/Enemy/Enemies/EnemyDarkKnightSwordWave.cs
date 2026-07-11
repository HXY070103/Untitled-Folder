using UnityEngine;

public class EnemyDarkKnightSwordWave : MonoBehaviour, ICounterable, ISlowable {
    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;

    private EntityCombat combat;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatCanCollideWith;
    [SerializeField] private float destroyDelay = 3;
    [SerializeField] private GameObject destroyVFXPrefab;

    private float swordWaveSpeed;
    private Vector2 swordWaveVelocity;

    public void SetUpSwordWave(float swordWaveSpeed, EntityCombat combat) {
        SetUpSwordWave(new Vector2(swordWaveSpeed, 0), combat);
    }

    public void SetUpSwordWave(Vector2 velocity, EntityCombat combat) {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        this.combat = combat;

        swordWaveVelocity = velocity;
        swordWaveSpeed = velocity.magnitude;
        rb.linearVelocity = velocity;

        if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x)) {
            transform.Rotate(0, 0, velocity.y < 0 ? -90 : 90);
        } else if (rb.linearVelocity.x < 0) {
            transform.Rotate(0, 180, 0);
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
        anim.speed = 1;
        anim.SetBool("Explosion", true);
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
        CancelInvoke(nameof(RecoverSpeed));
        anim.speed = 1 - slowDownPercentage;
        rb.linearVelocity = swordWaveVelocity.normalized * swordWaveSpeed * (1 - slowDownPercentage);
    }

    public void RecoverSpeed() {
        anim.speed = 1;
        rb.linearVelocity = swordWaveVelocity.normalized * swordWaveSpeed;
    }

    public void RecoverSpeedWithDelay(float delay) {
        Invoke(nameof(RecoverSpeed), delay);
    }
}
