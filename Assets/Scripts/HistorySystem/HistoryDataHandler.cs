using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;

public class HistoryDataHandler
{
    private string fullPath;
    private const int MaxCompletedRuns = 50;

    // dataDirPath must = Application.persistentDataPath   dataFileName must = historyData.json
    public HistoryDataHandler(string dataDirPath, string dataFileName)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
    }

    public bool SaveData(HistoryData data)
    {
        try
        {
            if (data == null)
            {
                data = new HistoryData();
            }
            EnsureAllCollections(data);
            TrimCompletedRuns(data);

            string directoryPath = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string dataToSave = JsonUtility.ToJson(data, true);
            File.WriteAllText(fullPath, dataToSave);
            return true;
        }
        catch(Exception e) {
            Debug.LogError("Error saving history data: " + e.Message);
            return false;
        }
    }

    public HistoryData LoadData()
    {
        if (!File.Exists(fullPath))
        {
            return CreateEmptyHistoryData();
        }
        try
        {
            string dataToLoad = File.ReadAllText(fullPath);
            if (string.IsNullOrWhiteSpace(dataToLoad))
            {
                Debug.LogWarning("Empty history data");
                return CreateEmptyHistoryData();
            }
            HistoryData historyData = JsonUtility.FromJson<HistoryData>(dataToLoad);
            if (historyData == null)
            {
                Debug.LogWarning("Cannot load histroy data");
                return CreateEmptyHistoryData();
            }
            EnsureAllCollections(historyData);
            TrimCompletedRuns(historyData);
            return historyData;
            
        } catch(Exception e)
        {
            Debug.LogError("Error loading history data" + e.Message);
            return CreateEmptyHistoryData();
        }
    }

    private HistoryData CreateEmptyHistoryData()
    {
        HistoryData historyData = new HistoryData();
        EnsureAllCollections(historyData);
        return historyData;
    }

    // must write run.startTimeIso = DateTimeOffset.Now.ToString("O"); when create new game; 用于记录当前时间+时区
    private void TrimCompletedRuns(HistoryData data)
    {
        data.completedRuns.RemoveAll(run => run == null);
        data.completedRuns.Sort(CompareRunsNewFirst);
        if (data.completedRuns.Count <= MaxCompletedRuns)
        {
            return;
        }
        data.completedRuns.RemoveRange(MaxCompletedRuns, data.completedRuns.Count - MaxCompletedRuns);
    }

    private int CompareRunsNewFirst(RunHistoryRecord first, RunHistoryRecord second)
    {
        bool firstValid = TryParseStartTime(first, out DateTimeOffset firstTime);
        bool secondValid = TryParseStartTime(second, out DateTimeOffset secondTime);
        if (!firstValid && !secondValid)
        {
            return 0;
        }
        if (!firstValid)
        {
            return 1;
        }
        if (!secondValid)
        {
            return -1;
        }
        return secondTime.CompareTo(firstTime);
    }

    private bool TryParseStartTime(RunHistoryRecord run, out DateTimeOffset startTime)
    {
        startTime = default;
        if (run == null || string.IsNullOrWhiteSpace(run.startTimeIso))
        {
            return false;
        }
        return DateTimeOffset.TryParseExact(run.startTimeIso, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime);
    }

    private void EnsureAllCollections(HistoryData historyData)
    {
        if (historyData.completedRuns == null)
        {
            historyData.completedRuns = new List<RunHistoryRecord>();
        }

        EnsureRunCollections(historyData.activeRun);

        foreach (RunHistoryRecord run in historyData.completedRuns)
        {
            EnsureRunCollections(run);
        }
    }

    private void EnsureRunCollections(RunHistoryRecord run)
    {
        if (run == null)
        {
            return;
        }

        if (run.rooms == null)
        {
            run.rooms = new List<RoomHistoryRecord>();
        }

        if (run.skillUpgrades == null)
        {
            run.skillUpgrades =
                new List<UpgradeHistoryRecord>();
        }

        if (run.attributeUpgrades == null)
        {
            run.attributeUpgrades =
                new List<UpgradeHistoryRecord>();
        }

        if (run.finalStats == null)
        {
            run.finalStats = new FinalStatsSnapshot();
        }

        foreach (RoomHistoryRecord room in run.rooms)
        {
            if (room != null && room.rewards == null)
            {
                room.rewards = new List<UpgradeHistoryRecord>();
            }
        }
    }
}
