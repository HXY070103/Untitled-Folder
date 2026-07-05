using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private string transferToScene;
    [Space]
    [SerializeField] private WaypointType waypointType;
    [SerializeField] private WaypointType connectedWaypoint;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTrigger = true;
    [SerializeField] private bool isBossWinExit = false;

    public void SetCanbeTrigger(bool canBeTrigger)
    {
        this.canBeTrigger = canBeTrigger;
    }

    public WaypointType GetWaypointType()
    {
        return waypointType;
    }

    public Vector3 GetRespawnPoint()
    {
        return respawnPoint == null ? transform.position : respawnPoint.position;
    }

    private void OnValidate()
    {
        gameObject.name = "Object_Waypoint - " + waypointType.ToString() + " - " + transferToScene;         
        if (waypointType == WaypointType.Enter)
        {
            connectedWaypoint = WaypointType.Exit;
        }
        if (waypointType == WaypointType.Exit)
        {
            connectedWaypoint = WaypointType.Enter;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTrigger == false)
        {
            return;
        }

        if (collision.GetComponent<Player>() == null)
        {
            return;
        }
        canBeTrigger = false;
        if (isBossWinExit)
        {
            if (GameOverManager.instance != null)
            {
                GameOverManager.instance.ShowWin();
            } else
            {
                Debug.LogError("Cannot find gameover manager");
            }
            return;
        }

        DecideScene();
        if (SaveManager.instance != null && !SaveManager.instance.IsCurrentSceneNoSave())
        {
            SaveManager.instance.SaveGame();
        }
        GameManager.instance.ChangeScene(transferToScene, connectedWaypoint);
        
    }

    private void DecideScene()
    {
        if (transferToScene == "EliteLevel_Repeat")
        {
            transferToScene += RandomChoose(1,4);
        } else if (transferToScene == "EliteLevel_1")
        {
            transferToScene = "EliteLevel" + RandomChoose(1,2);
        } else if (transferToScene == "EliteLevel_2")
        {
            transferToScene = "EliteLevel" + RandomChoose(3,4);
        } else if (transferToScene == "NormalLevel_Repeat")
        {
            transferToScene += RandomChoose(1,4);
        }
        List<string> scenes = GameManager.instance.GetRepeatLevel();
        Debug.Log(transferToScene);
        if (scenes.Contains(transferToScene))
        {
            SaveManager.instance.TrytoRemoveSceneFromCleared(transferToScene);
            SaveManager.instance.TrytoRemoveChestID(transferToScene);
        }
    }

    private string RandomChoose(int minNum, int maxNum)
    {
        int randomIndex = Random.Range(minNum, maxNum + 1);
        return "_" + randomIndex;
    }

    /* can go back by open this method
    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTrigger = true;
    }
    */
}
