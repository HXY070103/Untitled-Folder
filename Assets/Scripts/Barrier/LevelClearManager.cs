using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelClearManager : MonoBehaviour, ISaveable
{
    public static LevelClearManager instance { get; private set; }

    [Header("Barrier")]
    [SerializeField] private Barrier barrier;

    [Header("Enemy Count")]
    [SerializeField] private int remainingEnemies;

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
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
        remainingEnemies = enemies.Length;

        if (remainingEnemies <= 0)
        {
            SetLevelClear(true, false);
        }
        else
        {
            SetLevelClear(false, false);
        }
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
            SetLevelClear(true, true);
        }
    }

    private void SetLevelClear(bool clear, bool saveAfterClear)
    {
        isLevelClear = clear;

        if (isLevelClear)
        {
            if (barrier != null)
            {
                barrier.OpenBarrier();
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