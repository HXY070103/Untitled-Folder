using TMPro;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "Mainscene";

    private const string PauseReason = "GameOver";

    private void Awake()
    {
        instance = this;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += ShowDeath;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= ShowDeath;
    }

    public void ShowDeath()
    {
        ShowGameOver("You Died");
    }

    public void ShowWin()
    {
        ShowGameOver("You Win");
    }

    private void ShowGameOver(string message)
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.MarkGameOver();
        }

        if (resultText != null)
        {
            resultText.text = message;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.Pause(PauseReason);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    public void ReturnToMainMenu()
    {
        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.Resume(PauseReason);
        }

        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("GameManager not found.");
        }
    }
}