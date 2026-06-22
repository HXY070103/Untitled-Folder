using UnityEngine;

public class EnemyBossLightningAnimationTrigger : MonoBehaviour {
    private EnemyBossLightning lightning;

    private void Awake() {
        lightning = GetComponentInParent<EnemyBossLightning>();
    }

    public void StartAttack() {
        lightning.StartAttack();
    }

    public void EndAttack() {
        lightning.EndAttack();
    }

    public void DestroyGameObject() {
        Destroy(lightning.gameObject);
    }
}
