using UnityEngine;

public class EnemyStoneGuardLaser : MonoBehaviour {

    [SerializeField] private GameObject aimLineObject;
    [SerializeField] private GameObject chargeObject;
    [SerializeField] private GameObject beamObject;
    [SerializeField] private LayerMask whatIsTarget;

    private Collider2D col;
    private EnemyCombat combat;

    public void SetUpLaser(EnemyCombat combat, int dir) {
        col = GetComponent<Collider2D>();
        this.combat = combat;

        if (dir < 0) {
            transform.Rotate(0, 180, 0);
        }

        aimLineObject.SetActive(true);
        chargeObject.SetActive(true);
        beamObject.SetActive(false);

        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0) {
            combat.PerformAttackOnTarget(collision.transform);
        }
    }

    public void StartLaser() {
        aimLineObject.SetActive(false);
        chargeObject.SetActive(false);
        beamObject.SetActive(true);
        col.enabled = true;
    }

    public void EndLaser() {
        col.enabled = false;
        Destroy(gameObject);
    }
}
