using UnityEngine;

public class SkillLifeSteal : SkillBase {
    private EntityHealth health;

    [SerializeField] private GameObject fireVFXPrefab;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float conversionRate = 0.3f;
    [Space]
    [SerializeField] private float minFireballGenerationRadius = 1.5f;
    [SerializeField] private float maxFireballGenerationRadius = 2;
    [SerializeField] private int fireballAmount = 5;

    private float totalDamage;
    private GameObject fireVFX;

    public float ConversionRate => conversionRate * stats.GetSkillEffect();

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
        CreateFireball();
    }

    public void Healing() {
        float totalHealingHP = totalDamage * ConversionRate;
        float healingHP = totalHealingHP / fireballAmount;
        health.Healing(healingHP);
    }

    private void CreateFireball() {
        for (int i = 0; i < fireballAmount; i += 1) {
            Vector2 direction = CalculateDirection(i);
            Vector3 randomPosition = GenerateRandomPosition(direction);
            GameObject fireball = 
                Instantiate(fireballPrefab, randomPosition, Quaternion.identity);
            fireball.GetComponent<FireballController>().SetUpFireball(this);
        }
    }

    private Vector2 CalculateDirection(int i) {
        float angle = i * 360f / fireballAmount;
        float rad = angle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    private Vector3 GenerateRandomPosition(Vector2 direction) {
        float randomDistance = 
            Random.Range(minFireballGenerationRadius, maxFireballGenerationRadius);
        Vector3 randomOffset = direction * randomDistance;

        return transform.position + randomOffset;
    }

    private void CreatFireVFX() {
        fireVFX = Instantiate(fireVFXPrefab, transform.position, Quaternion.identity);
        fireVFX.transform.SetParent(transform);
    }

    private void DestroyFireVFX() {
        Destroy(fireVFX);
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
