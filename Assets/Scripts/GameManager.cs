using Unity.VectorGraphics;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    public void ChangeScene(string sceneName, WaypointType type)
    {
        StartCoroutine(ChangeSceneCo(sceneName, type));
    }

    private IEnumerator ChangeSceneCo(string sceneName, WaypointType waypointType)
    {
        // Fade effect: to be completed

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);

        yield return null;
        yield return new WaitForSeconds(0.2f);

        if (SaveManager.instance != null)
        {
            SaveManager.instance.RefreshSavables();
            SaveManager.instance.LoadDataToCurrentScene();
        }
       
        Vector3 position = GetWaypointPosition(waypointType);

        if (position != Vector3.zero)
        {
            Player.instance.TeleportPlayer(position);
        }

        if (SaveManager.instance != null)
        {
            SaveManager.instance.RefreshSavables();
            SaveManager.instance.SaveGame();
        }
    }

    private Vector3 GetWaypointPosition(WaypointType type)
    {
        var waypoints = FindObjectsByType<Waypoint>(FindObjectsSortMode.None);
        foreach(var point in waypoints)
        {
            if (point.GetWaypointType() == type)
            {
                point.SetCanbeTrigger(false);
                return point.GetRespawnPoint();
            }
        }
        return Vector3.zero;
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
