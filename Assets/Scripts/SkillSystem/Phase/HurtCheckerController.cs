using UnityEngine;

public class HurtCheckerController : MonoBehaviour {
    private SkillPhase skill;

    public void SetUpHurtChecker(SkillPhase skill) {
        this.skill = skill;
    }

    public void GetHurt() {
        skill.PerfectPhaseTriggered();
    }
}
