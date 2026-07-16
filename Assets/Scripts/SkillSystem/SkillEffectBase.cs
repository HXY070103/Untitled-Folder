using UnityEngine;

public class SkillEffectBase {
    public string name { get; private set; }
    public bool isPermanent { get; private set; }

    protected Enemy enemy;

    public SkillEffectBase(string name, bool isPermanent) {
        this.name = name;
        this.isPermanent = isPermanent;
    }

    public virtual void Apply(Enemy enemy) {
        this.enemy = enemy;
    }

    public virtual void Update() {}

    public virtual void Remove() {}
}
