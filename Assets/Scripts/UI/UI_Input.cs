using UnityEngine;

public class UI_Input : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private UI_Character characterUI;
    [SerializeField] private GameObject optionsUI;
    private PlayerInputSet input;

    private void Awake()
    {
        input = new PlayerInputSet();

        input.UI.CharacterUI.performed += ctx => ToggleCharacterUI();
        input.UI.OptionsUI.performed += ctx => ToggleOptionsUI();
    }

    private void OnEnable()
    {
        input.UI.Enable();
    }
    private void OnDisable()
    {
        input.UI.Disable();
    }

    private void ToggleCharacterUI()
    {
        if (characterUI == null)
        {
            return;
        }
        bool nextState = !characterUI.gameObject.activeSelf;
        characterUI.ToggleCharacterUI();
        
        if (nextState && optionsUI != null)
        {
            optionsUI.SetActive(false);
        }
        UpdateGamePausedState();
    }
    private void ToggleOptionsUI()
    {
        if (optionsUI == null) 
        {
            return;
        }
        bool nextState = !optionsUI.activeSelf;
        optionsUI.SetActive(nextState);
        if (nextState && characterUI != null)
        {
            characterUI.gameObject.SetActive(false);
        }
        UpdateGamePausedState();
    }

    private void UpdateGamePausedState()
    {
        bool isAnyUIOpen = (characterUI != null && characterUI.gameObject.activeSelf) || (optionsUI != null && optionsUI.activeSelf);
        if (isAnyUIOpen)
        {
            GamePauseManager.Instance.Pause("UI_Input");
        }
        else
        {
            GamePauseManager.Instance.Resume("UI_Input");
        }
    }
}