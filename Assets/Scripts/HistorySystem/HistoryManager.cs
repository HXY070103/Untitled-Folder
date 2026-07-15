using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HistoryManager : MonoBehaviour
{
    public static HistoryManager instance;
    private HistoryDataHandler historyDataHandler;
    private HistoryData historyData;
    [SerializeField] private string fileName = "historyData.json";

    [Header("Room Config")]
    [SerializeField] private HistoryRoomDatabaseSO roomDatabase;

    private bool applicationHasFocus = true;
    private bool applicationIsPaused;
    private bool skipNextTimingFrame;

    private bool skipNextSavedRoom;
    private string savedRoomToReuse = "";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        historyDataHandler = new HistoryDataHandler(Application.persistentDataPath, fileName);
        historyData = historyDataHandler.LoadData();
        if (historyData == null)
        {
            historyData = new HistoryData();
        }
    }

    private void Update()
    {
        // The first frame after returning can contain the time spent outside the game.
        if (skipNextTimingFrame)
        {
            skipNextTimingFrame = false;
            return;
        }
        if (!ShouldCountPlayTime())
        {
            return;
        }
        historyData.activeRun.accumulatedPlaySeconds += Time.unscaledDeltaTime;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void OnApplicationFocus(bool focus)
    {
        applicationHasFocus = focus;
        if (!focus && HasActiveRun())
        {
            SaveHistory();
            return;
        }
        if (focus)
        {
            skipNextTimingFrame = true;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        applicationIsPaused = pause;
        if (pause)
        {
            if (HasActiveRun())
            {
                SaveHistory();
            }
            return;
        }
        else
        {
            skipNextTimingFrame = true;
        }
    }

    private void OnApplicationQuit()
    {
        if (!HasActiveRun())
        {
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Mainscene" || currentSceneName == "BeginGuide")
        {
            SaveHistory();
            return;
        }
        RecordCurrentStatsBeforeExit();
    }

    private bool ShouldCountPlayTime()
    {
        if (!HasActiveRun())
        {
            return false;
        }
        if (!applicationHasFocus || applicationIsPaused || !Application.isFocused)
        {
            return false;
        }
        if (SceneManager.GetActiveScene().name =="Mainscene" || SceneManager.GetActiveScene().name == "BeginGuide")
        {
            return false;
        }
        if (Time.timeScale <= 0f)
        {
            return false;
        }
        if (GamePauseManager.Instance != null && GamePauseManager.Instance.IsGamePaused())
        {
            return false;
        }
        return true;
    }

    public void PrepareForSavedSceneLoad(string sceneName)
    {
        skipNextSavedRoom = true;
        savedRoomToReuse = sceneName;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        string sceneName = scene.name;

        if (ShouldReuseLastRoom(sceneName))
        {
            Debug.Log("Reuse existing history room: " + sceneName);
            return;
        }

        if (!HasActiveRun())
        {
            return;
        }

        if (roomDatabase == null)
        {
            Debug.LogWarning("History room database is not assigned.");
            return;
        }

        HistoryRoomConfig config = roomDatabase.GetRoomConfig(sceneName);
        if (config == null)
        {
            return;
        }

        if (!RecordRoomEntered(
                config.sceneName,
                config.displayName,
                config.roomType,
                config.encounterName))
        {
            Debug.LogWarning("Failed to record room entry: " + sceneName);
        }
    }

    private bool ShouldReuseLastRoom(string loadedSceneName)
    {
        if (!skipNextSavedRoom)
        {
            return false;
        }

        bool isExpectedSavedScene = savedRoomToReuse == loadedSceneName;

        // The flag is for one scene load only, even if the loaded scene is unexpected.
        skipNextSavedRoom = false;
        savedRoomToReuse = "";

        if (!isExpectedSavedScene || !HasActiveRun())
        {
            return false;
        }

        List<RoomHistoryRecord> rooms = historyData.activeRun.rooms;
        if (rooms == null || rooms.Count == 0)
        {
            return false;
        }

        RoomHistoryRecord lastRoom = rooms[^1];
        return lastRoom != null && lastRoom.sceneName == loadedSceneName;
    }

    public bool StartNewRun()
    {
        if (historyDataHandler == null)
        {
            Debug.LogWarning("historyDataHandler is not initialized");
            return false;
        }
        if (HasActiveRun())
        {
            return false;
        }
        RunHistoryRecord newRun = new RunHistoryRecord();
        newRun.runId = Guid.NewGuid().ToString("N");
        newRun.startTimeIso = DateTimeOffset.Now.ToString("O");
        newRun.endTimeIso = "";
        newRun.accumulatedPlaySeconds = 0;
        newRun.result = RunHistoryResult.InProgress;
        newRun.deathCount = 0;
        newRun.initialSkillName = "";
        newRun.initialSkillDescription = "";
        newRun.finalStats.hasValue = false;
        historyData.activeRun = newRun;
        return SaveHistory();
    }

    public bool RecordInitialSkill(string skillName, string skillDescription)
    {
        if (!HasActiveRun())
        {
            return false;
        }
        historyData.activeRun.initialSkillName = skillName;
        historyData.activeRun.initialSkillDescription = skillDescription;
        return SaveHistory();
    }

    public bool RecordRoomEntered(string sceneName, string displayName, HistoryRoomType roomType, string encounterName)
    {
        if (!HasActiveRun())
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Cannot record a room without a scene name.");
            return false;
        }
        if (historyData.activeRun.rooms == null)
        {
            historyData.activeRun.rooms = new List<RoomHistoryRecord>();
        }
        RoomHistoryRecord newRoom = new RoomHistoryRecord();
        newRoom.sceneName = sceneName;
        newRoom.displayName = string.IsNullOrWhiteSpace(displayName) ? sceneName : displayName;
        newRoom.roomType = roomType;
        newRoom.encounterName = encounterName ?? "";
        newRoom.cleared = false;
        newRoom.giveUpHere = false;
        historyData.activeRun.rooms.Add(newRoom);
        return SaveHistory();
    }

    public bool RecordRoomCleared(string sceneName)
    {
        if (!HasActiveRun())
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Cannot record a room cleared without a scene name.");
            return false;
        }
        if (historyData.activeRun.rooms == null || historyData.activeRun.rooms.Count == 0)
        {
            Debug.LogWarning("No current room to mark as cleared.");
            return false;
        }
        RoomHistoryRecord room = historyData.activeRun.rooms
            .FindLast(room => room != null && room.sceneName == sceneName);
        if (room == null)
        {
            Debug.LogWarning("Cannot find room: " + sceneName);
            return false;
        }
        if (room.cleared)
        {
            return true; // Already marked as cleared, no need to save again
        }
        room.cleared = true;
        return SaveHistory();
    }

    public bool RecordUpgrade(string upgradeName, string description, HistoryUpgradeType upgradeType)
    {
        if (!HasActiveRun())
        {
            return false;
        }
        if (upgradeType != HistoryUpgradeType.Skill && upgradeType != HistoryUpgradeType.Attribute)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(upgradeName))
        {
            Debug.LogWarning("Cannot record an upgrade without a name");
            return false;
        }
        if (historyData.activeRun.rooms == null || historyData.activeRun.rooms.Count == 0)
        {
            Debug.LogWarning("No current room");
            return false;
        }
        if (historyData.activeRun.rooms[^1] == null)
        {
            Debug.LogWarning("Current room data is null");
            return false;
        }
        int acquisitionOrder = GetNextAcquisitionOrder(historyData.activeRun);
        AddUpgradeToRoom(upgradeName, description, upgradeType, acquisitionOrder);
        AddUpgradeToRun(upgradeName, description, upgradeType, acquisitionOrder);
        return SaveHistory();
    }

    private void AddUpgradeToRun(string upgradeName, string description, HistoryUpgradeType upgradeType, int acquisitionOrder)
    {
        List<UpgradeHistoryRecord> targetList;
        if (upgradeType == HistoryUpgradeType.Skill)
        {
            targetList = historyData.activeRun.skillUpgrades;
        } else
        {
            targetList = historyData.activeRun.attributeUpgrades;
        }
        UpgradeHistoryRecord existingUpgrade = targetList.Find(upgrade => upgrade != null && upgrade.upgradeName == upgradeName);
        if (existingUpgrade != null)
        {
            existingUpgrade.count++;
            return;
        }
        UpgradeHistoryRecord newUpgrade = new UpgradeHistoryRecord();
        newUpgrade.upgradeName = upgradeName;
        newUpgrade.description = description;
        newUpgrade.upgradeType = upgradeType;
        newUpgrade.acquisitionOrder = acquisitionOrder;
        newUpgrade.count = 1;
        targetList.Add(newUpgrade);
    }

    private void AddUpgradeToRoom(string upgradeName, string description, HistoryUpgradeType upgradeType, int acquisitionOrder)
    {
        RoomHistoryRecord currentRoom = historyData.activeRun.rooms[^1];
        if (currentRoom.rewards == null)
        {
            currentRoom.rewards = new List<UpgradeHistoryRecord>();
        }
        int index = currentRoom.rewards.FindIndex(reward => reward != null && reward.upgradeName == upgradeName);
        if (index == -1) // first get this upgrade
        {
            UpgradeHistoryRecord newUpgrade = new UpgradeHistoryRecord();
            newUpgrade.upgradeName = upgradeName;
            newUpgrade.description = description;
            newUpgrade.upgradeType = upgradeType;
            newUpgrade.acquisitionOrder = acquisitionOrder;
            newUpgrade.count = 1;
            currentRoom.rewards.Add(newUpgrade);
            return;
        }
        currentRoom.rewards[index].count++;
    }

    private int GetNextAcquisitionOrder(RunHistoryRecord run)
    {
        int nextOrder = 0;

        foreach (UpgradeHistoryRecord upgrade in run.skillUpgrades)
        {
            if (upgrade != null)
            {
                nextOrder += Mathf.Max(upgrade.count, 1);
            }
        }
        foreach (UpgradeHistoryRecord upgrade in run.attributeUpgrades)
        {
            if (upgrade != null)
            {
                nextOrder += Mathf.Max(upgrade.count, 1);
            }
        }
        return nextOrder;
    }

    public bool RecordPurification(int decayReduction)
    {
        if (!HasActiveRun())
        {
            return false;
        }
        if (historyData.activeRun.rooms == null || historyData.activeRun.rooms.Count == 0)
        {
            Debug.LogWarning("No current room");
            return false;
        }
        RoomHistoryRecord room = historyData.activeRun.rooms[^1];
        if (room == null)
        {
            Debug.LogWarning("Current room data is null");
            return false;
        }
        if (room.rewards == null)
        {
            room.rewards = new List<UpgradeHistoryRecord>();
        }
        UpgradeHistoryRecord purfication = new UpgradeHistoryRecord();
        purfication.upgradeName = "Purification";
        purfication.upgradeType = HistoryUpgradeType.Purification;
        purfication.description = "-" + decayReduction;
        room.rewards.Add(purfication);
        return SaveHistory();
    }

    public bool RecordPlayerDeath(FinalStatsSnapshot statsSnapshot)
    {
        if (!HasActiveRun())
        {
            return false;
        }
        historyData.activeRun.endTimeIso = DateTimeOffset.Now.ToString("O");
        historyData.activeRun.deathCount++;
        ApplyStatsSnapshot(historyData.activeRun, statsSnapshot);
        return SaveHistory();
    }

    private bool FinalizeWin(FinalStatsSnapshot statsSnapshot)
    {
        if (!HasActiveRun())
        {
            return false;
        }
        if (statsSnapshot != null)
        {
            ApplyStatsSnapshot(historyData.activeRun, statsSnapshot);
        }
        if (historyData.activeRun.rooms != null && historyData.activeRun.rooms.Count > 0 && historyData.activeRun.rooms[^1] != null)
        {
            historyData.activeRun.rooms[^1].cleared = true;
            historyData.activeRun.rooms[^1].giveUpHere = false;
        }
        historyData.activeRun.endTimeIso = DateTimeOffset.Now.ToString("O");
        historyData.activeRun.result = RunHistoryResult.Win;
        historyData.completedRuns.Add(historyData.activeRun);
        historyData.activeRun = null;
        return SaveHistory();
    }

    private bool FinalizeWinFromPlayer(PlayerStats stats)
    {
        FinalStatsSnapshot finalStatsSnapshot = CreateStatsSnapshot(stats);
        return FinalizeWin(finalStatsSnapshot);
    }

    public void FinalizeHistoryWin()
    {
        if (!HasActiveRun())
        {
            return;
        }
        Player player = Player.instance;
        PlayerStats stats = null;
        if (player != null)
        {
            stats = player.stats;
        }
        bool isSucceed = FinalizeWinFromPlayer(stats);
        if (!isSucceed)
        {
            Debug.LogWarning("Cannot record win in history");
        }
    }

    public bool FinalizeLose(FinalStatsSnapshot statsSnapshot)
    {
        if (!HasActiveRun())
        {
            return false;
        }
        if (historyData.activeRun.rooms != null && historyData.activeRun.rooms.Count > 0)
        {
            RoomHistoryRecord lastRoom = historyData.activeRun.rooms[^1];
            if (lastRoom != null && !lastRoom.cleared)
            {
                lastRoom.giveUpHere = true;
            }
        }
        if (statsSnapshot != null)
        {
            ApplyStatsSnapshot(historyData.activeRun, statsSnapshot);
        }
        if (string.IsNullOrEmpty(historyData.activeRun.endTimeIso))
        {
            historyData.activeRun.endTimeIso = DateTimeOffset.Now.ToString("O");
        }
        historyData.activeRun.result = RunHistoryResult.Lose;
        historyData.completedRuns.Add(historyData.activeRun);
        historyData.activeRun = null;
        return SaveHistory();
    }

    public bool RecordCurrentStatsBeforeExit()
    {
        if (!HasActiveRun())
        {
            return false;
        }
        historyData.activeRun.endTimeIso = DateTimeOffset.Now.ToString("O");

        // Fix: an exit time still needs to be saved in scenes where no Player
        // object exists, such as ChooseSkill.
        if (Player.instance != null && Player.instance.stats != null)
        {
            PlayerStats stats = Player.instance.stats;
            FinalStatsSnapshot statsSnapshot = CreateStatsSnapshot(stats);
            if (statsSnapshot != null)
            {
                ApplyStatsSnapshot(historyData.activeRun, statsSnapshot);
            }
        }

        return SaveHistory();
    }

    private void ApplyStatsSnapshot(RunHistoryRecord run, FinalStatsSnapshot statsSnapshot)
    {
        if (statsSnapshot == null)
        {
            Debug.LogWarning("Cannot update history stats with a null snapshot");
            return;
        }
        statsSnapshot.hasValue = true;
        run.finalStats = statsSnapshot;
    }

    public List<RunHistoryRecord> getCompletedRuns()
    {
        List<RunHistoryRecord> temp = new List<RunHistoryRecord>();
        foreach (var run in historyData.completedRuns)
        {
            temp.Add(run);
        }
        return temp;
    }

    public bool SaveHistory()
    {
        if (historyDataHandler == null)
        {
            Debug.LogError("HistoryDataHandler is not initialized.");
            return false;
        }

        if (historyData == null)
        {
            historyData = new HistoryData();
        }

        bool saveSucceed = historyDataHandler.SaveData(historyData);
        if (!saveSucceed)
        {
            Debug.LogError("Cannot save th game to history.");
        }

        return saveSucceed;
    }

    public bool HasActiveRun()
    {
        return historyData != null && historyData.activeRun != null;
    }

    private void HandlePlayerDeath()
    {
        if (!HasActiveRun())
        {
            return;
        }
        Player player = Player.instance;
        if (player == null)
        {
            return;
        }
        PlayerStats stats = player.stats;
        FinalStatsSnapshot finalStatsSnapshot = CreateStatsSnapshot(stats);
        bool isSucceed = RecordPlayerDeath(finalStatsSnapshot);
        if (!isSucceed)
        {
            Debug.LogWarning("Cannot record player death");
        }
    }

    private FinalStatsSnapshot CreateStatsSnapshot(PlayerStats stats)
    {
        if (stats == null)
        {
            return null;
        }
        FinalStatsSnapshot snapshot = new FinalStatsSnapshot();
        snapshot.hasValue = true;
        snapshot.maxHP = stats.maxHP.GetValue();
        snapshot.damage = stats.damage.GetValue();
        snapshot.armor = stats.armor.GetValue();
        snapshot.critChance = stats.critChance.GetValue();
        snapshot.critPower = stats.critPower.GetValue();
        snapshot.skillEffect = stats.skillEffect.GetValue();
        snapshot.attackSpeed = stats.attackSpeed.GetValue();
        return snapshot;
    }
    
}
