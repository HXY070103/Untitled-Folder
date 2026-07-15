using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HistoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject historyPanel;
    [SerializeField] private GameObject emptyHistoryPanel;
    [SerializeField] private GameObject historyPage;

    [Header("Top Summary")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI deathCountText;
    [SerializeField] private TextMeshProUGUI startTimeText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    [Header("Room Flow")]
    [SerializeField] private Transform roomContent;
    [SerializeField] private UI_HistoryRoomItem roomItemPrefab;
    [SerializeField] private GameObject noRoomsText;
    [SerializeField] ScrollRect roomScrollRect;
    [SerializeField] Sprite normalRoomSprite;
    [SerializeField] Sprite eliteRoomSprite;
    [SerializeField] Sprite bossRoomSprite;
    [SerializeField] Sprite unknownRoomSprite;
    List<UI_HistoryRoomItem> spawnedRoomItem = new List<UI_HistoryRoomItem>();

    [Header("Final Stats")]
    [SerializeField] private TextMeshProUGUI maxHPText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI critChanceText;
    [SerializeField] private TextMeshProUGUI critPowerText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI skillEffectText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;

    [Header("Skill")]
    [SerializeField] private TextMeshProUGUI initialSkillNameText;
    [SerializeField] private Transform upgradeContent;
    [SerializeField] private UI_HistoryUpgradeItem upgradeItemPrefab;
    [SerializeField] private GameObject noUpgradeText;
    [SerializeField] private UI_HistoryTooltip historyTooltip;
    [SerializeField] private ScrollRect upgradeScrollRect;

    private List<UI_HistoryUpgradeItem> spawnedUpgradeItems = new List<UI_HistoryUpgradeItem>();

    [Header("Navigation")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextbutton;
    [SerializeField] private TextMeshProUGUI pageNumberText;

    private List<RunHistoryRecord> completedRun = new List<RunHistoryRecord>();
    private int currentPageIndex = 0;
    private RunHistoryRecord currentRun;

    public void Open()
    {
        completedRun.Clear();
        currentPageIndex = 0;
        if (HistoryManager.instance == null)
        {
            Debug.LogWarning("No history manager");
            ShowEmptyState();
            return;
        }
        List<RunHistoryRecord> runs = HistoryManager.instance.getCompletedRuns();
        completedRun = new List<RunHistoryRecord>(runs);
        RefreshPage();
    }

    public void RefreshPage()
    {
        if (completedRun == null || completedRun.Count == 0)
        {
            ShowEmptyState();
            return;
        }
        currentPageIndex = Mathf.Clamp(currentPageIndex, 0, completedRun.Count - 1);
        emptyHistoryPanel.SetActive(false);
        historyPage.SetActive(true);
        currentRun = completedRun[currentPageIndex];
        pageNumberText.text = (currentPageIndex + 1) + " / " + completedRun.Count;
        UpdateNavigationButton();
        DisplayRun(currentRun);
    }

    private void DisplayRun(RunHistoryRecord run)
    {
        if (run == null)
        {
            ShowEmptyState();
            return;
        }
        DisplaySummary(run);
        DisplayFinalStats(run.finalStats);
        DisplayInitialSkill(run);
        DisplayUpgrade(run);
        DisplayRoom(run);
    }

    private void DisplayRoom(RunHistoryRecord run)
    {
        ClearRoomItem();
        if (run == null || run.rooms == null)
        {
            Debug.LogWarning("Cannot display room icon");
            noRoomsText.SetActive(true);
            return;
        }
        foreach (var room in run.rooms)
        {
            if (ShouldDisplayRoom(room))
            {
                CreateRoomItem(room);
            }
        }
        if (spawnedRoomItem.Count <= 0)
        {
            noRoomsText.SetActive(true);
        } else
        {
            noRoomsText.SetActive(false);
        }
        RectTransform roomContentRect = roomContent as RectTransform;
        if (roomContentRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(roomContentRect);
        }
        Canvas.ForceUpdateCanvases();
        if (roomScrollRect != null)
        {
            roomScrollRect.horizontalNormalizedPosition = 0f;
        }
    }

    private void DisplayUpgrade(RunHistoryRecord run)
    {
        ClearUpgradeItems();
        if (run == null)
        {
            Debug.LogWarning("Cannot display upgrades");
            return;
        }

        List<UpgradeHistoryRecord> upgradeHistory = new List<UpgradeHistoryRecord>(run.skillUpgrades);
        List<UpgradeHistoryRecord> attributeHistory = new List<UpgradeHistoryRecord>(run.attributeUpgrades);
        upgradeHistory.RemoveAll(x => x == null);
        attributeHistory.RemoveAll(x => x == null);
        upgradeHistory.Sort((a, b) => a.acquisitionOrder.CompareTo(b.acquisitionOrder));
        attributeHistory.Sort((a, b) => a.acquisitionOrder.CompareTo(b.acquisitionOrder));
        foreach (var record in upgradeHistory)
        {
            CreateUpgradeItem(record);
        }
        foreach (var record in attributeHistory)
        {
            CreateUpgradeItem(record);
        }
        if (spawnedUpgradeItems.Count <= 0)
        {
            noUpgradeText.SetActive(true);
        } else
        {
            noUpgradeText.SetActive(false);
        }
        // 修复：强化条目是本帧动态创建的，先立即重建 Content，
        // 让 ScrollRect 使用更新后的高度，再把视图重置到顶部。
        RectTransform upgradeContentRect = upgradeContent as RectTransform;
        if (upgradeContentRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(upgradeContentRect);
        }
        Canvas.ForceUpdateCanvases();
        if (upgradeScrollRect != null)
        {
            upgradeScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void CreateUpgradeItem(UpgradeHistoryRecord record)
    {
        if (record == null || upgradeItemPrefab == null)
        {
            Debug.LogWarning("prefab or record is null");
            return;
        }
        UI_HistoryUpgradeItem newItem = Instantiate(upgradeItemPrefab, upgradeContent);
        newItem.Setup(record, historyTooltip);
        spawnedUpgradeItems.Add(newItem);
    }

    private void ClearUpgradeItems()
    {
        foreach (var item in spawnedUpgradeItems)
        {
            if (item != null)
            {
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
            }
        }
        spawnedUpgradeItems.Clear();
        if (historyTooltip != null)
        {
            historyTooltip.Hide();
        }
    }

    private void ClearRoomItem()
    {
        foreach (var room in spawnedRoomItem)
        {
            if (room != null)
            {
                room.gameObject.SetActive(false);
                Destroy(room.gameObject);
            }
        }
        spawnedRoomItem.Clear();
        if (historyTooltip != null)
        {
            historyTooltip.Hide();
        }
    }

    private bool ShouldDisplayRoom(RoomHistoryRecord room)
    {
        if (room == null)
        {
            return false;
        }
        if (room.sceneName == "ChooseSkill" || room.sceneName == "PreBossLevel")
        {
            return false;
        }
        return true;
    }

    private void CreateRoomItem(RoomHistoryRecord room)
    {
        if (room == null || roomItemPrefab == null || roomContent == null)
        {
            Debug.LogWarning("Cannot display room item");
            return;
        }

        UI_HistoryRoomItem historyRoomItem = Instantiate(roomItemPrefab, roomContent);
        historyRoomItem.Setup(room, GetRoomSprite(room.roomType), historyTooltip);
        spawnedRoomItem.Add(historyRoomItem);
    }

    private void DisplayFinalStats(FinalStatsSnapshot stats)
    {
        if (stats == null || !stats.hasValue)
        {
            Debug.LogWarning("Cannot display final stats");
            SetFinalStats(0, 0, 0, 0, 0, 0, 0);
            return;
        }
        SetFinalStats(stats.maxHP, stats.damage, stats.critChance, stats.critPower, stats.armor, stats.skillEffect, stats.attackSpeed);

    }

    private void SetFinalStats(float maxHP, float damage, float critChance, float critPower, float armor, float skillEffect, float attackSpeed)
    {
        maxHPText.text = maxHP.ToString("F1");
        damageText.text = damage.ToString("F1");
        critChanceText.text = critChance.ToString("F1") + "%";
        critPowerText.text = (critPower * 100f).ToString("F1") + "%";
        armorText.text = armor.ToString("F1");
        skillEffectText.text = "x" + skillEffect.ToString("F2");
        attackSpeedText.text = "x" + attackSpeed.ToString("F2");
    }

    private void DisplayInitialSkill(RunHistoryRecord run)
    {
        if (run == null || run.initialSkillName == null)
        {
            Debug.LogWarning("Cannot display skill name");
            initialSkillNameText.text = "-";
            return;
        }
        initialSkillNameText.text = run.initialSkillName;
    }

    private void DisplaySummary(RunHistoryRecord run)
    {
        DisplayResult(run.result);
        startTimeText.text = "Start Time: \n";
        startTimeText.text += FormatStartTime(run.startTimeIso);
        durationText.text = "Game Duration: ";
        durationText.text += FormatDuration(run.accumulatedPlaySeconds);
        deathCountText.text = "Death Count: " + run.deathCount;
    }

    private void DisplayResult(RunHistoryResult result)
    {
        switch(result)
        {
            case RunHistoryResult.Win:
                resultText.text = "WIN";
                resultText.color = winColor;
                break;

            case RunHistoryResult.Lose:
                resultText.text = "LOSE";
                resultText.color = loseColor;
                break;

            default:
                resultText.text = "-";
                resultText.color = Color.white;
                break;
        }
    }

    private string FormatStartTime(string isoTime)
    {
        if (string.IsNullOrWhiteSpace(isoTime))
        {
            return "-";
        }

        if (!DateTimeOffset.TryParse(isoTime, out DateTimeOffset time))
        {
            return "-";
        }

        return time.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private string FormatDuration(double seconds)
    {
        if (seconds < 0)
        {
            seconds = 0;
        }
        TimeSpan span = TimeSpan.FromSeconds(seconds);
        int hours = (int) span.TotalHours;
        return $"{hours:00}:{span.Minutes:00}:{span.Seconds:00}";
    }

    private void UpdateNavigationButton()
    {
        if (currentPageIndex > 0)
        {
            previousButton.interactable = true;
        } else
        {
            previousButton.interactable = false;
        }

        if (currentPageIndex < completedRun.Count - 1)
        {
            nextbutton.interactable = true;
        } else
        {
            nextbutton.interactable = false;
        }
    }

    public void ShowPreviousPanel()
    {
        if (completedRun == null || completedRun.Count == 0)
        {
            return;
        }
        if (currentPageIndex <= 0)
        {
            return;
        }
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        currentPageIndex--;
        RefreshPage();
    }

    public void ShowNextPage()
    {
        if (completedRun == null || completedRun.Count == 0)
        {
            return;
        }
        if (currentPageIndex >= completedRun.Count - 1)
        {
            return;
        }
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        currentPageIndex++;
        RefreshPage();
    }

    public void CloseHistoryPanel()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        if (historyPanel == null)
        {
            Debug.LogWarning("Not assign history panel");
            return;
        }
        historyPanel.SetActive(false);
        historyTooltip.Hide();
    }

    public void ShowEmptyState()
    {
        historyTooltip.Hide();
        ClearRoomItem();
        ClearUpgradeItems();
        emptyHistoryPanel.SetActive(true);
        historyPage.SetActive(false);
        pageNumberText.text = "0 / 0";
        previousButton.interactable = false;
        nextbutton.interactable = false;
        currentRun = null;
    }

    private Sprite GetRoomSprite(HistoryRoomType historyRoomType)
    {
        switch(historyRoomType)
        {
            case HistoryRoomType.Normal:
                return normalRoomSprite;
            case HistoryRoomType.Elite:
                return eliteRoomSprite;
            case HistoryRoomType.Boss:
                return bossRoomSprite;
            default:
                return unknownRoomSprite;
        }
    }
}
