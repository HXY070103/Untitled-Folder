using UnityEngine;

public class EnemyVFX : EntityVFX {
    [Header("Counter attack window info")]
    [SerializeField] private GameObject attackAlert;

    public void SetAttackAlert(bool enable) {
        attackAlert.SetActive(enable);
    }
}
