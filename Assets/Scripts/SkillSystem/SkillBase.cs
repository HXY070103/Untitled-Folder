using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using NUnit.Framework;

public class SkillBase : MonoBehaviour{
    protected PlayerStats stats;

    [SerializeField] protected string skillName;
    [SerializeField] protected SkillType skillType;

    [TextArea]
    [SerializeField] protected string description;

    [Header("Skill Upgrades")]
    [SerializeField] protected List<SkillUpgradeDisplayData> upgrades = new List<SkillUpgradeDisplayData>();
    [SerializeField] protected List<SkillUpgradeDisplayData> betterUpgrades = new List<SkillUpgradeDisplayData>();
    [SerializeField] protected float[] decreaseRate;
    [SerializeField] protected int decreaseIndex;
    [SerializeField] protected PlayerSkillManager playerSkillManager;

    public string SkillName => skillName;
    public SkillType SkillType => skillType;
    public string Description => description;
    public List<SkillUpgradeDisplayData> Upgrades => upgrades;
    public List<SkillUpgradeDisplayData> BetterUpgrades => betterUpgrades;

    [Header("Basic skill info")]
    [SerializeField] private float cooldown = 3;
    [SerializeField] private float skillDuration = 3;
    
    private float lastTimeUsed;
    private Coroutine handleSkillCo;
    protected float remainingDuration;
    private bool isSkillActive;
    private int numOfUse;

    public float RemainingDuration => remainingDuration;
    public bool IsSkillActive => isSkillActive;

    protected virtual void Awake() {
        stats = GetComponentInParent<PlayerStats>();
    
        lastTimeUsed = float.MinValue;
        remainingDuration = 0;
        isSkillActive = false;
        decreaseRate = new float[6] {1f, 1f, 0.8f, 0.7f, 0.6f, 0.5f}; // normal, 1decay, 2decay, 3decay, 4decay, 5decay
        decreaseIndex = 0;
        numOfUse = 0;
    }

    public void ApplySkill() {
        if (handleSkillCo != null) {
            StopCoroutine(handleSkillCo);
        }

        handleSkillCo = StartCoroutine(HanldeSkill());
    }

    public IEnumerator HanldeSkill() {
        StartSkill();
        while(remainingDuration > 0) {
            remainingDuration -= Time.deltaTime;
            yield return null;
        }
        EndSkill();
    }

    protected virtual void StartSkill() {
        lastTimeUsed = Time.time;
        isSkillActive = true;
        remainingDuration = GetSkillDuration();
        numOfUse++;
        IncreaseSkillDecay();
        playerSkillManager.UpdateSkillDecay(decreaseIndex);
    }
    
    protected void IncreaseSkillDecay()
    {
        //if (unlock 磨损1 && numOfUse % 2 == 0  ||  !unlock 磨损1)
        if ((UnlockedForBetterUpgrade("Stable") && numOfUse % 2 == 0) || !UnlockedForBetterUpgrade("Stable"))
        {
            if (decreaseIndex == decreaseRate.Length - 1) {
                PlayerHealth playerHealth = GetComponentInParent<PlayerHealth>();
                if (stats.GetMaxHP() >= 0.3 * playerHealth.GetInitialMaxHP())
                {
                    playerHealth.DecreaseMaxHP(stats.GetMaxHP() * 0.2f, "Skill Decay");
                }
            }
            decreaseIndex = Mathf.Min(decreaseIndex + 1, decreaseRate.Length - 1);
        }
    }

    protected virtual void EndSkill() {
        isSkillActive = false;
        remainingDuration = 0;
    }

