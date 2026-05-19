using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SkillLifeSteal : SkillBase {
    private EntityHealth health;

    [SerializeField] private GameObject firePrefab;
    [SerializeField] private float coversionRate = 0.3f;

    public float totalDamage;
    private GameObject fireVFX;

    protected override void Awake() {
        base.Awake();

        health = GetComponentInParent<EntityHealth>();
    }

    protected override void StartSkill() {
        base.StartSkill();

        StartAccumulateDamage();
        CreatFireVFX();
    }

    protected override void EndSkill() {
        base.EndSkill();

        EndAccumulateDamage();
        DestroyFireVFX();
    }

    public void Healing() {
        float healingHP = totalDamage * coversionRate;
        health.Healing(healingHP);
    }

    private void CreatFireVFX() {
        fireVFX = Instantiate(firePrefab, transform.position, Quaternion.identity);
        fireVFX.transform.SetParent(transform);
    }

    private void DestroyFireVFX() {
        fireVFX.GetComponent<FireVFXController>().Shrink();
    }

    private void StartAccumulateDamage() {
        totalDamage = 0;
        EnemyHealth.OnDamageTaken += AccumulateDamage;
    }

    private void EndAccumulateDamage() {
        EnemyHealth.OnDamageTaken -= AccumulateDamage;
    }

    private void AccumulateDamage(float damage) {
        totalDamage += damage;
    }
}
