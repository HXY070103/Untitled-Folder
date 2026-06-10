using UnityEngine;
using System.Collections;
public class PlayerHealth : EntityHealth, ISaveable {
    private SkillPhase phase;

    public bool isAttacked;

    private bool isVulnerable = false;
    private float damageMultiplier;

    protected override void Awake() {
        base.Awake();

        phase = GetComponentInChildren<SkillPhase>();
    }

    public override void TakeDamage(Damage damage, Transform damageDealer) {
        if (isVulnerable) {
            damage = new Damage(damage.value * damageMultiplier, damage.isCrit);
        }
        isAttacked = true;

        base.TakeDamage(damage, damageDealer);

        if (phase.InPerfectPhaseWindow) {
            phase.PerfectPhaseTriggered();
        }
    }

    public void StartVulnerability(float damageMultiplier) {
        isVulnerable = true;
        this.damageMultiplier = damageMultiplier;
    }

    public void EndVulnerability() {
        isVulnerable = false;
    }

    public void LoadData(GameData data)
    {
        if (data.playerCurrentHP >= 0)
        {
            StartCoroutine(LoadHealthNextFrame(data.playerCurrentHP));
        }
    }

    private IEnumerator LoadHealthNextFrame(float HP)
    {
        yield return null;
        SetCurrentHP(HP);
    }

    public void SaveData(ref GameData data)
    {
        data.playerCurrentHP = GetCurrentHP();
    }
}
