using UnityEngine;

public class DamageArmorDebuff : SkillEffectBase {
    private float damageReduction;
    private float armorReduction;

    public DamageArmorDebuff(string name, float damageReduction, float armorReduction) : base(name, true) {
        this.damageReduction = damageReduction;
        this.armorReduction = armorReduction;
    }

    public override void Apply(Enemy enemy) {
        base.Apply(enemy);

        enemy.stats.damage.AddModifier(-damageReduction, name);
        enemy.stats.armor.AddModifier(-armorReduction, name);
    }

    public override void Update() {
        base.Update();
    }

    public override void Remove() {
        base.Remove();
    }
}
