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
    public float TemporaryShield => temporaryShield;
    public float PermanentShield => permanentShield;
    public bool HasTemporaryShield => hasTemporaryShield;
    public bool HasPermanentShield => hasPermanentShield;
    public event Action OnShieldChanged;

    public void Initialize(PlayerStats stats, SkillLifeSteal skill) {
        this.stats = stats;
        this.skill = skill;
    }

    public float ConsumeShield(float damage) {
        if (damage <= 0 || !HasShield) {
            return damage;
        }

        float previousTemporaryShield = temporaryShield;
        float previousPermanentShield = permanentShield;
        bool previouslyHadTemporaryShield = hasTemporaryShield;
        bool previouslyHadPermanentShield = hasPermanentShield;

        float remain = damage;
        remain = ConsumeTemporaryShield(remain);

        if (remain > 0) {
            remain = ConsumePermanentShield(remain);
        }

        bool shieldChanged =
            !Mathf.Approximately(previousTemporaryShield, temporaryShield) ||
            !Mathf.Approximately(previousPermanentShield, permanentShield) ||
            previouslyHadTemporaryShield != hasTemporaryShield ||
            previouslyHadPermanentShield != hasPermanentShield;

        if (shieldChanged) {
            NotifyShieldChanged();
        }

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

        float remain = Mathf.Max(-temporaryShield, 0);
        ClearTemporaryShield();

        return remain;
    }

    private float ConsumePermanentShield(float damage) {
        if (!hasPermanentShield) {
            return damage;
        }

        permanentShield -= damage;

        if (permanentShield > 0.5f) {
            return 0;
        }

        float remain = Mathf.Max(-permanentShield, 0);
        ClearPermanentShield();

        return remain;
    }

    public void AddTemporaryShield(float shield) {
        if (shield <= 0) {
            return;
        }

        if (!HasShield) {
            AddShieldBuff();
        }

        temporaryShield += shield;
        hasTemporaryShield = true;
        NotifyShieldChanged();
    }

    public void RemoveTemporaryShield() {
        if (!hasTemporaryShield && Mathf.Approximately(temporaryShield, 0)) {
            return;
        }

        ClearTemporaryShield();
        NotifyShieldChanged();
    }

    public void AddPermanentShield(float shield) {
        if (shield <= 0) {
            return;
        }

        if (!HasShield) {
            AddShieldBuff();
        }

        permanentShield += shield;
        hasPermanentShield = true;
        NotifyShieldChanged();
    }

    public void RemovePermanentShield() {
        if (!hasPermanentShield && Mathf.Approximately(permanentShield, 0)) {
            return;
        }

        ClearPermanentShield();
        NotifyShieldChanged();
    }

    private void ClearTemporaryShield() {
        temporaryShield = 0;
        hasTemporaryShield = false;

        if (!HasShield) {
            RemoveShieldBuff();
        }
    }

    private void ClearPermanentShield() {
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

    public void RefreshBuff() {
        stats.damage.RemoveMultiplier("Shield");
        stats.attackSpeed.RemoveMultiplier("Shield");

        if (HasShield) {
            AddShieldBuff();
        }
    }

    public void RestoreShields(float temporary, float permanent)
    {
        temporaryShield = Mathf.Max(temporary, 0);
        permanentShield = Mathf.Max(permanent, 0);
        hasTemporaryShield = temporaryShield > 0;
        hasPermanentShield = permanentShield > 0;
        RefreshBuff();
        NotifyShieldChanged();
    }

    private void NotifyShieldChanged() {
        OnShieldChanged?.Invoke();
    }
}
