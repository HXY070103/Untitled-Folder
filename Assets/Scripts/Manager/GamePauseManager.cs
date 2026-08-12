using System.Collections.Generic;
using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }
    private HashSet<string> pauseReasons = new HashSet<string>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Pause(string reason)
    {
        pauseReasons.Add(reason);
        Time.timeScale = 0f;
    }

    public void Resume(string reason)
    {
        pauseReasons.Remove(reason);

        if (pauseReasons.Count == 0)
        {
            Time.timeScale = 1f;
        }
    }

    public bool IsPausedBy(string reason)
    {
        return pauseReasons.Contains(reason);
    }

    public bool IsGamePaused()
    {
        return pauseReasons.Count > 0;
    }

    public void ClearAllPauses()
    {
        pauseReasons.Clear();
        Time.timeScale = 1f;
    }
}
