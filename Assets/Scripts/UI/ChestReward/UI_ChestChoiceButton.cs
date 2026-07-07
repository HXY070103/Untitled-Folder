using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChestChoiceButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;

    [Header("Click Effect")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.85f, 0.25f);
    [SerializeField] private float delay = 0.2f;

    private System.Action onSelected;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    public void Setup(System.Action action)
    {
        onSelected = action;

        button.interactable = true;

        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    private void OnClick()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        button.interactable = false;

        if (backgroundImage != null)
            backgroundImage.color = selectedColor;

        StartCoroutine(InvokeAfterDelay());
    }

    private IEnumerator InvokeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delay);

        onSelected?.Invoke();
    }
}