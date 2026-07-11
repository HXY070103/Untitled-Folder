using UnityEngine;

public class EnemyNecromancerFire : MonoBehaviour {
    private Collider2D col;
    private EnemyCombat combat;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float destroyDelay = 1;
    private Vector2 velocity;

    public void SetUpFire(EnemyCombat combat) {
        col = GetComponent<Collider2D>();
        this.combat = combat;
        Destroy(gameObject, destroyDelay);
    }

    public void SetUpFallingFire(EnemyCombat combat, Vector2 velocity, float lifetime) {
        SetUpFire(combat);
        this.velocity = velocity;
        CancelInvoke();
        Destroy(gameObject, lifetime);
    }

    private void Update() {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0) {
            combat.PerformAttackOnTarget(collision.transform);
            col.enabled = false;
        }
    }
}
