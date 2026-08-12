using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private string transferToScene;
    [Space]
    [SerializeField] private WaypointType waypointType;
    [SerializeField] private WaypointType connectedWaypoint;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTrigger = true;
    [SerializeField] private bool isBossWinExit = false;

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    private Player playerInRange;

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

        Player player = collision.GetComponent<Player>();

        if (player == null)
        {
            return;
        }

        playerInRange = player;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!canBeTrigger)
        {
            return;
        }

        Player player = collision.GetComponent<Player>();

        if (player == null)
        {
            return;
        }

        if (player == playerInRange) 
        {
            playerInRange = null;
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx) 
    {
        if (playerInRange == null)
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

    private void OnEnable() {
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    private void OnDisable() 
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }
}
