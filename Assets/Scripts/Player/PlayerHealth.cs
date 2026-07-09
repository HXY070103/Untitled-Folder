using UnityEngine;
using System.Collections;
using System;
public class PlayerHealth : EntityHealth, ISaveable {
    [SerializeField] private float temporaryShield = 0;
    [SerializeField] private float permanentShield = 0;
    [SerializeField] private bool hasTemporaryShield = false;
    [SerializeField] private bool hasPermanentShield = false;

    public float Shield => temporaryShield + permanentShield;
    public bool HasShield => hasTemporaryShield || hasPermanentShield;

    protected override float CalculateReducedHP(Damage damage) {
        float reducedHP = base.CalculateReducedHP(damage);
        reducedHP = ConsumeTemporaryShield(reducedHP);
        reducedHP = ConsumePermanentShield(reducedHP);
        return reducedHP;
    }

    private float ConsumeTemporaryShield(float reducedHP) {
        if (!hasTemporaryShield) {
            return reducedHP;
        }

        temporaryShield -= reducedHP;

        if (temporaryShield > 0.5f) {
            return 0;
        }

        float remainReducedHP = Math.Abs(temporaryShield);
        CancelTemporaryShield();

        return hasPermanentShield ? remainReducedHP : 0;
    }

    private float ConsumePermanentShield(float reducedHP) {
        if (!hasPermanentShield) {
            return reducedHP;
        }

        permanentShield -= reducedHP;

        if (permanentShield > 0.5f) {
            return 0;
        }

        CancelPermanentShield();

        return 0;
    }

    public void GenerateTemporaryShield(float shield) {
        temporaryShield += shield;
        hasTemporaryShield = true;
    }

    public void CancelTemporaryShield() {
        temporaryShield = 0;
        hasTemporaryShield = false;
    }

    public void GeneratePermanentShield(float shield) {
        permanentShield += shield;
        hasPermanentShield = true;
    }

    public void CancelPermanentShield() {
        permanentShield = 0;
        hasPermanentShield = false;
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
