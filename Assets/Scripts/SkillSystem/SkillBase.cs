using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SkillBase : MonoBehaviour {
    [SerializeField] protected string skillName;

    [TextArea]
    [SerializeField] protected string description;

    [Header("Skill Evolution")]
    [SerializeField] protected List<SkillEvolutionDisplayData> evolutions = new List<SkillEvolutionDisplayData>();

    [Header("Skill Upgrades")]
    [SerializeField] protected List<SkillUpgradeDisplayData> upgrades = new List<SkillUpgradeDisplayData>();

    public string SkillName => skillName;
    public string Description => description;
    public List<SkillEvolutionDisplayData> Evolutions => evolutions;
    public List<SkillUpgradeDisplayData> Upgrades => upgrades;


    protected PlayerStats stats;

    protected PlayerStats stats;

    [SerializeField] private float cooldown = 3;
    
    private float lastTimeUsed;
    private Coroutine handleSkillCo;
    private float remainingDuration;

    private bool isSkillActive;

    public float RemainingDuration => remainingDuration;

    public bool IsSkillActive => isSkillActive;
    public float SkillDuration => stats.GetSkillDuration();
    public float SkillDuration => stats.GetSkillDuration();

    protected virtual void Awake() {
        stats = GetComponentInParent<PlayerStats>();
        stats = GetComponentInParent<PlayerStats>();
        lastTimeUsed = float.MinValue;
        remainingDuration = 0;
        isSkillActive = false;
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
        handleSkillCo = null;
    }

    protected virtual void StartSkill() {
        lastTimeUsed = Time.time;
        isSkillActive = true;
        remainingDuration = stats.GetSkillDuration();
        remainingDuration = stats.GetSkillDuration();
    }

    protected virtual void EndSkill()
    {
        remainingDuration = 0;
        isSkillActive = false;
    }

    public bool OnCooldown() {
        return Time.time < lastTimeUsed + cooldown;
    }

    //以后解锁进化的时候currentSkill.UnlockEvolution("Wider Range");
    //upgradePanel.SetCurrentSkill(currentSkill);
    public void UnlockEvolution(string evolutionName) {
        foreach (var evo in evolutions) {
            if (evo.evolutionName == evolutionName) {
                evo.unlocked = true;
                break;
            }
        }
        Debug.Log("there is no evolution");
    }

    //当玩家获得强化时调用：currentSkill.UnlockSkillUpgrade("Blood Fight");
    //upgradePanel.SetCurrentSkill(currentSkill);
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
        Debug.Log("there is no upgrade");
    }
}
