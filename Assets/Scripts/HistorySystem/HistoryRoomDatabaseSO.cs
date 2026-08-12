using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HISTORY ROOM DATABASE", menuName = "History/Room Databse")]
public class HistoryRoomDatabaseSO : ScriptableObject
{
    [SerializeField] private List<HistoryRoomConfig> rooms = new List<HistoryRoomConfig>();

    public HistoryRoomConfig GetRoomConfig(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty.");
            return null;
        }
        HistoryRoomConfig room = rooms.Find(room => room != null && room.sceneName == sceneName);
        if (room != null)
        {
            return room;
        }
        return CreateFallbackConfig(sceneName);
    }

    private HistoryRoomConfig CreateFallbackConfig(string sceneName)
    {
        HistoryRoomType roomType;
        if (sceneName.StartsWith("NormalLevel"))
        {
            roomType = HistoryRoomType.Normal;
        } else if (sceneName.StartsWith("BossLevel"))
        {
            roomType = HistoryRoomType.Boss;
        } else if (sceneName.StartsWith("EliteLevel"))
        {
            roomType = HistoryRoomType.Elite;
        } else
        {
            return null;
        }
        return new HistoryRoomConfig
        {
            sceneName = sceneName,
            displayName = sceneName,
            roomType = roomType,
            encounterName = ""
        };
    }

    public bool IsRecordable(string sceneName)
    {
        return GetRoomConfig(sceneName) != null;
    }
}