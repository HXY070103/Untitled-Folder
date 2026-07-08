using System;
using Unity.VisualScripting;
using UnityEngine;

public class SkillTimeControl : SkillBase {
    [Header("Skill info")]
    [SerializeField] private GameObject domainPrefab;
    private GameObject domain;

    [Header("Domain info")]
    [SerializeField] private float maxSize = 20;
    [SerializeField] private float slowDownPercentage = 0.6f;
    public float expandSpeed = 2;

    [Header("Temporal corrosion info")]
    public float damage = 5;
    public float damageInterval = 0.5f;

    [Header("Vulnerable field info")]
    public float damageMultiplier = 1.5f;

    [Header("Synchronized field info")]
    [SerializeField] private float reducedMaxSize = 10;
    [SerializeField] private float increasedSkillDuration = 4;
    public float recoverSpeedDelay = 1;
    
    [Header("Hunting time info")]
    [SerializeField] private float bonusDuration = 1.5f;
    [SerializeField] private GameObject shockwavePrefab;
    public float shockwaveDamage = 20;
    public float shockwaveDamageRange = 3;
    public float shockwaveDestoryDelay = 1;


    public float GetMaxSize() {
        if (Unlocked("Synchronized Field")) {
            return reducedMaxSize * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
        }

        return maxSize * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    public float GetSlowDownPercentage() {
        if (Unlocked("Temporal Freeze")) {
            return 1;
        }

        return slowDownPercentage * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    public override float GetSkillDuration() {
        if (Unlocked("Synchronized Field")) {
            return increasedSkillDuration * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
        }

        return base.GetSkillDuration();
    }

    protected override void StartSkill() {
        base.StartSkill();

        CreateDomain();

        EnemyHealth.OnEnemyKilled += ApplyHuntingTimeEffect;
    }

    protected override void EndSkill() {
        base.EndSkill();

        DestroyDomain();

        EnemyHealth.OnEnemyKilled -= ApplyHuntingTimeEffect;
    }

    private void CreateDomain() {
        domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<Domain>().SetUpDomain(this);

        if (Unlocked("Synchronized Field")) {
            domain.transform.SetParent(transform);
        }
    }

    private void DestroyDomain() {
        domain.GetComponent<Domain>().ShrinkDomain();
    }

    private void ApplyHuntingTimeEffect(Enemy enemy) {
        if (Unlocked("Hunting Time")) {
            remainingDuration += bonusDuration;
        }

        if (Unlocked("Hunting Time+")) {
            GameObject shockwave = 
                Instantiate(shockwavePrefab, enemy.transform.position, Quaternion.identity);
            shockwave.GetComponent<Shockwave>().SetUpShockwave(this);
        }
    }
}
