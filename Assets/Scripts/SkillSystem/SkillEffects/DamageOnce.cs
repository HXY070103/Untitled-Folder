using UnityEngine;

public class DamageOnce : SkillEffectBase {
    private float damage;

    public DamageOnce(string name, float damage) : base(name, true) {
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
