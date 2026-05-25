using UnityEngine;

public class Damage {
    public float value { get; private set; }
    public bool isCrit { get; private set; }

    public Damage(float value, bool isCrit) {
        this.value = value;
        this.isCrit = isCrit;
    }
}
