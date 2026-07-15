using System;
using System.Collections.Generic;

[Serializable]
public class HistoryData
{
    public RunHistoryRecord activeRun;
    public List<RunHistoryRecord> completedRuns = new List<RunHistoryRecord>();
}

[Serializable]
public class RunHistoryRecord
{
    public string runId = "";
    public string startTimeIso = "";
    public string endTimeIso = "";
    public double accumulatedPlaySeconds;
    public RunHistoryResult result = RunHistoryResult.InProgress;
    public int deathCount;

    public string initialSkillName = "";
    public string initialSkillDescription = "";

    public List<RoomHistoryRecord> rooms = new List<RoomHistoryRecord>();
    public List<UpgradeHistoryRecord> skillUpgrades = new List<UpgradeHistoryRecord>();
    public List<UpgradeHistoryRecord> attributeUpgrades = new List<UpgradeHistoryRecord>();
    public FinalStatsSnapshot finalStats = new FinalStatsSnapshot();
}

[Serializable]
public class RoomHistoryRecord
{
    public string sceneName = "";
    public string displayName = "";
    public HistoryRoomType roomType = HistoryRoomType.Normal;
    public string encounterName = "";
    public bool cleared;
    public bool giveUpHere;
    public List<UpgradeHistoryRecord> rewards = new List<UpgradeHistoryRecord>();
}

[Serializable]
public class UpgradeHistoryRecord
{
    public string upgradeName = "";
    public string description = "";
    public HistoryUpgradeType upgradeType = HistoryUpgradeType.Skill;
    public int acquisitionOrder;
    public int count = 1;
}

[Serializable]
public class FinalStatsSnapshot
{
    public bool hasValue;
    public float maxHP;
    public float damage;
    public float critChance;
    public float critPower;
    public float armor;
    public float skillEffect;
    public float attackSpeed;
}
