using UnityEngine;

public class HurtCheckerController : MonoBehaviour {
    private SkillPhase hurtCheckerManager;

    public void SetUpHurtChecker(SkillPhase hurtCheckerManager) {
        this.hurtCheckerManager = hurtCheckerManager;
    }

    public void GetHurt() {
        hurtCheckerManager.PerfectPhaseTriggered();
    }
}
