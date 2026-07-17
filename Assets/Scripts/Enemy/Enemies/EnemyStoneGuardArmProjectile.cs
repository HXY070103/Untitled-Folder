using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class EnemyStoneGuardArmProjectile : MonoBehaviour, ICounterable, ISlowable {
    private Rigidbody2D rb;
    private Animator anim;
    private EnemyStoneGuard owner;

    [Header("Collision info")]
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private LayerMask whatCanCollideWith;

    [Header("Projectile info")]
    [SerializeField] private float damage = 16;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float lifeTime = 2.5f;
    [SerializeField] private GameObject counterVFXPrefab;
    [SerializeField] private Color counterVFXColor = Color.cyan;

    private int moveDirection = 1;
    private int comboAttackId;
    private float speedMultiplier = 1;
    private float remainingLifeTime;
    private bool isResolved;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        remainingLifeTime = lifeTime;
    }

    private void Update() {
        if (isResolved) {
            return;
        }

        remainingLifeTime -= Time.deltaTime * speedMultiplier;

        if (remainingLifeTime <= 0) {
            Resolve(false, false);
        }
    }

    public void SetUp(EnemyStoneGuard owner, int moveDirection, int comboAttackId = 0) {
        this.owner = owner;
        this.moveDirection = moveDirection >= 0 ? 1 : -1;
        this.comboAttackId = comboAttackId;

        if (this.moveDirection < 0) {
            transform.Rotate(0, 180, 0);
        }

        owner?.RegisterArmProjectile(this);
        ApplySpeedMultiplier();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isResolved) {
            return;
        }

        if (IsInLayerMask(collision.gameObject.layer, whatIsTarget)) {
            HitTarget(collision);
            Resolve(false, false);
            return;
        }

        if (IsInLayerMask(collision.gameObject.layer, whatCanCollideWith)) {
            Resolve(false, false);
        }
    }

    private void HitTarget(Collider2D collision) {
        owner.sfx?.PlayAttackHit();
        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(new Damage(damage, false), transform);

        HurtCheckerController hurtChecker = collision.GetComponent<HurtCheckerController>();
        hurtChecker?.GetHurt();
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask) {
        return (layerMask.value & (1 << layer)) != 0;
    }

    public bool CanBeCountered() {
        return !isResolved;
    }

    public void HandleCounter() {
        if (!CanBeCountered()) {
            return;
        }

        Resolve(true, true);
    }

    public void SlowDown(float slowDownPercentage) {
        CancelInvoke(nameof(RecoverSpeed));
        speedMultiplier = 1 - Mathf.Clamp01(slowDownPercentage);
        ApplySpeedMultiplier();
    }

    public void RecoverSpeed() {
        CancelInvoke(nameof(RecoverSpeed));
        speedMultiplier = 1;
        ApplySpeedMultiplier();
    }

    public void RecoverSpeedWithDelay(float delay) {
        CancelInvoke(nameof(RecoverSpeed));
        Invoke(nameof(RecoverSpeed), delay);
    }

    public void DestroyFromOwner() {
        Resolve(false, false);
    }

    private void ApplySpeedMultiplier() {
        if (rb != null) {
            rb.linearVelocity = new Vector2(moveDirection * moveSpeed * speedMultiplier, 0);
        }

        if (anim != null) {
            anim.speed = speedMultiplier;
        }
    }

    private void Resolve(bool exposeCore, bool playDestroyVFX) {
        if (isResolved) {
            return;
        }

        isResolved = true;
        rb.linearVelocity = Vector2.zero;

        if (playDestroyVFX) {
            CreateCounterVFX();
        }

        if (exposeCore) {
            owner?.HandleArmProjectileCounter(comboAttackId);
        }

        Destroy(gameObject);
    }

    private void CreateCounterVFX() {
        if (counterVFXPrefab == null) {
            return;
        }

        GameObject counterVFX = Instantiate(counterVFXPrefab, transform.position, Quaternion.identity);
        SpriteRenderer vfxRenderer = counterVFX.GetComponentInChildren<SpriteRenderer>();

        if (vfxRenderer != null) {
            vfxRenderer.color = counterVFXColor;
        }

        ParticleSystem particleSystem = counterVFX.GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null) {
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = counterVFXColor;
        }
    }

    private void OnDestroy() {
        owner?.UnregisterArmProjectile(this);
    }
}
