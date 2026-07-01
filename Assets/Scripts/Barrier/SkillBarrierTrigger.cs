using TMPro;
using UnityEngine;

public class SkillBarrierTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Hint")]
    [SerializeField] private string message = "Please choose a skill";

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

        if (!SaveManager.instance.HasChoosenSkill())
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