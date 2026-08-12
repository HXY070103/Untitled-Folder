using TMPro;
using UnityEngine;
using System.Collections;
using System;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private float waitSecondsDead = 1f;

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
        Player.OnPlayerDeath += PlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= PlayerDeath;
    }

    private void PlayerDeath()
    {
        StartCoroutine(DelayCall(waitSecondsDead, ShowDeath));
    }

    private IEnumerator DelayCall(float decay, Action callback)
    {
        yield return new WaitForSeconds(decay);
        callback?.Invoke();
    }

    public void ShowDeath()
    {
        ShowGameOver("You Died", false);
    }

    public void ShowWin()
    {
        if(HistoryManager.instance != null)
        {
            HistoryManager.instance.FinalizeHistoryWin();
        }
        ShowGameOver("You Win", true);
    }

    private void ShowGameOver(string message, bool finishGame)
    {
        if (finishGame && SaveManager.instance != null)
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