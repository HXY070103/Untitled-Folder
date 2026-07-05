using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string newGameSceneName = "ChooseSkill";
    [SerializeField] private string tutorialSceneName = "BeginGuide";

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Panels")]
    [SerializeField] private GameObject confirmNewGamePanel;
    [SerializeField] private GameObject settingPanel;

    [Header("Save")]
    [SerializeField] private string saveFileName = "saveData.json";

    private FileDataHandler dataHandler;

    private void Awake()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);

        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }

        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    private void Start()
    {
        RefreshContinueButton();
    }

    private void RefreshContinueButton()
    {
        if (continueButton == null)
        {
            return;
        }

        GameData data = dataHandler.LoadData();

        bool canContinue = data != null && data.isGameInProgess && !string.IsNullOrEmpty(data.currentSceneName);

        continueButton.interactable = canContinue;
    }

    public void OnNewGameButton()
    {
        GameData data = dataHandler.LoadData();

        bool hasUnfinishedGame = data != null && data.isGameInProgess;

        if (hasUnfinishedGame)
        {
            if (confirmNewGamePanel != null)
            {
                confirmNewGamePanel.SetActive(true);
            }

            return;
        }

        StartNewGame();
    }

    public void OnConfirmNewGameButton()
    {
        StartNewGame();
    }

    public void OnCancelNewGameButton()
    {
        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }
    }

    private void StartNewGame()
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.CreateNewGame(newGameSceneName);
        } else
        {
            GameData newData = new GameData();

            newData.isGameInProgess = true;
            newData.hasChosenSkill = false;
            newData.currentSceneName = newGameSceneName;
            newData.playerCurrentHP = -1;
            newData.numOfLevel = 0;
            newData.numOfUse = 0;
            newData.hasPlayerPosition = false;
            dataHandler.SaveData(newData);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.LoadScene(newGameSceneName);
        }
        else
        {
            Debug.LogError("GameManager not found");
        }
    }

    public void OnContinueGameButton()
    {
        GameData data = dataHandler.LoadData();

        if (data == null)
        {
            Debug.LogWarning("No save data found.");
            RefreshContinueButton();
            return;
        }

        if (data.isGameInProgess == false)
        {
            Debug.LogWarning("No unfinished game.");
            RefreshContinueButton();
            return;
        }

        if (string.IsNullOrEmpty(data.currentSceneName))
        {
            Debug.LogWarning("Saved scene name is empty.");
            RefreshContinueButton();
            return;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.LoadSavedScene(data.currentSceneName);
        }
        else
        {
            Debug.LogError("GameManager not found");
        }
    }

    public void OnTutorialButton()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadScene(tutorialSceneName);
        }
        else
        {
            Debug.LogError("GameManager not found");
        }
    }

    public void OnSettingButton()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }
    }

    public void OnCloseSettingButton()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}