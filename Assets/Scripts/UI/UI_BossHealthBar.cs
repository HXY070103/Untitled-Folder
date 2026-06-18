using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_BossHealthbar : MonoBehaviour
{
    public static UI_BossHealthbar instance;

    [Header("UI")]
    [SerializeField] private GameObject bossHealthBarRoot;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI bossNameText;

    private EnemyBoss boss;

    private void Awake()
    {
        instance = this;

        if (bossHealthBarRoot != null)
        {
            bossHealthBarRoot.SetActive(false);
        }
    }

    public void Show(EnemyBoss currentBoss)
    {
        if (currentBoss == null)
        {
            return;
        }

        if (boss != null)
        {
            boss.Health.OnHealthUpdate -=UpdateBossHealthBar;
        }

        boss = currentBoss;
        boss.Health.OnHealthUpdate += UpdateBossHealthBar;

        if (bossHealthBarRoot != null)
        {
            bossHealthBarRoot.SetActive(true);
        }
        if (bossNameText != null)
        {
            bossNameText.text = boss.BossName;
        }
        UpdateBossHealthBar();
    }

    public void Hide(EnemyBoss currentBoss)
    {
        if (currentBoss != boss)
        {
            return;
        }

        boss.Health.OnHealthUpdate -= UpdateBossHealthBar;
        boss = null;

        if (bossHealthBarRoot != null)
        {
            bossHealthBarRoot.SetActive(false);
        }
    }

    private void UpdateBossHealthBar()
    {
        if (boss == null)
        {
            return;
        }

        float currentHP = Mathf.RoundToInt(boss.Health.GetCurrentHP());
        float maxHP = Mathf.RoundToInt(boss.Health.GetComponent<EntityStats>().GetMaxHP());

        if (healthSlider != null)
        {
            healthSlider.value = boss.Health.GetHPPercentage();
        }
        if (healthText != null)
        {
            healthText.text = currentHP + "/" + maxHP;
        }

        if (currentHP <= 0)
        {
            if (bossHealthBarRoot != null)
            {
                bossHealthBarRoot.SetActive(false);
            }
        }
    }
}