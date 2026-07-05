using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat {
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();
    [SerializeField] private List<StatModifier> multipliers = new List<StatModifier>();

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

    public void AddModifier(float value, string source) {
        StatModifier modToAdd = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        needToCalculate = true;
    }

    public void RemoveModifier(string source) {
        modifiers.RemoveAll(mod => mod.GetSource().Equals(source));
        needToCalculate = true;
    }

    public void AddMultiplier(float value, string source) {
        StatModifier mulToAdd = new StatModifier(value, source);
        multipliers.Add(mulToAdd);
        needToCalculate = true;
    }

    public void RemoveMultiplier(string source) {
        multipliers.RemoveAll(mul => mul.GetSource().Equals(source));
        needToCalculate = true;
    }

    private float GetFinalValue() {
        float finalValue = baseValue;

        foreach (var mod in modifiers) {
            finalValue += mod.GetValue() ;
        }

        foreach (var mul in multipliers) {
            finalValue *= mul.GetValue();
        }

        return finalValue;
    }
}
