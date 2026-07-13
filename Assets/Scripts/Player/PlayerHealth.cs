using UnityEngine;
using System.Collections;

public class PlayerHealth : EntityHealth, ISaveable {
    [SerializeField] private ShieldManager shieldManager = new ShieldManager();
    public ShieldManager ShieldManager => shieldManager;

    protected override void Awake() {
        base.Awake();

        PlayerStats stats = GetComponent<PlayerStats>();
        SkillLifeSteal lifeSteal = GetComponentInChildren<SkillLifeSteal>();
        shieldManager.Initialize(stats, lifeSteal);
    }

    protected override float CalculateReducedHP(Damage damage, Transform damageDealer) {
        float remain = base.CalculateReducedHP(damage, damageDealer);
        remain = shieldManager.ConsumeShield(remain);
        return remain;
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
