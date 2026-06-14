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
        if (!string.IsNullOrEmpty(gameData.currentSceneName)
            && SceneManager.GetActiveScene().name != gameData.currentSceneName)
        {
            SceneManager.LoadScene(gameData.currentSceneName);

            yield return null;
            yield return new WaitForSeconds(0.1f);
        }
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

    public void SetCheckpoint(Vector3 position)
    {
        if (gameData == null)
        {
            gameData = new GameData();
        }
        gameData.hasCheckpoint = true;
        gameData.checkpointX = position.x;
        gameData.checkpointY = position.y;
        gameData.checkpointZ = position.z;
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
            SaveGame();
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

    public void CreateNewGame(string startSceneName)
    {
        GameData data = new GameData();

        data.isGameInProgess = true;
        data.hasChosenSkill = false;
        data.currentSceneName = startSceneName;
        data.playerCurrentHP = -1;

        dataHandler.SaveData(data);
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
}
