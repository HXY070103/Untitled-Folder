using UnityEngine;

public class PlayerSkillManager : MonoBehaviour {
    private SkillBase[] skills;
    [SerializeField] private SkillType currentSkillType;

    private void Awake() {
        skills = new SkillBase[3];

        skills[(int) SkillType.TimeControl] = GetComponentInChildren<SkillTimeControl>();
        skills[(int) SkillType.LifeSteal] = GetComponentInChildren<SkillLifeSteal>();
        skills[(int) SkillType.Phase] = GetComponentInChildren<SkillPhase>();
    }

    public void HandleSkill() {
        SkillBase currentSkill = skills[(int) currentSkillType];

        if (!currentSkill.OnCooldown()) {
            currentSkill.ApplySkill();
        }
    }
}
