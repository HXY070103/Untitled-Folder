using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SkillBase : MonoBehaviour{
    protected PlayerStats stats;

    [SerializeField] protected string skillName;
    [SerializeField] protected SkillType skillType;

    [TextArea]
    [SerializeField] protected string description;

    [Header("Skill Upgrades")]
    [SerializeField] protected List<SkillUpgradeDisplayData> upgrades = new List<SkillUpgradeDisplayData>();
    [SerializeField] protected List<SkillUpgradeDisplayData> betterUpgrades = new List<SkillUpgradeDisplayData>();
    [SerializeField] protected List<SkillUpgradeDisplayData> skillPhaseUpgrades = new List<SkillUpgradeDisplayData>();
    [SerializeField] protected float[] decreaseRate;
    [SerializeField] protected int decreaseIndex;
    [SerializeField] protected PlayerSkillManager playerSkillManager;

    public int numOfUpgradesGet;
    private bool level4Skill = false;
    public string SkillName => skillName;
    public SkillType SkillType => skillType;
    public string Description => description;
    public List<SkillUpgradeDisplayData> Upgrades => upgrades;
    public List<SkillUpgradeDisplayData> BetterUpgrades => betterUpgrades;
    public List<SkillUpgradeDisplayData> SkillPhaseUpgrades => skillPhaseUpgrades;

    [Header("Basic skill info")]
    [SerializeField] private float cooldown = 3;
    [SerializeField] private float skillDuration = 3;
    
    private float lastTimeEnd;
    private Coroutine handleSkillCo;
    protected float remainingDuration;
    private bool isSkillActive;
    private int numOfUse;

    public float RemainingDuration => remainingDuration;
    public bool IsSkillActive => isSkillActive;

    protected virtual void Awake() {
        stats = GetComponentInParent<PlayerStats>();
    
        lastTimeEnd = float.MinValue;
        ResetRuntimeState();

        decreaseRate = new float[6] {1f, 1f, 0.8f, 0.7f, 0.6f, 0.5f}; // normal, 1decay, 2decay, 3decay, 4decay, 5decay
        decreaseIndex = 0;
        numOfUse = 0;
        numOfUpgradesGet = 0;
    }

    public void ApplySkill() {
        if (handleSkillCo != null) {
            StopCoroutine(handleSkillCo);
        }

        handleSkillCo = StartCoroutine(HanldeSkillCo());
    }

    public IEnumerator HanldeSkillCo() {
        StartSkill();
        while(remainingDuration > 0) {
            remainingDuration -= Time.deltaTime;
            yield return null;
        }
        EndSkill(true);
    }

    protected virtual void StartSkill() {
        isSkillActive = true;
        remainingDuration = GetSkillDuration();
        numOfUse++;
        //IncreaseSkillDecay();
        playerSkillManager.UpdateSkillDecay(decreaseIndex);
    }

    protected virtual void EndSkill(bool endNormally) {
        ResetRuntimeState();
        UpdateSkillTimer();
    }

    public void ClearRuntimeSkillState() {
        StopSkillCoroutine();
        ResetRuntimeState();
    }

    public void CancelSkill() {
        if (!isSkillActive) {
            return;
        }

        StopSkillCoroutine();
        EndSkill(false);
    }

    private void StopSkillCoroutine() {
        if (handleSkillCo != null) {
            StopCoroutine(handleSkillCo);
            handleSkillCo = null;
        }
    }

    private void ResetRuntimeState() {
        isSkillActive = false;
        remainingDuration = 0;
    }

    private void UpdateSkillTimer() {
        lastTimeEnd = Time.time;
    }

    protected void IncreaseSkillDecay()
    {
        //if (unlock 磨损1 && numOfUse % 2 == 0  ||  !unlock 磨损1)
        if ((level4Skill && numOfUse % 2 == 0) || !level4Skill)
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

    public virtual float GetSkillDuration() {
        return skillDuration * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    public bool Unlocked(string upgradeName) {
        foreach (var upgrade in upgrades) {
            if (upgrade.upgradeName.Equals(upgradeName)) {
                return upgrade.unlocked;
            }
        }

        foreach(var upgrade in betterUpgrades) {
            if (upgrade.upgradeName.Equals(upgradeName)) {
                return upgrade.unlocked;
            }
        }

        foreach (var upgrade in skillPhaseUpgrades) {
            if (upgrade.upgradeName.Equals(upgradeName)) {
                return upgrade.unlocked;
            }
        }

        return false;
    }

    public void InterruptSkill() {
        remainingDuration = 0;
    }

    public bool OnCooldown() {
        return Time.time < lastTimeEnd + cooldown;
    }

    private float GetCurrentCooldown() {
        if (Time.time < lastTimeEnd)
        {
            return cooldown;
        }
        return Mathf.Max(0, cooldown - (Time.time - lastTimeEnd));
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
            lastTimeEnd = float.MinValue;
        } else
        {
            lastTimeEnd = Time.time - (cooldown - remaining);
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
                numOfUpgradesGet++;
                CheckIfNextPhase();
                return;
            }
        }
        foreach (var upgrade in betterUpgrades)
        {
            if (upgrade.upgradeName == upgradeName)
            {
                upgrade.unlocked = true;
                numOfUpgradesGet++;
                CheckIfNextPhase();
                return;
            }
        }
        Debug.Log("there is no upgrade");
    }

    private void UnlockedSkillPhaseUpgrade(string upgradeName)
    {
        foreach (var upgrade in skillPhaseUpgrades)
        {
            if (upgrade.upgradeName.Equals(upgradeName))
            {
                upgrade.unlocked = true;
                return;
            }
        }
    }

    private void CheckIfNextPhase()
    {
        if (numOfUpgradesGet == 2)
        {
            stats.ReachLevel2Skill();
            UnlockedSkillPhaseUpgrade("2 Upgrades");
        }
        if (numOfUpgradesGet == 4)
        {
            level4Skill = true;
            UnlockedSkillPhaseUpgrade("4 Upgrades");
        }
        if (numOfUpgradesGet == 6)
        {
            stats.ReachLevel6Skill();
            UnlockedSkillPhaseUpgrade("6 Upgrades");
        }
        if (numOfUpgradesGet == upgrades.Count + betterUpgrades.Count)
        {
            stats.ReachFullLevel();
            UnlockedSkillPhaseUpgrade("Full Upgrades");
        }
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

    public List<string> GetUnlockedBetterUpgrades()
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

    public List<string> GetUnlockedSkillPhaseUpgrades()
    {
        List<string> result = new List<string>();
        foreach (var upgrade in skillPhaseUpgrades)
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

    public void LoadUnlockedSkillPhaseUpgrades(List<string> unlocked)
    {
        numOfUpgradesGet = 0;
        foreach (var upgrade in upgrades)
        {
            if (upgrade != null && upgrade.unlocked)
            {
                numOfUpgradesGet++;
            }
        }
        foreach (var upgrade in betterUpgrades)
        {
            if (upgrade != null && upgrade.unlocked)
            {
                numOfUpgradesGet++;
            }
        }

        // Rebuild phase data for save files created before phase upgrades were saved.
        if (unlocked == null || unlocked.Count == 0)
        {
            unlocked = new List<string>();
            if (numOfUpgradesGet >= 2)
            {
                unlocked.Add("2 Upgrades");
            }
            if (numOfUpgradesGet >= 4)
            {
                unlocked.Add("4 Upgrades");
            }
            if (numOfUpgradesGet >= 6)
            {
                unlocked.Add("6 Upgrades");
            }
            if (numOfUpgradesGet == upgrades.Count + betterUpgrades.Count)
            {
                unlocked.Add("Full Upgrades");
            }
        }

        foreach (var upgrade in skillPhaseUpgrades)
        {
            if (upgrade != null)
            {
                upgrade.unlocked = unlocked.Contains(upgrade.upgradeName);
            }
        }

        level4Skill = Unlocked("4 Upgrades");
        stats.RestoreSkillPhaseBonuses(
            Unlocked("2 Upgrades"),
            Unlocked("6 Upgrades"),
            Unlocked("Full Upgrades"));
    }

    public void SetDecayLevel(int level)
    {
        decreaseIndex = Mathf.Clamp(level, 0, decreaseRate.Length - 1);
        playerSkillManager.UpdateSkillDecay(decreaseIndex);
    }

    public void SetNumOfUse(int num)
    {
        numOfUse = num;
    }

    public int GetNumOfUse()
    {
        return numOfUse;
    }

    private void OnDisable() {
        CancelSkill();
    }
}
