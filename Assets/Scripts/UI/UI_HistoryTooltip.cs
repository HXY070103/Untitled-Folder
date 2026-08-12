using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_HistoryTooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Vector2 mouseOffset = new Vector2(20, -20);

    public void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    public void Show(string title, string description)
    {
        titleText.text = title;
        if (string.IsNullOrEmpty(description))
        {
            descriptionText.text = "No description";
        } else
        {
            descriptionText.text = description;
        }
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.SetAsLastSibling();
        UpdateTooltipPosition();
    }

    private void UpdateTooltipPosition()
    {
        Vector2 position = (Vector2) Input.mousePosition + mouseOffset;
        float width = tooltipRect.rect.width * tooltipRect.lossyScale.x;
        float height = tooltipRect.rect.height * tooltipRect.lossyScale.y;
        float finalx = Mathf.Clamp(position.x, 10, Screen.width - width - 10);
        float finaly = Mathf.Clamp(position.y, height + 10, Screen.height - 10);
        tooltipRect.position = new Vector2(finalx, finaly);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}