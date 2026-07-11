using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class PlayerSkillManager : MonoBehaviour, ISaveable {
    private SkillBase[] skills;
    [SerializeField] private SkillType currentSkillType;
    [SerializeField] private UI_SkillDecay skillDecayUI;
    [SerializeField] private bool hasChosenSkill;
    private int skillDecay;

    public int SkillDecay => skillDecay;

    private void Awake() {
        Initialize();
    }

    public void HandleSkill() {
        if (!hasChosenSkill)
        {
            Debug.Log("No skill chosen yet");
            return;
        }
        if (skills == null)
        {
            Initialize();
        }
        SkillBase currentSkill = skills[(int) currentSkillType];

        if (!currentSkill.IsSkillActive && !currentSkill.OnCooldown()) {
            currentSkill.ApplySkill();
        }
    }

    public SkillBase GetCurrentSkill() {
        if (!hasChosenSkill)
        {
            return null;
        }
        if (skills == null)
        {
            Initialize();
        }
        return skills[(int) currentSkillType];
    }

    private void Initialize()
    {
        skills = new SkillBase[3];

        skills[(int) SkillType.TimeControl] = GetComponentInChildren<SkillTimeControl>();
        skills[(int) SkillType.LifeSteal] = GetComponentInChildren<SkillLifeSteal>();
        skills[(int) SkillType.Phase] = GetComponentInChildren<SkillPhase>();
    }

    public void UpdateSkillDecay(int skillDecay)
    {
        this.skillDecay = skillDecay;
        if (skillDecayUI != null)
        {
            skillDecayUI.UpdateDecayUI(this.skillDecay);
        }
    }

    public void ChooseSkill(SkillType type)
    {
        if (skills == null)
        {
            Initialize();
        }
        hasChosenSkill = true;
        currentSkillType = type;
        SkillBase currentSkill = GetCurrentSkill();
        currentSkill.SetNumOfUse(0);
        currentSkill.ClearRuntimeSkillState();
        currentSkill.SetDecayLevel(0);
        currentSkill.SetRemainCooldown(0);
        currentSkill.LoadUnlockedUpgrades(new List<string>());
        currentSkill.LoadUnlockedBetterUpgrades(new List<string>());
        currentSkill.LoadUnlockedSkillPhaseUpgrades(new List<string>());

        UpdateSkillDecay(0);
    }

    public void LoadData(GameData data)
    {
        if (skills == null)
        {
            Initialize();
        }
        hasChosenSkill = data.hasChosenSkill;
        if (!hasChosenSkill)
        {
            UpdateSkillDecay(0);
            return;
        }
        currentSkillType = (SkillType) data.chosenSkillType;
        SkillBase currentSKill = GetCurrentSkill();
        currentSKill.SetNumOfUse(data.numOfUse);
        currentSKill.ClearRuntimeSkillState();
        currentSKill.SetDecayLevel(data.skillDecay);
        currentSKill.SetRemainCooldown(data.remainSkillColldown);
        currentSKill.LoadUnlockedUpgrades(data.unlockedSkillUpgrades);
        currentSKill.LoadUnlockedBetterUpgrades(data.unlockedBetterUpgrades);
        currentSKill.LoadUnlockedSkillPhaseUpgrades(data.unlockedSkillPhaseUpgrades);
        UpdateSkillDecay(data.skillDecay);
    }

    public void SaveData(ref GameData data)
    {
        if (skills == null)
        {
            Initialize();
        }
        if (!hasChosenSkill)
        {
            return;
        }
        data.hasChosenSkill = true;
        SkillBase currentSkill = GetCurrentSkill();

        data.chosenSkillType = (int) currentSkillType;
        data.skillDecay = currentSkill.GetDecayLevel();
        data.numOfUse = currentSkill.GetNumOfUse();
        data.remainSkillColldown = currentSkill.GetReaminCooldown();
        data.unlockedSkillUpgrades = currentSkill.GetUnlockedUpgrades();
        data.unlockedBetterUpgrades = currentSkill.GetUnlockedBetterUpgrades();
        data.unlockedSkillPhaseUpgrades = currentSkill.GetUnlockedSkillPhaseUpgrades();
    }
}
