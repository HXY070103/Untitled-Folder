using UnityEngine;
using System.Collections;
public class PlayerHealth : EntityHealth, ISaveable {
    public bool isAttacked;

    protected override void Awake() {
        base.Awake();
    }

    public override void TakeDamage(Damage damage, Transform damageDealer) {
        isAttacked = true;

        base.TakeDamage(damage, damageDealer);
    }

    public void LoadData(GameData data)
    {
        if (data.playerCurrentHP > 0) 
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
