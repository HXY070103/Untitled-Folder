using System.Collections;
using UnityEngine;

public class SkillBase : MonoBehaviour {
    [SerializeField] private float cooldown = 3;
    public float skillDuration = 3;
    
    private float lastTimeUsed;
    private Coroutine handleSkillCo;

    protected virtual void Awake() {
        lastTimeUsed = float.MinValue;
    }

    public void ApplySkill() {
        if (handleSkillCo != null) {
            StopCoroutine(handleSkillCo);
        }

        handleSkillCo = StartCoroutine(HanldeSkill());
    }

    public IEnumerator HanldeSkill() {
        StartSkill();
        yield return new WaitForSeconds(skillDuration);
        EndSkill();
    }

    protected virtual void StartSkill() {
        lastTimeUsed = Time.time;
    }

    protected virtual void EndSkill() {}

    public bool OnCooldown() {
        return Time.time < lastTimeUsed + cooldown;
    }
}
