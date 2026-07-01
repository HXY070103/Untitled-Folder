using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
public class SaveManager : MonoBehaviour
{

    public static SaveManager instance;
    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> allSaveables;
    [SerializeField] private string fileName = "saveData.json";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        RefreshSavables();
        yield return new WaitForSeconds(.01f);
        LoadGame();
    }

    public void RefreshSavables()
    {
        allSaveables = FindISaveables();       
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            gameData = new GameData();
        }
        gameData.currentSceneName = SceneManager.GetActiveScene().name;

        RefreshSavables();
        foreach(var saveable in allSaveables)
        {
            saveable.SaveData(ref gameData);
        }
        dataHandler.SaveData(gameData);
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameCo());
    }

    public GameData LoadRawGameData()
    {
        if (dataHandler == null)
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }
        return dataHandler.LoadData();
    }

    private IEnumerator LoadGameCo()
    {
        gameData = dataHandler.LoadData();
        if (gameData == null)
        {
            Debug.Log("No save data, create a new save");
            gameData = new GameData();
            gameData.currentSceneName = SceneManager.GetActiveScene().name;
            yield break;
        }
        
        yield return null;
        yield return new WaitForSeconds(0.1f);
    
        RefreshSavables();
        foreach(var saveable in allSaveables)
        {
            saveable.LoadData(gameData);
        }
    }

    public void LoadDataToCurrentScene()
    {
        if (gameData == null)
        {
            gameData = dataHandler.LoadData();
        }

        gameData = dataHandler.LoadData();

        if (gameData == null)
        {
            Debug.LogWarning("No game data to load to current scene.");
            return;
        }

        RefreshSavables();
        foreach (var saveable in allSaveables)
        {
            saveable.LoadData(gameData);
        }
    }

    [ContextMenu("***Delate save data***")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataHandler.Delete();
    }

    private List<ISaveable> FindISaveables()
    {
        return 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .ToList();
    }

    private void OnApplicationQuit()
    {
        if (dataHandler != null)
        {
            SaveGameIfAllowed();
        }
    }

    public bool HasUnfinishedGame()
    {
        GameData data = LoadRawGameData();
        if (data == null)
        {
            return false;
        }
        return data.isGameInProgess;
    }

    public string GetSavedSceneName()
    {
        GameData data = LoadRawGameData();
        if (data == null)
        {
            return "";
        }
        return data.currentSceneName;
    }

    public bool IsSceneCleared(string sceneName)
    {
        if (dataHandler == null)
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        if (gameData == null)
        {
            gameData = dataHandler.LoadData();
        }

        if (gameData == null)
        {
            return false;
        }

        return gameData.clearedSceneNames.Contains(sceneName);
    }

    public bool HasSavedPlayerPosition()
    {
        if (dataHandler == null)
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        if (gameData == null)
        {
            gameData = dataHandler.LoadData();
        }

        return gameData != null && gameData.hasPlayerPosition;
    }

    public Vector3 GetSavedPlayerPosition()
    {
        if (dataHandler == null)
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        if (gameData == null)
        {
            gameData = dataHandler.LoadData();
        }

        if (gameData == null || !gameData.hasPlayerPosition)
        {
            return Vector3.zero;
        }

        return new Vector3(
            gameData.playerPositionX,
            gameData.playerPositionY,
            gameData.playerPositionZ
        );
    }

    public void CreateNewGame(string startSceneName)
    {
        gameData = new GameData();

        gameData.isGameInProgess = true;
        gameData.hasChosenSkill = false;
        gameData.currentSceneName = startSceneName;
        gameData.playerCurrentHP = -1;
        gameData.numOfLevel = 0;
        gameData.numOfUse = 0;
        gameData.hasPlayerPosition = false;

        dataHandler.SaveData(gameData);
    }

    public void MarkGameOver()
    {
        EnsureGameDataLoaded();

        gameData.isGameInProgess = false;
        gameData.currentSceneName = "";
        gameData.hasPlayerPosition = false;
        gameData.playerCurrentHP = -1;

        dataHandler.SaveData(gameData);
    }
    public void MarkInitialSkillChosen()
    {
        if (dataHandler == null)
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        if (gameData == null)
        {
            gameData = LoadRawGameData();
        }

        if (gameData == null)
        {
            gameData = new GameData();
        }

        gameData.isGameInProgess = true;
        gameData.hasChosenSkill = true;
    }

    public bool HasChoosenSkill()
    {
        if (dataHandler == null)
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }
        if (gameData == null)
        {
            gameData = dataHandler.LoadData();
        }
        if (gameData == null)
        {
            return false;
        }
        return gameData.hasChosenSkill;
    }

    public void DestorySelf()
    {
        if (instance == this)
        {
            instance = null;
        }
        Destroy(gameObject);
    }

    public void IncreaseNumOfLevel()
    {
        EnsureGameDataLoaded();
        gameData.numOfLevel ++;
        dataHandler.SaveData(gameData);
    }

    public int GetNumOfLevel()
    {
        EnsureGameDataLoaded();
        return gameData.numOfLevel;
    }

    private void EnsureGameDataLoaded()
    {
        if (dataHandler == null)
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        if (gameData == null)
        {
            gameData = dataHandler.LoadData();
        }

        if (gameData == null)
        {
            gameData = new GameData();
        }
    }

    public void SaveGameIfAllowed()
    {
        if (!CanSaveCurrentSceneProgress())
        {
            Debug.Log("Current level is not cleared. Progress will not be saved.");
            return;
        }

        SaveGame();
    }

    public bool CanSaveCurrentSceneProgress()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // 主菜单、选技能场景这种没有 LevelClearManager 的场景，允许保存
        LevelClearManager levelClearManager = FindFirstObjectByType<LevelClearManager>();
        if (levelClearManager == null)
        {
            return true;
        }
        if (IsSceneCleared(sceneName))
        {
            return true;
        }

        return false;
    }

    public void TrytoRemoveSceneFromCleared(string sceneName)
    {
        if (gameData.clearedSceneNames.Contains(sceneName))
        {
            gameData.clearedSceneNames.Remove(sceneName);
        }
    }

    public void TrytoRemoveChestID(string sceneName)
    {
        if (!sceneName.Contains("Repeat"))
        {
            Debug.LogWarning("enter unrepeated room");
            return;
        }
        char c = sceneName[sceneName.Length - 1];
        string chestID = "Chest_Repeat_";
        if (sceneName.Contains("Normal"))
        {
            chestID += "00" + c;
        } else
        {
            chestID += "10" + c;
        }
        if (gameData.openedChestID.Contains(chestID))
        {
            gameData.openedChestID.Remove(chestID);
        } else
        {
            Debug.Log(chestID);
        }
    }
}
