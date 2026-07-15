using Unity.VisualScripting;
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
    [SerializeField] private UI_Options uI_Options;

    [Header("Save")]
    [SerializeField] private string saveFileName = "saveData.json";

    [Header("History")]
    [SerializeField] private GameObject historyPanel;
    [SerializeField] private UI_HistoryPanel uI_HistoryPanel;   

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
        uI_Options.LoadUpVolume();
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

    public void OpenHistoryPanel()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        if (historyPanel == null)
        {
            Debug.LogWarning("Not assign history panel");
            return;
        }
        historyPanel.SetActive(true);
        if (uI_HistoryPanel == null)
        {
            Debug.LogWarning("Not assign history UI panel");
            return;
        }
        uI_HistoryPanel.Open();
    }

    public void OnNewGameButton()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
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
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        StartNewGame();
    }

    public void OnCancelNewGameButton()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        if (confirmNewGamePanel != null)
        {
            confirmNewGamePanel.SetActive(false);
        }
    }

    private void StartNewGame()
    {
        PrepareHistoryForNewGame();

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

    private void PrepareHistoryForNewGame()
    {
        if (HistoryManager.instance == null)
        {
            Debug.LogWarning("No History Manager");
            return;
        }
        if (HistoryManager.instance.HasActiveRun())
        {
            bool finizeSucceed = HistoryManager.instance.FinalizeLose(null);
            if (!finizeSucceed)
            {
                Debug.LogWarning("Fail to finalize the previous history run");
            }
        }
        bool startSucceed = HistoryManager.instance.StartNewRun();
        if (!startSucceed)
        {
            Debug.LogWarning("Fail to creat a new history run");
        }
    }

    public void OnContinueGameButton()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
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
        AudioManager.instance.PlayGlobalSFX("Button_Click");
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
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }
    }

    public void OnCloseSettingButton()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    public void OnExitButton()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}