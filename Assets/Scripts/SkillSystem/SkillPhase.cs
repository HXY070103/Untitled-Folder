using UnityEngine;

public class SkillPhase : SkillBase {
    protected override void StartSkill() {
        base.StartSkill();

        Debug.Log("Start Phase!");
    }

    protected override void EndSkill() {
        base.EndSkill();

        Debug.Log("End Phase!");
    }
}
