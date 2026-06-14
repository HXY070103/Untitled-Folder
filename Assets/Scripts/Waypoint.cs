using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private string transferToScene;
    [Space]
    [SerializeField] private WaypointType waypointType;
    [SerializeField] private WaypointType connectedWaypoint;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTrigger = true;

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
        GameManager.instance.ChangeScene(transferToScene, connectedWaypoint);
        
    }

    /* cannot go back
    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTrigger = true;
    }
    */
}
