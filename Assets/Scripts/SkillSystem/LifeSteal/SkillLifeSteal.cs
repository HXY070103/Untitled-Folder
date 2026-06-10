using UnityEngine;

public class SkillLifeSteal : SkillBase {
    private PlayerHealth health;
    private Player player;

    [Header("Skill info")]
    [SerializeField] private GameObject fireVFXPrefab;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float conversionRate = 0.3f;
    [Space]
    [SerializeField] private float minFireballGenerationRadius = 1.5f;
    [SerializeField] private float maxFireballGenerationRadius = 2;
    [SerializeField] private int fireballAmount = 5;
    
    [Header("Unshakable resolve info")]
    [SerializeField] private float reviveHPMultiplier = 0.2f;

    [Header("Desperate gambit info")]
    [SerializeField] private float maxConversionRate = 2;

    [Header("Blood growth info")]
    [SerializeField] private float healingHPThreshold = 0.2f;
    [SerializeField] private float bonusMaxHP = 10;

    [Header("Blood frenzy info")]
    [SerializeField] private float damageTakenMultiplier = 2;
    [SerializeField] private float damageApplyMultiplier = 2;
    [SerializeField] private float attackAnimSpeedMultiplier = 1.5f;

    [SerializeField] private float amountOfBloodBack;

    private float totalHealingHP;
    private GameObject fireVFX;

    public float GetConversionRate() {
        float bonusConversionRate;
        if (UnLocked("Desperate Gambit")) {
            bonusConversionRate = (maxConversionRate - conversionRate) * (1 - health.GetHPPercentage());
        } else {
            bonusConversionRate = 0;
        }
        if (UnlockedForBetterUpgrade("Fixed Blood"))
        {
            return (conversionRate + bonusConversionRate) * stats.GetSkillEffect();
        }
        return (conversionRate + bonusConversionRate) * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    protected override void Awake() {
        base.Awake();

        health = GetComponentInParent<PlayerHealth>();
        player = GetComponentInParent<Player>();
    }

    protected override void StartSkill() {
        base.StartSkill();

        StartAccumulateDamage();
        CreatFireVFX();

        if (UnLocked("Unshakable Resolve")) {
            health.StartDeathPrevention(reviveHPMultiplier);
        }

        if (UnLocked("Blood Frenzy")) {
            health.StartVulnerability(damageTakenMultiplier);
            IncreaseDamage();
            player.IncreaseAttackSpeed(attackAnimSpeedMultiplier);
        }
    }

    protected override void EndSkill() {
        base.EndSkill();

        EndAccumulateDamage();
        DestroyFireVFX();

        if (UnlockedForBetterUpgrade("Blood Back"))
        {
            HandleBloodBack();
        }

        if (!UnLocked("Unshakable Resolve") && !UnLocked("Stable Gain")) {
            CreateFireball();
        }

        if (UnLocked("Unshakable Resolve")) {
            health.EndDeathPrevention();
        }

        if (UnLocked("Blood Growth")) {
            HandleMaxHPIncrease();
        }

        if (UnLocked("Blood Frenzy")) {
            health.EndVulnerability();
            ResetDamage();
            player.ResetAttackSpeed();
        }
    }

    private void HandleBloodBack()
    {
        if (totalHealingHP >= amountOfBloodBack)
        {
            DecreaseDecay(1);
        }
    }

    public void HealingAtEnd() {
        float healingHP = totalHealingHP / fireballAmount;
        health.Healing(healingHP);
    }

    private void IncreaseDamage() {
        float baseDamage = stats.damage.GetValue();
        float bonusDamage = baseDamage * (damageApplyMultiplier - 1);
        stats.damage.AddModifier(bonusDamage, "BloodFrenzy");
    }

    private void ResetDamage() {
        stats.damage.RemoveModifier("BloodFrenzy");
    }

    private void HandleMaxHPIncrease() {
        if (totalHealingHP > stats.GetMaxHP() * healingHPThreshold) {
            health.IncreaseMaxHP(bonusMaxHP, "BloodGrowth");
        }
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

        if (UnLocked("Stable Gain")) {
            health.Healing(healingHP);
        }
    }
}
