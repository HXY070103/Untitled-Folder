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
    public float maxDamageMultiplier = 2;
    public float maxAttackSpeedMultiplier = 2;

    [Header("Blood growth info")]
    [SerializeField] private float healingHPThreshold = 0.5f;
    [SerializeField] private float bonusMaxHP = 10;
    [SerializeField] private float bonusArmor = 30;
    [SerializeField] private float bonusDamage = 15;

    [Header("Overflowing aegis info")]
    public float damageMultiplier = 2;
    public float attackSpeedMultiplier = 1.5f;
    
    [Header("Unshakable resolve info")]
    [SerializeField] private float reviveHPMultiplier = 0.2f;

    [SerializeField] private float amountOfBloodBack;

    public float GetConversionRate() {
        float bonusConversionRate;

        if (Unlocked("Desperate Gambit")) {
            bonusConversionRate = 
                Mathf.Lerp(maxConversionRate - conversionRate, 0, health.GetHPPercentage());
        } else {
            bonusConversionRate = 0;
        }
        
        return (conversionRate + bonusConversionRate) * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    public float GetDamageMultiplier() {
        return Mathf.Lerp(maxDamageMultiplier, 1, health.GetHPPercentage());
    }

    public float GetAttackSpeedMultiplier() {
        return Mathf.Lerp(maxAttackSpeedMultiplier, 1, health.GetHPPercentage());
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

        if (Unlocked("Stable Gain+")) {
            health.ShieldManager.AddTemporaryShield(temporaryShield);
        }

        StartDesperateGambitBuff(health.GetCurrentHP());

        if (Unlocked("Unshakable Resolve")) {
            health.StartDeathPrevention(reviveHPMultiplier);
        }
    }

    protected override void EndSkill() {
        base.EndSkill();

        EndAccumulateDamage();
        DestroyFireVFX();
        CreateFireball();

        if (Unlocked("Stable Gain+")) {
            health.ShieldManager.RemoveTemporaryShield();
        }

        EndDesperateGambitBuff();
        ApplyBloodGrowthReward();

        if (Unlocked("Unshakable Resolve")) {
            health.StopDeathPrevention();
        }
    }

    private void StartDesperateGambitBuff(float reducedHP) {
        if (!Unlocked("Desperate Gambit+")) {
            return;
        }

        health.SetCanApplyBuff(true);

        stats.damage.AddMultiplier(GetDamageMultiplier(), "DesperateGambit");
        stats.attackSpeed.AddMultiplier(GetAttackSpeedMultiplier(), "DesperateGambit");
    }

    private void EndDesperateGambitBuff() {
        if (!Unlocked("Desperate Gambit+")) {
            return;
        }

        health.SetCanApplyBuff(false);

        stats.damage.RemoveMultiplier("DesperateGambit");
        stats.attackSpeed.RemoveMultiplier("DesperateGambit");
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
        
        if (Unlocked("Overflowing Aegis") && overflowingHP > 0) {
            health.ShieldManager.AddPermanentShield(overflowingHP);
        }
    }

    public void HealingAtEnd() {
        float healingHP = totalHealingHP / fireballAmount;
        Healing(healingHP);
    }

    private void CreateFireball() {
        if (Unlocked("Unshakable Resolve") || Unlocked("Stable Gain")) {
            return;
        }

        for (int i = 0; i < fireballAmount; i += 1) {
            Vector3 randomPosition = GenerateRandomPosition(i);
            GameObject fireball = 
                Instantiate(fireballPrefab, randomPosition, Quaternion.identity);
            fireball.GetComponent<FireballController>().SetUpFireball(this);
        }
    }

    private Vector3 GenerateRandomPosition(int i) {
        float angle = i * 360f / fireballAmount;
        float rad = angle * Mathf.Deg2Rad;
        Vector2 dir =  new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        float randomDistance = 
            Random.Range(minFireballGenerationRadius, maxFireballGenerationRadius);
        Vector3 randomOffset = dir * randomDistance;

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

    private void AccumulateDamage(float damage, Transform damageDealer) {
        if (damageDealer != player.transform) {
            return;
        }

        float healingHP = damage * GetConversionRate();
        totalHealingHP += healingHP;

        if (Unlocked("Stable Gain")) {
            Healing(healingHP);
        }
    }
}