    public virtual float GetSkillDuration() {
        if (UnlockedForBetterUpgrade("Fixed Time"))
        {
            return skillDuration * stats.GetSkillEffect();
        }
        return skillDuration * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    public bool UnLocked(string upgradeName) {
        foreach (var upgrade in upgrades) {
            if (upgrade.upgradeName.Equals(upgradeName)) {
                return upgrade.unlocked;
            }
        }

        return false;
    }

    public bool UnlockedForBetterUpgrade(string upgradeName)
    {
        foreach(var upgrade in betterUpgrades)
        {
            if (upgrade.upgradeName.Equals(upgradeName))
            {
                return upgrade.unlocked;
            }
        }
        return false;
    }

    public void InterruptSkill() {
        remainingDuration = 0;
    }

    public bool OnCooldown() {
        return Time.time < lastTimeUsed + cooldown;
    }

    public float GetCurrentCooldown() {
        return Mathf.Max(0, cooldown - (Time.time - lastTimeUsed));
    }

    public float GetReaminCooldown()
    {
        return GetCurrentCooldown();
    }

    public void SetRemainCooldown(float remaining)
    {
        remaining = Mathf.Clamp(remaining, 0, cooldown);
        if (remaining <= 0)
        {
            lastTimeUsed = float.MinValue;
        } else
        {
            lastTimeUsed = Time.time - (cooldown - remaining);
        }
    }

    public float GetMaxCooldown() {
        return cooldown;
    }

    public void DecreaseDecay(int decreaseLevel) {
        decreaseIndex = Mathf.Max(decreaseIndex - decreaseLevel, 0);
        playerSkillManager.UpdateSkillDecay(decreaseIndex);
    }

    public int GetDecayLevel() {
        return decreaseIndex;
    }

    public void UpdateSkillDecay(int skillDecay)
    {
        decreaseIndex = skillDecay;
    }

    //当玩家获得强化时调用：currentSkill.UnlockSkillUpgrade("Blood Fight");
    public void UnlockSkillUpgrade(string upgradeName)
    {
        foreach (var upgrade in upgrades)
        {
            if (upgrade.upgradeName == upgradeName)
            {
                upgrade.unlocked = true;
                return;
            }
        }
        foreach (var upgrade in betterUpgrades)
        {
            if (upgrade.upgradeName == upgradeName)
            {
                upgrade.unlocked = true;
                return;
            }
        }
        Debug.Log("there is no upgrade");
    }

    public List<string> GetUnlockedUpgrades()
    {
        List<string> result = new List<string>();

        foreach (var upgrade in upgrades)
        {
            if (upgrade != null && upgrade.unlocked)
            {
                result.Add(upgrade.upgradeName);
            }
        }
        return result;
    }

    public List<string> GetUnLockedBetterUpgrades()
    {
        List<string> result = new List<string>();

        foreach (var upgrade in betterUpgrades)
        {
            if (upgrade != null && upgrade.unlocked)
            {
                result.Add(upgrade.upgradeName);
            }
        }
        return result;
    }

    public void LoadUnlockedUpgrades(List<string> unlocked)
    {
        foreach (var upgrade in upgrades)
        {
            if (upgrade != null && unlocked.Contains(upgrade.upgradeName))
            {
                upgrade.unlocked = true;
            } else
            {
                upgrade.unlocked = false;
            }
        }
    }

    public void LoadUnlockedBetterUpgrades(List<string> unlocked)
    {
        foreach (var upgrade in betterUpgrades)
        {
            if (upgrade != null && unlocked.Contains(upgrade.upgradeName))
            {
                upgrade.unlocked = true;
            } else
            {
                upgrade.unlocked = false;
            }
        }
    }

    public void SetDecayLevel(int level)
    {
        decreaseIndex = Mathf.Clamp(level, 0, decreaseRate.Length - 1);
        playerSkillManager.UpdateSkillDecay(decreaseIndex);
    }

    public void ClearRuntimeSkillState()
    {
        if (handleSkillCo != null)
        {
            StopCoroutine(handleSkillCo);
            handleSkillCo = null;
        }
        isSkillActive = false;
        remainingDuration = 0;
    }

    public void SetNumOfUse(int num)
    {
        numOfUse = num;
    }

    public int GetNumOfUse()
    {
        return numOfUse;
    }
}
