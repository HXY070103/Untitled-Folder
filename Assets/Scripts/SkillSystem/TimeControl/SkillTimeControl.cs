using System;
using UnityEngine;

public class SkillTimeControl : SkillBase {
    public static event Action<float> OnSetFirstAttack;
    public static event Action OnResetFirstAttack;

    [Header("Skill info")]
    [SerializeField] private GameObject domainPrefab;
    [SerializeField] private float maxSize = 20;
    [SerializeField] private float slowDownPercentage = 0.8f;
    
    [Header("Hunting time info")]
    [SerializeField] private float bonusDuration = 1;

    [Header("Precision targeting info")]
    [SerializeField] private float damageMultiplier = 2;

    private GameObject domain;

    public float GetMaxSize() {
        return maxSize * stats.GetSkillEffect();
    }

    public float GetSlowDownPercentage() {
        if (UnLocked("Temporal Freeze")) {
            return 1;
        }

        return slowDownPercentage * stats.GetSkillEffect();
    }


    protected override void StartSkill() {
        base.StartSkill();

        CreateDomain();

        if (UnLocked("Hunting Time")) {
            EnemyHealth.OnEnemyDeath += ExtendSkillDuration;
        }

        if (UnLocked("Precision Targeting")) {
            OnSetFirstAttack?.Invoke(damageMultiplier);
        }
    }

    protected override void EndSkill() {
        base.EndSkill();

        DestroyDomain();

        if (UnLocked("Hunting Time")) {
            EnemyHealth.OnEnemyDeath -= ExtendSkillDuration;
        }

        if (UnLocked("Precision Targeting")) {
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
