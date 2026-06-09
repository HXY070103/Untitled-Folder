using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
public class SaveManager : MonoBehaviour
{

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> allSaveables;
    [SerializeField] private string fileName = "saveData.json";

    private IEnumerator Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        allSaveables = FindISaveables();
        yield return new WaitForSeconds(.01f);
        LoadGame();
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            gameData = new GameData();
        }
        foreach(var saveable in allSaveables)
        {
            saveable.SaveData(ref gameData);
        }
        dataHandler.SaveData(gameData);
    }

    public void LoadGame()
    {
        gameData = dataHandler.LoadData();
        if (gameData == null)
        {
            Debug.Log("No save data, create a new save");
            gameData = new GameData();
            return;
        }
        foreach(var saveable in allSaveables)
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
        SaveGame();
    }
}
