using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_HistoryUpgradeItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private UpgradeHistoryRecord upgradeHistory;
    [SerializeField] private UI_HistoryTooltip tooltip;

    public void Setup(UpgradeHistoryRecord record, UI_HistoryTooltip tooltip)
    {
        upgradeHistory = record;
        this.tooltip = tooltip;
        if (record == null)
        {
            upgradeNameText.text = "-";
            return;
        }
        if (record.count <= 1)
        {
            upgradeNameText.text = record.upgradeName;
        } else
        {
            upgradeNameText.text = record.upgradeName + " x " + record.count;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (upgradeHistory == null || tooltip == null)
        {
            Debug.LogWarning("cannot show tooltip here");
            return;
        }
        tooltip.Show(upgradeHistory.upgradeName, upgradeHistory.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip == null)
        {
            return;
        }
        tooltip.Hide();
    }

    public void OnDisable()
    {
        if (tooltip == null)
        {
            return;
        }
        tooltip.Hide();
    }

}