using UnityEngine;

public class EnemyNecromancerFireAnimationTrigger : MonoBehaviour {
    private EnemyNecromancerFire fire;

    private void Awake() {
        fire = GetComponentInParent<EnemyNecromancerFire>();
    }

    public void StartAttack() {
        fire.StartAttack();
    }

    public void EndAttack() {
        fire.EndAttack();
    }

    public void DestroyGameObject() {
        Destroy(fire.gameObject);
    }
}
