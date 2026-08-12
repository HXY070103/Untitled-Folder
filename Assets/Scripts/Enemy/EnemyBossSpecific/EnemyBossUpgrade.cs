using UnityEngine;
using System.Collections;

public class EnemyBossUpgrade : MonoBehaviour
{
    private const string BossScalingSource = "BossLevelIncrease";
    private const string SummonScalingSource = "BossSummonIncrease";

    [Header("Boss name")]
    [SerializeField] private string bossName = "Final Boss";

    [Header("Boss increase per repeat level")]
    [SerializeField] private float maxHPIncreasePerLevel = 0.04f;
    [SerializeField] private float maxHPIncreaseCap = 0.8f;
    [SerializeField] private float damageIncreasePerLevel = 0.025f;
    [SerializeField] private float damageIncreaseCap = 0.5f;
    [SerializeField] private float armorIncreasePerLevel = 1f;
    [SerializeField] private float maxArmor = 30f;
    [SerializeField] private float moveSpeedIncreasePerLevel = 0.005f;
    [SerializeField] private float moveSpeedIncreaseCap = 0.1f;

    [Header("Summon increase per repeat level")]
    [SerializeField] private float summonMaxHPIncreasePerLevel = 0.03f;
    [SerializeField] private float summonMaxHPIncreaseCap = 0.3f;
    [SerializeField] private float summonDamageIncreasePerLevel = 0.02f;
    [SerializeField] private float summonDamageIncreaseCap = 0.2f;

    private Enemy enemy;
    private EntityStats stats;
    private EnemyHealth health;
    private float baseBattleMoveSpeed;
    private float baseMoveSpeed;

    public EnemyHealth Health => health;
    public string BossName => bossName;
    public int RepeatLevel { get; private set; }

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        stats = GetComponent<EntityStats>();
        health = GetComponent<EnemyHealth>();
        baseBattleMoveSpeed = enemy.battleMoveSpeed;
        baseMoveSpeed = enemy.moveSpeed;
    }

    private IEnumerator Start()
    {
        yield return null;

        ApplyBossIncrease();
        if (UI_BossHealthbar.instance != null)
        {
            UI_BossHealthbar.instance.Show(this);
        }
    }

    private void ApplyBossIncrease()
    {
        RepeatLevel = SaveManager.instance == null ? 0 : SaveManager.instance.GetNumOfLevel();
        stats.maxHP.RemoveModifier(BossScalingSource);
        stats.damage.RemoveModifier(BossScalingSource);
        stats.armor.RemoveModifier(BossScalingSource);

        float baseMaxHP = stats.maxHP.GetValue();
        float baseDamage = stats.damage.GetValue();
        float baseArmor = stats.armor.GetValue();

        float hpIncrease = Mathf.Min(
            maxHPIncreasePerLevel * RepeatLevel,
            maxHPIncreaseCap
        );
        float damageIncrease = Mathf.Min(
            damageIncreasePerLevel * RepeatLevel,
            damageIncreaseCap
        );
        float moveSpeedIncrease = Mathf.Min(
            moveSpeedIncreasePerLevel * RepeatLevel,
            moveSpeedIncreaseCap
        );

        float bonusHP = baseMaxHP * hpIncrease;
        float bonusDamage = baseDamage * damageIncrease;
        float targetArmor = Mathf.Min(
            baseArmor + armorIncreasePerLevel * RepeatLevel,
            maxArmor
        );
        float bonusArmor = targetArmor - baseArmor;

        stats.maxHP.AddModifier(bonusHP, BossScalingSource);
        stats.damage.AddModifier(bonusDamage, BossScalingSource);
        stats.armor.AddModifier(bonusArmor, BossScalingSource);

        enemy.battleMoveSpeed = baseBattleMoveSpeed * (1 + moveSpeedIncrease);
        enemy.moveSpeed = baseMoveSpeed * (1 + moveSpeedIncrease);

        health.SetCurrentHP(stats.GetMaxHP());
        Debug.Log(
            $"Boss repeat level {RepeatLevel}: " +
            $"HP {stats.GetMaxHP()}, damage {stats.damage.GetValue()}, " +
            $"armor {stats.armor.GetValue()}"
        );
    }

    public void ApplySummonIncrease(Enemy summonedEnemy)
    {
        if (summonedEnemy == null)
        {
            return;
        }

        EntityStats summonStats = summonedEnemy.stats;
        EnemyHealth summonHealth = summonedEnemy.health;

        summonStats.maxHP.RemoveModifier(SummonScalingSource);
        summonStats.damage.RemoveModifier(SummonScalingSource);

        float baseMaxHP = summonStats.maxHP.GetValue();
        float baseDamage = summonStats.damage.GetValue();
        float hpIncrease = Mathf.Min(
            summonMaxHPIncreasePerLevel * RepeatLevel,
            summonMaxHPIncreaseCap
        );
        float damageIncrease = Mathf.Min(
            summonDamageIncreasePerLevel * RepeatLevel,
            summonDamageIncreaseCap
        );

        summonStats.maxHP.AddModifier(
            baseMaxHP * hpIncrease,
            SummonScalingSource
        );
        summonStats.damage.AddModifier(
            baseDamage * damageIncrease,
            SummonScalingSource
        );
        summonHealth.SetCurrentHP(summonStats.GetMaxHP());
    }
}
