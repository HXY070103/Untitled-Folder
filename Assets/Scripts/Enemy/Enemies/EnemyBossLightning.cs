using System.Collections.Generic;
using UnityEngine;

public class EnemyBossLightning : MonoBehaviour {
    private Collider2D col;
    private Animator anim;

    private EnemyCombat combat;
    private HashSet<Transform> hitTargets;

    [SerializeField] private LayerMask whatIsTarget;


    public void SetUpLightning(EnemyCombat combat, HashSet<Transform> hitTargets, float warningDuration) {
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        this.combat = combat;
        this.hitTargets = hitTargets;

        col.enabled = false;
        anim.enabled = false;

        Invoke(nameof(StartAnimation), warningDuration);
    }

    private void StartAnimation() {
        anim.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & whatIsTarget) == 0) {
            return;
        }

        Player player = collision.GetComponentInParent<Player>();
        Transform target = player == null ? collision.transform : player.transform;

        if (hitTargets != null && !hitTargets.Add(target)) {
            return;
        }

        combat.PerformAttackOnTarget(target);
    }

    public void StartAttack() {
        col.enabled = true;
    }

    public void EndAttack() {
        col.enabled = false;
    }
}
