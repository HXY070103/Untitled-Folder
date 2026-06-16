using TMPro;
using UnityEngine;

public class BarrierHintTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Hint")]
    [SerializeField] private string message = "Defeat all enemies to continue.";

    private void Start()
    {
        HideHint();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<Player>())
        {
            return;
        }

        if (LevelClearManager.instance == null)
        {
            return;
        }

        if (!LevelClearManager.instance.IsLevelClear)
        {
            ShowHint();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.GetComponent<Player>())
        {
            return;
        }

        HideHint();
    }

    private void ShowHint()
    {
        if (hintText != null)
        {
            hintText.text = message;
        }

        if (hintPanel != null)
        {
            hintPanel.SetActive(true);
        }
    }

    private void HideHint()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }
}