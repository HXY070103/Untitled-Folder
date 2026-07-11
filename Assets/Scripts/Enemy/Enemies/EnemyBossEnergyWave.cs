using UnityEngine;

public class EnemyBossEnergyWave : MonoBehaviour, ICounterable, ISlowable {
    private Rigidbody2D rb;
    private Animator anim;
    private EntityCombat combat;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask canCollideWith;
    [SerializeField] private GameObject destroyVFXPrefab;
    [SerializeField] private float maxDistance = 20;

    private Vector3 startPosition;
    private float energyWaveSpeed;

    public void SetUpEnergyWave(float energyWaveSpeed, EntityCombat combat) {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        this.combat = combat;

        startPosition = transform.position;
        this.energyWaveSpeed = energyWaveSpeed;
        
        rb.linearVelocity = new Vector2(energyWaveSpeed, 0);

        if (rb.linearVelocity.x < 0) {
            transform.Rotate(0, 180, 0);
        }
    }

    public void SetUpEnergyWave(float energyWaveSpeed, EntityCombat combat, float travelDistance) {
        maxDistance = Mathf.Max(1, travelDistance);
        SetUpEnergyWave(energyWaveSpeed, combat);
    }

    private void Update() {
        if (ReachedMaxDistance()) {
            Destroy(gameObject);
        }
    }

    private bool ReachedMaxDistance() {
        return Vector3.Distance(transform.position, startPosition) > maxDistance;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0) {
            combat.PerformAttackOnTarget(collision.transform);
        }

        if (((1 << collision.gameObject.layer) & canCollideWith) != 0) {
            Destroy(gameObject);
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
        anim.speed = 1 - slowDownPercentage;
        float xVelocity = energyWaveSpeed * (1 - slowDownPercentage);
        rb.linearVelocity = new Vector2(xVelocity, 0);
    }

    public void RecoverSpeed() {
        anim.speed = 1;
        rb.linearVelocity = new Vector2(energyWaveSpeed, 0);
    }

    public void RecoverSpeedWithDelay(float delay) {
        Invoke(nameof(RecoverSpeed), delay);
    }
}
