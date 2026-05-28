using System;
using UnityEngine;

public class SkillTimeControl : SkillBase {
    public static event Action<float> OnSetFirstAttack;
    public static event Action OnResetFirstAttack;

    [SerializeField] private GameObject domainPrefab;
    [SerializeField] private float maxSize = 20;
    [SerializeField] private float slowDownPercentage = 0.8f;
    [SerializeField] private float bonusDuration = 1;
    [SerializeField] private float damageMultiplier = 2;

    private GameObject domain;

    public float GetMaxSize() {
        return maxSize * stats.GetSkillEffect();
    }

    public float GetSlowDownPercentage() {
        if (UnLocked("TemporalFreeze")) {
            return 1;
        }

        return slowDownPercentage * stats.GetSkillEffect();
    }


    protected override void StartSkill() {
        base.StartSkill();

        CreateDomain();

        if (UnLocked("HuntingTime")) {
            EnemyHealth.OnEnemyDeath += ExtendSkillDuration;
        }

        if (UnLocked("PrecisionTargeting")) {
            OnSetFirstAttack?.Invoke(damageMultiplier);
        }
    }

    protected override void EndSkill() {
        base.EndSkill();

        DestroyDomain();

        if (UnLocked("HuntingTime")) {
            EnemyHealth.OnEnemyDeath -= ExtendSkillDuration;
        }

        if (UnLocked("PrecisionTargeting")) {
            OnResetFirstAttack?.Invoke();
        }
    }

    private void CreateDomain() {
        domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<Domain>().SetUpDomain(this);
    }

    private void DestroyDomain() {
        domain.GetComponent<Domain>().ShrinkDomain();
    }

    private void ExtendSkillDuration() {
        remainingDuration += bonusDuration;
    }
}
