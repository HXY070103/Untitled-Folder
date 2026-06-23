using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using TMPro;

public class UI_ChestReward : MonoBehaviour
{
    public static UI_ChestReward Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private GameObject mainChoicePanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject purificationPanel;

    [Header("Upgrade Options")]
    [SerializeField] private UI_UpgradeOption[] upgradeOptionUIs;

    [Header("Main Choice Buttons")]
    [SerializeField] private UI_ChestChoiceButton upgradeChoiceButton;
    [SerializeField] private UI_ChestChoiceButton purificationChoiceButton;
    [SerializeField] private TextMeshProUGUI detailForPurification;

    private GameObject player;
    private PlayerVFX vfx;
    private PlayerStats playerStats;
    private PlayerSkillManager skillManager;
    private Chest currentChest;
    private bool isBetter;

    private void Awake()
    {
        Instance = this;

        rootPanel.SetActive(false);
        mainChoicePanel.SetActive(false);
        upgradePanel.SetActive(false);
        purificationPanel.SetActive(false);
    }

    public void Open(GameObject playerObj, Chest chest, bool better) 
    {
        player = playerObj;
        currentChest = chest;
        isBetter = better;

        vfx = player.GetComponent<PlayerVFX>();
        playerStats = player.GetComponent<PlayerStats>();
        skillManager = player.GetComponent<PlayerSkillManager>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        float healHP;

        if (playerStats == null)
        {
            Debug.LogError("ChestRewardUI: PlayerStats not found on player.");
            return;
        }

        if (skillManager == null)
        {
            Debug.LogError("ChestRewardUI: PlayerSkillManager not found on player.");
            return;
        }

        GamePauseManager.Instance.Pause("ChestReward");

        rootPanel.SetActive(true);
        mainChoicePanel.SetActive(true);
        upgradePanel.SetActive(false);
        purificationPanel.SetActive(false);
        if (isBetter)
        {
            detailForPurification.text = "- 3 skill decay";
            healHP = 0.5f * playerStats.GetMaxHP();
        } else
        {
            detailForPurification.text = "- 2 skill decay";
            healHP = 0.3f * playerStats.GetMaxHP();
        }
        playerHealth.Healing(healHP);
        upgradeChoiceButton.Setup(ShowUpgrades);
        purificationChoiceButton.Setup(ShowPurification);
    }

