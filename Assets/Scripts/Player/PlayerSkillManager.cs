using UnityEngine;

public class PlayerSkillManager : MonoBehaviour {
    private SkillBase[] skills;
    [SerializeField] private SkillType currentSkillType;

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
}
