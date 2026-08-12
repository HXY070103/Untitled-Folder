using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class UI_UpgradeOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;

    private ChestUpgradeOption currentOption;
    private UI_ChestReward rewardUI;
    private Color currentBaseColor;

    [Header("Click Effect")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color betterColor = Color.green;
    [SerializeField] private Color highlightColor = new Color(1f, 0.85f, 0.25f);
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
            if (option.optionType == ChestUpgradeOptionType.Skill && option.isBetter)
            {
                currentBaseColor = betterColor;
            }
            else
            {
                currentBaseColor = normalColor;
            }
            backgroundImage.color = currentBaseColor;
        }
        button.interactable = true;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null && button.interactable)
        {
            backgroundImage.color = highlightColor;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null && button.interactable)
        {
            backgroundImage.color = currentBaseColor;
        }
    }

    private void OnClick()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
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