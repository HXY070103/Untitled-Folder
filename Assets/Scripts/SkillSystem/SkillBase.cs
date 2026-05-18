using System;
using UnityEngine;

public class SkillBase : MonoBehaviour {
    [Header("General info")]
    [SerializeField] private float cooldown = 3;
    public float skillDuration = 3;

    private float lastTimeUsed;

    protected virtual void Awake() {
        lastTimeUsed = float.MinValue;
    }

    public virtual void ApplySkill() {
        lastTimeUsed = Time.time;
    }

    public bool OnCooldown() {
        return Time.time < lastTimeUsed + cooldown;
    }
}
