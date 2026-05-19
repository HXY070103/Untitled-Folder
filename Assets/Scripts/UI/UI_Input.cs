using NUnit.Framework;
using UnityEngine;

public class UI_Input : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject characterUI;
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
        bool nextState = !characterUI.activeSelf;
        characterUI.SetActive(nextState);
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
            characterUI.SetActive(false);
        }
        UpdateGamePausedState();
    }

    private void UpdateGamePausedState()
    {
        bool isAnyUIOpen = (characterUI != null && characterUI.activeSelf) || (optionsUI != null && optionsUI.activeSelf);
        if (isAnyUIOpen)
        {
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
        }
    }
}