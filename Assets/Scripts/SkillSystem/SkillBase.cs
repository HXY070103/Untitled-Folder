using System.Collections;
using UnityEngine;

public class SkillBase : MonoBehaviour {
    [SerializeField] private float cooldown = 3;
    public float skillDuration = 3;
    
    private float lastTimeUsed;
    private Coroutine handleSkillCo;
    private float remainingDuration;

    private bool isSkillActive;

    public float RemainingDuration => remainingDuration;

    public bool IsSkillActive => isSkillActive;
    public float SkillDuration => skillDuration;

    protected virtual void Awake() {
        lastTimeUsed = float.MinValue;
        remainingDuration = 0;
        isSkillActive = false;
    }

    public void ApplySkill() {
        if (handleSkillCo != null) {
            StopCoroutine(handleSkillCo);
        }

        handleSkillCo = StartCoroutine(HanldeSkill());
    }

    public IEnumerator HanldeSkill() {
        StartSkill();
        while(remainingDuration > 0) {
            remainingDuration -= Time.deltaTime;
            yield return null;
        }
        EndSkill();
        handleSkillCo = null;
    }

    protected virtual void StartSkill() {
        lastTimeUsed = Time.time;
        isSkillActive = true;
        remainingDuration = skillDuration;
    }

    protected virtual void EndSkill()
    {
        remainingDuration = 0;
        isSkillActive = false;
    }

    public bool OnCooldown() {
        return Time.time < lastTimeUsed + cooldown;
    }
}
