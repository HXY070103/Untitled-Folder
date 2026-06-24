using UnityEngine;

public class PhaseTrackerController : MonoBehaviour {
    [Header("Debuff info")]
    [SerializeField] private float debuffDuration = 8;
    [SerializeField] private float damageReduction = 10;
    [SerializeField] private float armorRedection = 50;
 
    private void OnTriggerEnter2D(Collider2D collision) {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy == null) {
            return;
        }

        enemy.ApplyPhaseDebuff(debuffDuration, damageReduction, armorRedection);
    }
}
