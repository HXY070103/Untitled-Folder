using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_UpgradeOption : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;

    private ChestUpgradeOption currentOption;
    private UI_ChestReward rewardUI;

    [Header("Click Effect")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color clickColor = new Color(1f, 0.85f, 0.25f);
    [SerializeField] private float clickEffectDuration = 0.2f;

    public void Setup(ChestUpgradeOption option, UI_ChestReward ui)
    {
        currentOption = option;
        rewardUI = ui;

        titleText.text = option.GetTitle();
        descriptionText.text = option.GetDescription();

        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (currentOption == null)
            return;
        button.interactable = false;

        if (backgroundImage != null)
        {
            backgroundImage.color = clickColor;
        }
        StartCoroutine(SelectAfterDelay());
    }

    private IEnumerator SelectAfterDelay()
    {
        yield return new WaitForSecondsRealtime(clickEffectDuration);
        rewardUI.SelectUpgrade(currentOption);
    }
}