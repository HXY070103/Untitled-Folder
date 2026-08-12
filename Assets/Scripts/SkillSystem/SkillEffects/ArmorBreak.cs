using UnityEngine;

public class ArmorBreak : SkillEffectBase{
    public ArmorBreak(string name) : base(name, false) {}

    public override void Apply(Enemy enemy) {
        base.Apply(enemy);

        enemy.stats.armor.AddMultiplier(0, name);
    }

    public override void Update() {
        base.Update();
    }

    public override void Remove() {
        base.Remove();

        enemy.stats.armor.RemoveMultiplier(name);
    }
}
