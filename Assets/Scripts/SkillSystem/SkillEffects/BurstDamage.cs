using UnityEngine;

public class BurstDamage : SkillEffectBase {
    private float damage;

    public BurstDamage(string name, float damage) : base(name) {
        this.damage = damage;
    }

    public override void Apply(Enemy enemy) {
        base.Apply(enemy);

        enemy.health.ReduceHP(damage);
        enemy.vfx.PlayOnDamageVFX();
    }

    public override void Update() {
        base.Update();
    }

    public override void Remove() {
        base.Remove();
    }
}
