using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyStoneGuardLaser : MonoBehaviour {
    private BoxCollider2D damageCollider;
    private Animator chargeAnimator;
    private Animator beamAnimator;
    private EnemyStoneGuard owner;

    [Header("Visual references")]
    [SerializeField] private GameObject aimLineObject;
    [SerializeField] private GameObject chargeObject;
    [SerializeField] private GameObject beamObject;

    [Header("Damage info")]
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float damage = 12;

    private bool isFired;
    private bool hasDealtDamage;
    private bool isFinished;

    private void Awake() {
        damageCollider = GetComponent<BoxCollider2D>();
        chargeAnimator = chargeObject != null ? chargeObject.GetComponent<Animator>() : null;
        beamAnimator = beamObject != null ? beamObject.GetComponent<Animator>() : null;
        damageCollider.enabled = false;
    }

    public void SetUp(EnemyStoneGuard owner, int direction) {
        this.owner = owner;

        if (direction < 0) {
            transform.Rotate(0, 180, 0);
        }

        int projectileLayer = LayerMask.NameToLayer("EnemyProjectile");

        if (projectileLayer >= 0) {
            gameObject.layer = projectileLayer;
        }

        aimLineObject?.SetActive(true);
        chargeObject?.SetActive(true);
        beamObject?.SetActive(false);
        damageCollider.enabled = false;

        owner?.RegisterLaser(this);
    }

    public void SetTimeScale(float timeScale) {
        float clampedTimeScale = Mathf.Max(0, timeScale);

        if (chargeAnimator != null) {
            chargeAnimator.speed = clampedTimeScale;
        }

        if (beamAnimator != null) {
            beamAnimator.speed = clampedTimeScale;
        }
    }

    public void Fire() {
        if (isFired || isFinished) {
            return;
        }

        isFired = true;
        aimLineObject?.SetActive(false);
        chargeObject?.SetActive(false);
        beamObject?.SetActive(true);
        damageCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        TryHitTarget(collision);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        TryHitTarget(collision);
    }

    private void TryHitTarget(Collider2D collision) {
        if (!isFired || hasDealtDamage || isFinished || !IsInLayerMask(collision.gameObject.layer, whatIsTarget)) {
            return;
        }

        hasDealtDamage = true;
        owner?.sfx?.PlayAttackHit();

        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(new Damage(damage, false), transform);

        HurtCheckerController hurtChecker = collision.GetComponent<HurtCheckerController>();
        hurtChecker?.GetHurt();
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask) {
        return (layerMask.value & (1 << layer)) != 0;
    }

    public void Finish() {
        if (isFinished) {
            return;
        }

        isFinished = true;
        damageCollider.enabled = false;
        Destroy(gameObject);
    }

    public void DestroyFromOwner() {
        Finish();
    }

    private void OnDestroy() {
        owner?.UnregisterLaser(this);
    }
}
