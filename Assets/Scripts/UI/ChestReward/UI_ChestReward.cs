using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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

    private GameObject player;
    private PlayerVFX vfx;
    private PlayerStats playerStats;
    private PlayerSkillManager skillManager;
    private Chest currentChest;

    private void Awake()
    {
        Instance = this;

        rootPanel.SetActive(false);
        mainChoicePanel.SetActive(false);
        upgradePanel.SetActive(false);
        purificationPanel.SetActive(false);
    }

    public void Open(GameObject playerObj, Chest chest)
    {
        player = playerObj;
        currentChest = chest;

        vfx = player.GetComponent<PlayerVFX>();
        playerStats = player.GetComponent<PlayerStats>();
        skillManager = player.GetComponent<PlayerSkillManager>();

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

        upgradeChoiceButton.Setup(ShowUpgrades);
        purificationChoiceButton.Setup(ShowPurification);
    }

    public void ShowUpgrades()
    {
        mainChoicePanel.SetActive(false);
        purificationPanel.SetActive(false);
        upgradePanel.SetActive(true);

        List<ChestUpgradeOption> options = GetRandomUpgradeOptions(3);

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
        skillManager.GetCurrentSkill().DecreaseDecay(2);

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

    private List<ChestUpgradeOption> GetRandomUpgradeOptions(int count)
    {
        List<ChestUpgradeOption> pool = new List<ChestUpgradeOption>();

        AddAttributeUpgradesToPool(pool);
        AddSkillUpgradesToPool(pool);

        List<ChestUpgradeOption> result = new List<ChestUpgradeOption>();

        int targetCount = Mathf.Min(count, pool.Count);

        for (int i = 0; i < targetCount; i++)
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
}