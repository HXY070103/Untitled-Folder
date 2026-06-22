using UnityEngine;

public class EnemyBossLightning : MonoBehaviour {
    private Collider2D col;
    private EnemyCombat combat;

    [SerializeField] private LayerMask whatIsTarget;


    public void SetUpLightning(EnemyCombat combat) {
        col = GetComponent<Collider2D>();
        this.combat = combat;

        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0) {
            combat.PerformAttackOnTarget(collision.transform);
        }
    }

    public void StartAttack() {
        col.enabled = true;
    }

    public void EndAttack() {
        col.enabled = false;
    }
}
