using System.Collections.Generic;
using UnityEngine;

public class EnemySkillEffectReceiver : MonoBehaviour, ISlowable {
    private Enemy enemy;
    private List<SkillEffectBase> effects = new List<SkillEffectBase>();
    private bool isActive = true;
    
    private void Awake() {
        enemy = GetComponentInParent<Enemy>();
    }

    private void Update() {
        if (!isActive) {
            return;
        }

        for (int i = effects.Count - 1; i >= 0; i -= 1) {
            effects[i].Update();
        }
    }

    public void ApplySkillEffect(SkillEffectBase effect) {
        if (!isActive) {
            return;
        }

        effect.Apply(enemy);
        effects.Add(effect);
    }

    public void RemoveSkillEffect(string effectName) {
        if (!isActive) {
            return;
        }

        for (int i = effects.Count - 1; i >= 0; i -= 1) {
            if (effects[i].name.Equals(effectName)) {
                effects[i].Remove();
                effects.RemoveAt(i);
            }
        }
    }

    public void SlowDown(float slowDownPercentage) {
        CancelInvoke(nameof(RecoverSpeed));
        enemy.tc.SetLocalTimeScale(1 - slowDownPercentage);
        enemy.anim.speed = 1 - slowDownPercentage;
    }

    public void RecoverSpeed() {
        enemy.tc.ResetLocalTimeScale();
        enemy.anim.speed = 1;
    }

    public void RecoverSpeedWithDelay(float delay) {
        Invoke(nameof(RecoverSpeed), delay);
    }

    public void SwitchOn() {
        isActive = true;
    }

    public void SwitchOff() {
        isActive = false;
    }
}
