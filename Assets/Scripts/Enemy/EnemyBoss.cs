using UnityEngine;
using System.Collections;

public class EnemyBoss : MonoBehaviour
{
    [Header("Boss name")]
    [SerializeField] private string bossName = "Final Boss";

    [Header("Increase Per Level")]
    [SerializeField] private float maxHPIncreasePerLevel = 0.25f;
    [SerializeField] private float damageIncreasePerLevel = 0.15f;
    [SerializeField] private float armorIncreasePerLevel = 0.1f;
    [SerializeField] private float moveSpeedIncreasePerLevel = 0.05f;

    private string BossScalingSource = "BossLevelIncrease";

    private Enemy enemy;
    private EntityStats stats;
    private EnemyHealth health;
    public EnemyHealth Health => health;
    public string BossName => bossName;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        stats = GetComponent<EntityStats>();
        health = GetComponent<EnemyHealth>();
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
        int numOfLevel = 0;
        if (SaveManager.instance != null)
        {
            numOfLevel = SaveManager.instance.GetNumOfLevel();
        }
        Debug.Log("level: " + numOfLevel);

        stats.maxHP.RemoveModifier(BossScalingSource);
        stats.damage.RemoveModifier(BossScalingSource);
        stats.armor.RemoveModifier(BossScalingSource);

        float baseMaxHP = stats.maxHP.GetValue();
        float baseDamage = stats.damage.GetValue();
        float baseArmor = stats.armor.GetValue();

        float bonusHP = baseMaxHP * maxHPIncreasePerLevel * numOfLevel;
        float bonusDamage = baseDamage * damageIncreasePerLevel * numOfLevel;
        float bonusArmor = baseArmor * armorIncreasePerLevel * numOfLevel;

        stats.maxHP.AddModifier(bonusHP, BossScalingSource);
        stats.damage.AddModifier(bonusDamage, BossScalingSource);
        stats.armor.AddModifier(bonusArmor, BossScalingSource);

        enemy.battleMoveSpeed *= 1 + moveSpeedIncreasePerLevel * numOfLevel;
        enemy.moveSpeed *= 1 + moveSpeedIncreasePerLevel * numOfLevel;

        health.SetCurrentHP(stats.GetMaxHP());
    }

    private void OnDisable()
    {
        
    }
}