using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text;

public class UI_HistoryRoomItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image roomIcon;
    [SerializeField] private UI_HistoryTooltip tooltip;
    private RoomHistoryRecord roomHistory;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (roomHistory == null || tooltip == null)
        {
            Debug.LogWarning("Cannot show tooltip");
            return;
        }
        string title = GetTooltipTitle();
        string description = BuildTooltipDescription();
        tooltip.Show(title, description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }
    }

    public void OnDisable()
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }
    }

    public void Setup(RoomHistoryRecord roomHistoryRecord, Sprite icon, UI_HistoryTooltip tooltip)
    {
        roomHistory = roomHistoryRecord;
        this.tooltip = tooltip;
        if (roomHistoryRecord == null)
        {
            roomIcon.gameObject.SetActive(false);
            return;
        }
        roomIcon.gameObject.SetActive(true);
        if (icon != null)
        {
            roomIcon.sprite = icon;
        }
    }

    private string BuildTooltipDescription()
    {
        if (roomHistory.giveUpHere || !roomHistory.cleared)
        {
            return "Give up here";
        }
        if (roomHistory.rewards == null || roomHistory.rewards.Count == 0)
        {
            return "Cleared \n No reward recorded";
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("Cleared\n");
        foreach (var upgrade in roomHistory.rewards)
        {
            if (upgrade == null)
            {
                continue;
            }
            if (upgrade.upgradeType == HistoryUpgradeType.Skill)
            {
                sb.Append("Skill Upgrade:\n").Append(upgrade.upgradeName);
                if (upgrade.count > 1)
                {
                    sb.Append(" x" + upgrade.count);
                }
            }
            if (upgrade.upgradeType == HistoryUpgradeType.Attribute)
            {
                sb.Append("Attribute Upgrade:\n").Append(upgrade.description);
                if (upgrade.count > 1)
                {
                    sb.Append(" x" + upgrade.count);
                }
            }
            if (upgrade.upgradeType == HistoryUpgradeType.Purification)
            {
                sb.Append("Purification: Decay " + upgrade.description);
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

    private string GetTooltipTitle()
    {
        if (roomHistory.roomType == HistoryRoomType.Normal)
        {
            if (!string.IsNullOrWhiteSpace(roomHistory.displayName))
            {
                return roomHistory.displayName;
            } else if (!string.IsNullOrWhiteSpace(roomHistory.sceneName))
            {
                return roomHistory.sceneName;
            } else
            {
                return "Normal Room";
            }
        } else
        {
            if (!string.IsNullOrWhiteSpace(roomHistory.encounterName))
            {
                return roomHistory.encounterName;
            } else if (!string.IsNullOrWhiteSpace(roomHistory.displayName))
            {
                return roomHistory.displayName;
            } else if (!string.IsNullOrWhiteSpace(roomHistory.sceneName))
            {
                return roomHistory.sceneName;
            } else
            {
                return "Elite/Boss Room";
            }
        }
    }
}