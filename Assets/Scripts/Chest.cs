using UnityEngine;

public class Chest : MonoBehaviour, IDamageable {
    private Animator anim => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private EntityVFX fx => GetComponent<EntityVFX>();

    [Header("Open info")]
    [SerializeField] private Vector2 knockback = new Vector2(0, 5);

    public void TakeDamage(Damage damage, Transform damageDealer) {
        anim.SetBool("Open", true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-180, 180);
        fx.PlayOnDamageVFX();
    }

    public bool IsInvulnerable() {
        return false;
    }
}
