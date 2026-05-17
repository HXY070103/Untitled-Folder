using System;
using UnityEngine;

[Serializable]
public class StatModifier {
    private float value;
    private string source;

    public StatModifier(float value, string source) {
        this.value = value;
        this.source = source;
    }

    public float GetValue() {
        return value;
    }

    public string GetSource() {
        return source;
    }
}
