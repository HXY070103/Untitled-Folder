using UnityEngine;

public class ArmorBreak : SkillEffectBase{
    public ArmorBreak(string name) : base(name) {}

    public override void Apply(Enemy enemy) {
        base.Apply(enemy);

        enemy.stats.armor.AddMultiplier(0, "ArmorBreak");
    }

    public override void Update() {
        base.Update();
    }

    public override void Remove() {
        base.Remove();

        enemy.stats.armor.RemoveMultiplier("ArmorBreak");
    }
}
