using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelClearManager : MonoBehaviour, ISaveable
{
    public static LevelClearManager instance { get; private set; }

    [Header("Barrier")]
    [SerializeField] private Barrier barrier;

    [Header("Enemy Count")]
    [SerializeField] private int remainingEnemies = -1;
    [SerializeField] private int pendingEnemies = 0;

    private bool isLevelClear;
    private string currentSceneName;

    public bool IsLevelClear => isLevelClear;

    private void Awake()
    {
        instance = this;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnEnable()
    {
        EnemyHealth.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyDeath -= HandleEnemyDeath;
    }

    private IEnumerator Start()
    {
        yield return null;
        currentSceneName = SceneManager.GetActiveScene().name;

        if (SaveManager.instance != null && SaveManager.instance.IsSceneCleared(currentSceneName))
        {
            remainingEnemies = 0;
            RemoveAllEnemiesInScene();
            SetLevelClear(true, false);
            yield break;
        }
        UpdateRemainingEnemy();

        if (remainingEnemies <= 0)
        {
            SetLevelClear(true, false);
        }
        else
        {
            SetLevelClear(false, false);
        }
    }

    public void UpdateRemainingEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
        remainingEnemies = enemies.Length;
        pendingEnemies = 0;
    }

    private void HandleEnemyDeath()
    {
        if (isLevelClear)
        {
            return;
        }

        remainingEnemies--;

        if (remainingEnemies <= 0)
        {
            remainingEnemies = 0;
        }

        if (remainingEnemies + pendingEnemies <= 0)
        {
            SetLevelClear(true, true);
        }
    }

    public void SetLevelClear(bool clear, bool saveAfterClear)
    {
        bool justBecameClear = clear && !isLevelClear;
        isLevelClear = clear;

        if (isLevelClear)
        {
            if (barrier != null)
            {
                barrier.OpenBarrier();
            }

            // Record history only when the room changes from uncleared to cleared.
            if (justBecameClear)
            {
                NotifyHistoryRoomCleared();
            }

            if (saveAfterClear && SaveManager.instance != null)
            {
                SaveManager.instance.SaveGame();
            }
        }
        else
        {
            if (barrier != null)
            {
                barrier.CloseBarrier();
            }
        }
    }

    public int GetRemainingEnemies()
    {
        return remainingEnemies;
    }

    public void SetRemainingEnemies(int x)
    {
        remainingEnemies = x;
    }

    private void RemoveAllEnemiesInScene()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (EnemyHealth enemyHealth in enemies)
        {
            Destroy(enemyHealth.gameObject);
        }
    }

    public void RegisterSpawnedEnemy()
    {
        if (remainingEnemies == -1)
        {
            return;
        }
        remainingEnemies++;
        if (pendingEnemies > 0)
        {
            pendingEnemies--;
        }
        if (!isLevelClear)
        {
            return;
        }
        SetLevelClear(false, false);
    }

    public void AddPendingEnemies(int count)
    {
        if (count <= 0 || remainingEnemies == -1)
        {
            return;
        }
        pendingEnemies += count;
    }

    private void NotifyHistoryRoomCleared()
    {
        if (HistoryManager.instance == null)
        {
            return;
        }
        if (!HistoryManager.instance.HasActiveRun())
        {
            return;
        }
        string sceneName = SceneManager.GetActiveScene().name;
        bool isSucceed = HistoryManager.instance.RecordRoomCleared(sceneName);
        if (!isSucceed)
        {
            Debug.LogWarning("cannot save room clear in history");
        }
    }

    public void LoadData(GameData data)
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (data.clearedSceneNames.Contains(currentSceneName))
        {
            remainingEnemies = 0;
            RemoveAllEnemiesInScene();
            SetLevelClear(true, false);
        }
    }

    public void SaveData(ref GameData data)
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (isLevelClear && !data.clearedSceneNames.Contains(currentSceneName))
        {
            data.clearedSceneNames.Add(currentSceneName);
        }
    }
}
