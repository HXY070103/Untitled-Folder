using UnityEngine;
using System.Collections;

public class PlayerHealth : EntityHealth, ISaveable {
    private PlayerStats playerStats;
    private SkillLifeSteal lifeSteal;

    [SerializeField] private ShieldManager shieldManager = new ShieldManager();
    public ShieldManager ShieldManager => shieldManager;

    private bool canApplyBuff = false;
    private Coroutine loadPlayerStateCoroutine;
    private bool hasPendingLoad;
    private float pendingCurrentHP;
    private float pendingTemporaryShield;
    private float pendingPermanentShield;
    private float pendingTemporaryShieldRemainingDuration;

    protected override void Awake() {
        base.Awake();

        playerStats = stats as PlayerStats;
        lifeSteal = GetComponentInChildren<SkillLifeSteal>();
        shieldManager.Initialize(playerStats, lifeSteal);
    }

    protected override float CalculateReducedHP(Damage damage, Transform damageDealer) {
        float remain = base.CalculateReducedHP(damage, damageDealer);
        remain = shieldManager.ConsumeShield(remain);
        return remain;
    }

    public override void ReduceHP(float reducedHP) {
        base.ReduceHP(reducedHP);

        ApplyBuff();
    }

    public void SetCanApplyBuff(bool canApplyBuff) {
        this.canApplyBuff = canApplyBuff;
    }

    private void ApplyBuff() {
        if (!canApplyBuff) {
            return;
        }
        
        playerStats.damage.RemoveMultiplier("DesperateGambit");
        playerStats.attackSpeed.RemoveMultiplier("DesperateGambit");

        playerStats.damage.AddMultiplier(lifeSteal.GetDamageMultiplier(), "DesperateGambit");
        playerStats.attackSpeed.AddMultiplier(lifeSteal.GetAttackSpeedMultiplier(), "DesperateGambit");
    }

    public void LoadData(GameData data) {
        if (loadPlayerStateCoroutine != null) {
            StopCoroutine(loadPlayerStateCoroutine);
        }

        pendingCurrentHP = data.playerCurrentHP;
        pendingTemporaryShield = Mathf.Max(data.playerTemporaryShield, 0);
        pendingPermanentShield = Mathf.Max(data.playerPermanentShield, 0);
        pendingTemporaryShieldRemainingDuration = Mathf.Max(data.playerTemporaryShieldRemainingDuration,0);

        if ( pendingTemporaryShield <= 0 || pendingTemporaryShieldRemainingDuration <= 0) {
            pendingTemporaryShield = 0;
            pendingTemporaryShieldRemainingDuration = 0;
        }

        hasPendingLoad = true;
        loadPlayerStateCoroutine = StartCoroutine(LoadPlayerStateNextFrame());
    }

    private IEnumerator LoadPlayerStateNextFrame() {
        yield return null;

        if (pendingCurrentHP > 0) {
            SetCurrentHP(pendingCurrentHP);
        }

        float maxTemporaryShieldDuration = Mathf.Max(lifeSteal.GetSkillDuration(),0);
        float remainingDuration = Mathf.Clamp(
            pendingTemporaryShieldRemainingDuration,
            0,
            maxTemporaryShieldDuration
        );
        float temporaryShieldToRestore = remainingDuration > 0 ? pendingTemporaryShield : 0;

        shieldManager.RestoreShields(temporaryShieldToRestore, pendingPermanentShield);
        lifeSteal.RestoreTemporaryShieldDuration(remainingDuration);

        hasPendingLoad = false;
        loadPlayerStateCoroutine = null;
    }

    public void SaveData(ref GameData data) {
        if (hasPendingLoad) {
            data.playerCurrentHP = pendingCurrentHP;
            data.playerTemporaryShield = pendingTemporaryShield;
            data.playerPermanentShield = pendingPermanentShield;
            data.playerTemporaryShieldRemainingDuration =
                pendingTemporaryShieldRemainingDuration;
            return;
        }

        data.playerCurrentHP = GetCurrentHP();
        data.playerTemporaryShield = shieldManager.TemporaryShield;
        data.playerPermanentShield = shieldManager.PermanentShield;
        data.playerTemporaryShieldRemainingDuration =
            shieldManager.HasTemporaryShield
                ? lifeSteal.GetTemporaryShieldRemainingDuration()
                : 0;
    }
}
