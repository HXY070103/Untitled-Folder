using UnityEngine;
using System.Collections;

public class PlayerHealth : EntityHealth, ISaveable {
    private PlayerStats playerStats;
    private SkillLifeSteal lifeSteal;

    [SerializeField] private ShieldManager shieldManager = new ShieldManager();
    public ShieldManager ShieldManager => shieldManager;

    private bool canApplyBuff = false;

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
        if (data.playerCurrentHP > 0) {
            StartCoroutine(LoadHealthNextFrame(data.playerCurrentHP));
        }
    }

    private IEnumerator LoadHealthNextFrame(float HP) {
        yield return null;
        SetCurrentHP(HP);
    }

    public void SaveData(ref GameData data) {
        data.playerCurrentHP = GetCurrentHP();
    }
}
