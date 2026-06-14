using UnityEngine;
using System.Collections.Generic;

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

        if (!currentSkill.OnCooldown()) {
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

        UpdateSkillDecay(0);
    }

    public void LoadData(GameData data)
    {
        Debug.Log("Load skill. hasChosenInitialSkill = " + data.hasChosenSkill + ", chosenSkillType = " + data.chosenSkillType);
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
        UpdateSkillDecay(data.skillDecay);
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log("Save skill. hasChosenSkill = " + hasChosenSkill + ", currentSkillType = " + currentSkillType);
        if (skills == null)
        {
            Initialize();
        }
        if (!hasChosenSkill)
        {
            return;
        }
        SkillBase currentSKill = GetCurrentSkill();

        data.chosenSkillType = (int) currentSkillType;
        data.skillDecay = currentSKill.GetDecayLevel();
        data.numOfUse = currentSKill.GetNumOfUse();
        data.remainSkillColldown = currentSKill.GetReaminCooldown();
        data.unlockedSkillUpgrades = currentSKill.GetUnlockedUpgrades();
        data.unlockedBetterUpgrades = currentSKill.GetUnLockedBetterUpgrades();
    }
}
