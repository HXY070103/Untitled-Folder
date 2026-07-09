using Unity.VisualScripting;
using UnityEngine;

public class SkillLifeSteal : SkillBase {
    private PlayerHealth health;
    private Player player;

    [Header("Skill info")]
    [SerializeField] private float conversionRate = 0.3f;
    [SerializeField] private GameObject fireVFXPrefab;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float minFireballGenerationRadius = 1.5f;
    [SerializeField] private float maxFireballGenerationRadius = 2;
    [SerializeField] private int fireballAmount = 5;

    private float totalHealingHP;
    private GameObject fireVFX;

    [Header("Stable gain info")]
    [SerializeField] private float temporaryShield = 50;

    [Header("Desperate gambit info")]
    [SerializeField] private float maxConversionRate = 2;
    [SerializeField] private float maxDamageMultiplier = 2;
    [SerializeField] private float maxAttackSpeed = 2;

    [Header("Blood growth info")]
    [SerializeField] private float healingHPThreshold = 0.5f;
    [SerializeField] private float bonusMaxHP = 10;
    [SerializeField] private float bonusArmor = 30;
    [SerializeField] private float bonusDamage = 15;
    
    [Header("Unshakable resolve info")]
    [SerializeField] private float reviveHPMultiplier = 0.2f;

    [Header("Overflowing aegis info")]
    [SerializeField] private float damageTakenMultiplier = 2;
    [SerializeField] private float damageApplyMultiplier = 2;
    [SerializeField] private float attackAnimSpeedMultiplier = 1.5f;

    [SerializeField] private float amountOfBloodBack;

    public float GetConversionRate() {
        float bonusConversionRate;

        if (Unlocked("Desperate Gambit")) {
            bonusConversionRate = (maxConversionRate - conversionRate) * (1 - health.GetHPPercentage());
        } else {
            bonusConversionRate = 0;
        }
        
        return (conversionRate + bonusConversionRate) * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    protected override void Awake() {
        base.Awake();

        health = GetComponentInParent<PlayerHealth>();
        player = GetComponent<Player>();
    }

    protected override void StartSkill() {
        base.StartSkill();

        StartAccumulateDamage();
        CreatFireVFX();

        if (Unlocked("Stable Gain+")) {
            health.GenerateTemporaryShield(temporaryShield);
        }

        if (Unlocked("Unshakable Resolve")) {
            health.StartDeathPrevention(reviveHPMultiplier);
        }
    }

    protected override void EndSkill() {
        base.EndSkill();

        EndAccumulateDamage();
        DestroyFireVFX();

        if (Unlocked("Stable Gain+")) {
            health.CancelTemporaryShield();
        }

        ApplyBloodGrowthReward();

        if (!Unlocked("Unshakable Resolve") && !Unlocked("Stable Gain")) {
            CreateFireball();
        }

        if (Unlocked("Unshakable Resolve")) {
            health.StopDeathPrevention();
        }
    }

    private void ApplyBloodGrowthReward() {
        if (totalHealingHP < stats.GetMaxHP() * healingHPThreshold) {
            return;
        }

        if (Unlocked("Blood Growth")) {
            health.IncreaseMaxHP(bonusMaxHP, "BloodGrowth");
        }

        if (Unlocked("Blood Growth+")) {
            stats.armor.AddModifier(bonusArmor, "BloodGrowth");
            stats.damage.AddModifier(bonusDamage, "BloodGrowth");
        }
    }

    private void Healing(float healingHP) {
        float overflowingHP = health.Healing(healingHP);
        
        if (Unlocked("Overflowing Aegis")) {
            health.GeneratePermanentShield(overflowingHP);
        }
    }

    public void HealingAtEnd() {
        float healingHP = totalHealingHP / fireballAmount;
        Healing(healingHP);
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
        totalHealingHP = 0;
        EnemyHealth.OnDamageTaken += AccumulateDamage;
    }

    private void EndAccumulateDamage() {
        EnemyHealth.OnDamageTaken -= AccumulateDamage;
    }

    private void AccumulateDamage(float damage) {
        float healingHP = damage * GetConversionRate();
        totalHealingHP += healingHP;

        if (Unlocked("Stable Gain")) {
            Healing(healingHP);
        }
    }
}
