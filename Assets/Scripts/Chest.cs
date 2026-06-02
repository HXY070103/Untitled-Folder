using UnityEngine;

public class Chest : MonoBehaviour, IDamageable {
    private Animator anim => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private EntityVFX fx => GetComponent<EntityVFX>();

    [Header("Open info")]
    [SerializeField] private Vector2 knockback = new Vector2(0, 5);

    private bool isOpen;

    public void TakeDamage(Damage damage, Transform damageDealer) {
        if (isOpen) return;
        isOpen = true;
        anim.SetBool("Open", true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-180, 180);
        fx.PlayOnDamageVFX();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Chest: Player not found in scene.");
            return;
        }

        if (UI_ChestReward.Instance == null)
        {
            Debug.LogError("Chest: UI_ChestReward instance not found.");
            return;
        }

        UI_ChestReward.Instance.Open(player);
    }

    public bool IsInvulnerable() {
        return false;
    }

    public void TryApplyInstantKill() {}
}
