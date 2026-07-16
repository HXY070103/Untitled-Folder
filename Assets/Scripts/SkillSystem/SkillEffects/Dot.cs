using UnityEngine;

public class Dot : SkillEffectBase {
    private float damage;
    private float damageInterval;
    private float timer;

    public Dot(string name, float damage, float damageInterval) : base(name, false) {
        this.damage = damage;
        this.damageInterval = damageInterval;
    }

    public override void Apply(Enemy enemy) {
        base.Apply(enemy);

        timer = 0;
    }

    public override void Update() {
        base.Update();

        timer -= Time.deltaTime;

        if (timer < 0) {
            enemy.health.ReduceHP(damage);
            enemy.vfx.PlayOnDamageVFX();
            timer = damageInterval;
        }
    }

    public override void Remove() {
        base.Remove();
    }
}