    public void ShowUpgrades()
    {
        mainChoicePanel.SetActive(false);
        purificationPanel.SetActive(false);
        upgradePanel.SetActive(true);

        List<ChestUpgradeOption> options;

        if (isBetter)
        {
            options = GetRandomBetterUpgradeOptions(3);
        }
        else
        {
            options = GetRandomUpgradeOptions(3);
        }

        for (int i = 0; i < upgradeOptionUIs.Length; i++)
        {
            if (i < options.Count)
            {
                upgradeOptionUIs[i].gameObject.SetActive(true);
                upgradeOptionUIs[i].Setup(options[i], this);
            }
            else
            {
                upgradeOptionUIs[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowPurification()
    {
        mainChoicePanel.SetActive(false);
        upgradePanel.SetActive(false);
        purificationPanel.SetActive(true);

        vfx.GenerateParticleEffect(Color.blue);
        if (isBetter)
        {
            skillManager.GetCurrentSkill().DecreaseDecay(3);
        } else
        {
            skillManager.GetCurrentSkill().DecreaseDecay(2);
        }

        if (currentChest != null)
        {
            currentChest.ClaimReward();
        }
        
        SaveGame();
        StartCoroutine(ClosePurificationAfterDelay());
    }

    private IEnumerator ClosePurificationAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Close();
    }

    public void SelectUpgrade(ChestUpgradeOption option)
    {
        option.Apply();
        if (currentChest != null)
        {
            currentChest.ClaimReward();
        }
        SaveGame();
        Close();
    }

    private void SaveGame()
    {
        SaveManager saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
        {
            saveManager.SaveGame();
        }
    }

    public void Close()
    {
        rootPanel.SetActive(false);
        mainChoicePanel.SetActive(false);
        upgradePanel.SetActive(false);
        purificationPanel.SetActive(false);
        currentChest = null;

        GamePauseManager.Instance.Resume("ChestReward");
    }

    //改成1出现技能/属性(技能：属性 = 1:2) 2 （技能：属性 = 1:1） 3 只有属性
    private List<ChestUpgradeOption> GetRandomUpgradeOptions(int count)
    {
        List<ChestUpgradeOption> pool = new List<ChestUpgradeOption>();

        AddAttributeUpgradesToPool(pool);
        AddSkillUpgradesToPool(pool);

        List<ChestUpgradeOption> result = new List<ChestUpgradeOption>();

        //for the first one
        int randomIndex = Random.Range(0, pool.Count + playerStats.GetAttributeUpgrades().Length);
        if (randomIndex < 2 * playerStats.GetAttributeUpgrades().Length)
        {
            result.Add(pool[(int) randomIndex / 2]);
            pool.RemoveAt((int) randomIndex / 2);
        } else
        {
            result.Add(pool[randomIndex - playerStats.GetAttributeUpgrades().Length]);
            pool.RemoveAt(randomIndex - playerStats.GetAttributeUpgrades().Length);
        }
        
        //for the second one
        randomIndex = Random.Range(0, pool.Count);
        result.Add(pool[randomIndex]);
        pool.RemoveAt(randomIndex);

        //for the third one
        while (pool[pool.Count - 1].optionType == ChestUpgradeOptionType.Skill) {
            pool.RemoveAt(pool.Count - 1);
        }
        randomIndex = Random.Range(0, pool.Count);
        result.Add(pool[randomIndex]);
        pool.RemoveAt(randomIndex);
        return result;
    }

    private List<ChestUpgradeOption> GetRandomBetterUpgradeOptions(int count)
    {
        List<ChestUpgradeOption> pool = new List<ChestUpgradeOption>();

        AddSkillUpgradesToPool(pool);
        AddBetterUpgradesToPool(pool);

        List<ChestUpgradeOption> result = new List<ChestUpgradeOption>();

        int num = Mathf.Min(count, pool.Count);

        for (int i = 0; i < num; ++i)
        {
            int randomIndex = Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }
        return result;
    }

    private void AddAttributeUpgradesToPool(List<ChestUpgradeOption> pool)
    {
        AttributeUpgrade[] attributeUpgrades = playerStats.GetAttributeUpgrades();

        foreach (AttributeUpgrade upgrade in attributeUpgrades)
        {
            if (upgrade == null)
                continue;

            ChestUpgradeOption option = new ChestUpgradeOption();
            option.optionType = ChestUpgradeOptionType.Attribute;
            option.attributeUpgrade = upgrade;
            option.playerStats = playerStats;
            option.vfx = vfx;

            pool.Add(option);
        }
    }

    private void AddSkillUpgradesToPool(List<ChestUpgradeOption> pool)
    {
        SkillBase currentSkill = skillManager.GetCurrentSkill();

        if (currentSkill == null)
            return;

        foreach (SkillUpgradeDisplayData skillUpgrade in currentSkill.Upgrades)
        {
            if (skillUpgrade == null)
                continue;

            if (skillUpgrade.unlocked)
                continue;

            ChestUpgradeOption option = new ChestUpgradeOption();
            option.optionType = ChestUpgradeOptionType.Skill;
            option.skill = currentSkill;
            option.skillUpgrade = skillUpgrade;
            option.vfx = vfx;

            pool.Add(option);
        }
    }

    private void AddBetterUpgradesToPool(List<ChestUpgradeOption> pool)
    {
        SkillBase currentSkill = skillManager.GetCurrentSkill();

        if (currentSkill == null)
            return;
        foreach (SkillUpgradeDisplayData skillUpgrade in currentSkill.BetterUpgrades)
        {
            if (skillUpgrade == null)
                continue;

            if (skillUpgrade.unlocked)
                continue;
            if (check(currentSkill, skillUpgrade))
            {
                continue;
            }

            ChestUpgradeOption option = new ChestUpgradeOption();
            option.optionType = ChestUpgradeOptionType.Skill;
            option.skill = currentSkill;
            option.skillUpgrade = skillUpgrade;
            option.vfx = vfx;

            pool.Add(option);
        }
    }

    private bool check(SkillBase skill, SkillUpgradeDisplayData upgrade)
    {
        if (skill.SkillType == SkillType.LifeSteal)
        {
            if (upgrade.upgradeName == "Fixed Time")
            {
                return skill.UnlockedForBetterUpgrade("Fixed Blood");
            }
            if (upgrade.upgradeName == "Fixed Blood")
            {
                return skill.UnlockedForBetterUpgrade("Fixed Time");
            }
        }
        if (skill.SkillType == SkillType.TimeControl)
        {
            if (upgrade.upgradeName == "Fixed Rate")
            {
                return skill.UnlockedForBetterUpgrade("Fixed Time");
            }
            if (upgrade.upgradeName == "Fixed Time")
            {
                return skill.UnlockedForBetterUpgrade("Fixed Rate");
            }
        }
        if (skill.SkillType == SkillType.Phase)
        {
            if (upgrade.upgradeName == "Fixed Time")
            {
                return skill.UnlockedForBetterUpgrade("Fixed Attack");
            }
            if (upgrade.upgradeName == "Fixed Attack")
            {
                return skill.UnlockedForBetterUpgrade("Fixed Time");
            }
        }
        return true;
    }
}