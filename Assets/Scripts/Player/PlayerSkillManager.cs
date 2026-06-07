using UnityEngine;

public class PlayerSkillManager : MonoBehaviour, ISaveable {
    private SkillBase[] skills;
    [SerializeField] private SkillType currentSkillType;
    [SerializeField] private UI_SkillDecay skillDecayUI;
    private int skillDecay;

    public int SkillDecay => skillDecay;

    private void Awake() {
        Initialize();
    }

    public void HandleSkill() {
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
        skillDecayUI.UpdateDecayUI(this.skillDecay);
    }

    public void LoadData(GameData data)
    {
        this.skillDecay = data.skillDecay;
        skillDecayUI.UpdateDecayUI(this.skillDecay);
        this.GetCurrentSkill().UpdateSkillDecay(this.skillDecay);
    }

    public void SaveData(ref GameData data)
    {
        data.skillDecay = this.skillDecay;
    }
}
