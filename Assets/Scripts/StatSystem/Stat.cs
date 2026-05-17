using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat {
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    private bool needToCalculate = true;
    private float finalValue;
   
    public float GetValue() {
        if (needToCalculate) {
            finalValue = GetFinalValue();
            needToCalculate = false;
        }

        return finalValue;
    }

    public void SetBaseValue(float baseValue) {
        this.baseValue = baseValue;
    }

    public void addModifier(float value, string source) {
        StatModifier modToAdd = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        needToCalculate = true;
    }

    public void RemoveModifier(string source) {
        modifiers.RemoveAll(mod => mod.GetSource().Equals(source));
        needToCalculate = true;
    }

    private float GetFinalValue() {
        float finalValue = baseValue;

        foreach (var mod in modifiers) {
            finalValue += mod.GetValue() ;
        }

        return finalValue;
    }
}
