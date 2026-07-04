using UnityEngine;

public class SkillEffectBase {
    public string name { get; private set; }

    protected Enemy enemy;

    public SkillEffectBase(string name) {
        this.name = name;
    }

    public virtual void Apply(Enemy enemy) {
        this.enemy = enemy;
    }

    public virtual void Update() {}

    public virtual void Remove() {}
}
