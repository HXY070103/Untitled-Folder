using System;
using UnityEngine;

[Serializable]
public class ShieldManager {
    private PlayerStats stats;
    private SkillLifeSteal skill;

    [Header("Temporary shield info")]
    [SerializeField] private float temporaryShield = 0;
    [SerializeField] private bool hasTemporaryShield = false;

    [Header("Permanent shield info")]
    [SerializeField] private float permanentShield = 0;
    [SerializeField] private bool hasPermanentShield = false;

    public float Shield => temporaryShield + permanentShield;
    public bool HasShield => hasTemporaryShield || hasPermanentShield;

    public void Initialize(PlayerStats stats, SkillLifeSteal skill) {
        this.stats = stats;
        this.skill = skill;
    }

    public float ConsumeShield(float damage) {
        float remain = damage;
        remain = ConsumeTemporaryShield(remain);
        remain = ConsumePermanentShield(remain);
        return remain;
    }

    private float ConsumeTemporaryShield(float damage) {
        if (!hasTemporaryShield) {
            return damage;
        }

        temporaryShield -= damage;

        if (temporaryShield > 0.5f) {
            return 0;
        }

        float remain = Math.Abs(temporaryShield);
        RemoveTemporaryShield();

        return hasPermanentShield ? remain : 0;
    }

    private float ConsumePermanentShield(float damage) {
        if (!hasPermanentShield) {
            return damage;
        }

        permanentShield -= damage;

        if (permanentShield > 0.5f) {
            return 0;
        }

        RemovePermanentShield();

        return 0;
    }

    public void AddTemporaryShield(float shield) {
        if (!HasShield) {
            AddShieldBuff();
        }

        temporaryShield += shield;
        hasTemporaryShield = true;
    }

    public void RemoveTemporaryShield() {
        temporaryShield = 0;
        hasTemporaryShield = false;

        if (!HasShield) {
            RemoveShieldBuff();
        }
    }

    public void AddPermanentShield(float shield) {
        if (!HasShield) {
            AddShieldBuff();
        }

        permanentShield += shield;
        hasPermanentShield = true;
    }

    public void RemovePermanentShield() {
        permanentShield = 0;
        hasPermanentShield = false;

        if (!HasShield) {
            RemoveShieldBuff();
        }
    }

    private void AddShieldBuff() {
        if (!skill.Unlocked("Overflowing Aegis+")) {
            return;
        }

        stats.damage.AddMultiplier(skill.damageMultiplier, "Shield");
        stats.attackSpeed.AddMultiplier(skill.attackSpeedMultiplier, "Shield");
    }

    private void RemoveShieldBuff() {
        if (!skill.Unlocked("Overflowing Aegis+")) {
            return;
        }

        stats.damage.RemoveMultiplier("Shield");
        stats.attackSpeed.RemoveMultiplier("Shield");
    }
}
