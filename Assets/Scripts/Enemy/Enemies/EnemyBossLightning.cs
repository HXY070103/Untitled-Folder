using System.Collections.Generic;
using UnityEngine;

public class EnemyBossLightning : MonoBehaviour {
    private Collider2D col;
    private EnemyCombat combat;
    private float damageMultiplier = 1f;
    private HashSet<Transform> hitTargets;

    [SerializeField] private LayerMask whatIsTarget;


    public void SetUpLightning(
        EnemyCombat combat,
        float damageMultiplier = 1f,
        HashSet<Transform> sharedHitTargets = null
    ) {
        col = GetComponent<Collider2D>();
        this.combat = combat;
        this.damageMultiplier = Mathf.Max(0, damageMultiplier);
        hitTargets = sharedHitTargets ?? new HashSet<Transform>();

        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & whatIsTarget) == 0) {
            return;
        }

        Player player = collision.GetComponentInParent<Player>();
        Transform target = player == null
            ? collision.transform
            : player.transform;

        if (hitTargets != null && !hitTargets.Add(target)) {
            return;
        }

        combat.PerformAttackOnTarget(target, damageMultiplier);
    }

    public void StartAttack() {
        col.enabled = true;
    }

    public void EndAttack() {
        col.enabled = false;
    }
}
