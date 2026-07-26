using System.Collections;
using UnityEngine;

public class SkillPhase : SkillBase {
    private EntityHealth health;
    private Player player;
    private PlayerVFX vfx;

    [Header("Skill info")]
    [SerializeField] private GameObject hurtCheckerPrefab;
    [SerializeField] private float damageMultiplier = 2;
    [SerializeField] private float speedUpPercentage = 1.5f;
    [SerializeField] private float perfectPhaseWindow = 0.2f;
    [SerializeField] private float lockAttackDuration = 0.1f;
    [SerializeField] private float heavyAttackExpiryTime = 10;
    public float heavyAttackSpeed = 10;

    private Coroutine handleHeavyAttackCo;
    private bool isPerfectPhase;

    [Header("Perfect echo info")]
    [SerializeField] private float bonusHP = 15;

    [Header("Chain strikes info")]
    [SerializeField] private int bonusNumOfHeavyAttack = 3;

    [Header("Weakness marking info")]
    [SerializeField] private GameObject phaseTrackerPrefab;
    public float damageReduction = 10;
    public float armorReduction = 50;
    public float damage = 15;

    private GameObject phaseTracker;

    [Header("Shadow assassination info")]
    [SerializeField] private float HPThreshold = 0.5f;
    [SerializeField] private float bonusDamage = 50;
    [SerializeField] private float betterHPThreshold = 0.75f;
    [SerializeField] private float betterBonusDamage = 75;

    [Header("Chain phantasm info")]
    [SerializeField] private float bonusPhaseDuration = 1.5f;

    private bool hasBonusPhase = false;

    public override float GetSkillDuration() {
        if (hasBonusPhase) {
            return bonusPhaseDuration;
        }

        return base.GetSkillDuration();
    }

    public float GetSpeedUpPercentage() {
        return speedUpPercentage * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    public float GetDamageMultiplier() {
        return damageMultiplier * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }

    public float GetHPThreshold() {
        float threshold = Unlocked("Shadow Assassination+")
            ? betterHPThreshold
            : HPThreshold;

        if (isPerfectPhase) {
            threshold += 0.1f;
        }

        return Mathf.Clamp01(threshold);
    }

    public float GetBonusDamage() {
        if (Unlocked("Shadow Assassination+")) {
            return betterBonusDamage;
        }

        return bonusDamage;
    }

    protected override void Awake() {
        base.Awake();

        health = GetComponentInParent<EntityHealth>();
        player = GetComponentInParent<Player>();
        vfx = GetComponentInParent<PlayerVFX>();
    }

    protected override void StartSkill() {
        base.StartSkill();

        player.gameObject.layer = LayerMask.NameToLayer("Untargetable");
        
        isPerfectPhase = false;
        SetPerfectPhaseWindow();

        HandleHeavyAttack();
        player.SpeedUp(GetSpeedUpPercentage());
        vfx.DoFadeEffect();

        CreatePhaseTracker();
    }

    protected override void EndSkill(bool endNormally) {
        base.EndSkill(endNormally);

        player.gameObject.layer = LayerMask.NameToLayer("Player");
        
        player.RecoverSpeed();
        vfx.ResetTransparency();
        DestroyPhaseTracker();

        hasBonusPhase = false;

        if (endNormally) {
            ApplyPerfectEchoReward();
        }
    }

    protected override void IncreaseSkillDecay() {
        if (hasBonusPhase) {
            return;
        }

        base.IncreaseSkillDecay();
    }

    private int GetNumOfHeavyAttack() {
        if ((Unlocked("Chain Strikes") && isPerfectPhase) || Unlocked("Chain Strikes+")) {
            return bonusNumOfHeavyAttack;
        }

        return 1;
    }

    private void ApplyPerfectEchoReward() {
        if ((Unlocked("Perfect Echo") && isPerfectPhase) || Unlocked("Perfect Echo+")) {
            health.Healing(bonusHP);
        }
    }

    private void CreatePhaseTracker() {
        if (!Unlocked("Weakness Marking")) {
            return;
        }

        phaseTracker = Instantiate(phaseTrackerPrefab, transform.position, Quaternion.identity);
        phaseTracker.GetComponent<PhaseTrackerController>().SetUpPhaseTracker(this);
        phaseTracker.transform.SetParent(transform);
    }

    private void DestroyPhaseTracker() {
        if (!Unlocked("Weakness Marking")) {
            return;
        }

        Destroy(phaseTracker);
    }

    public void EnableBonusPhase() {
        EnemyHealth.OnEnemyDeath += EnterBonusPhase;
    }

    public void DisableBonusPhase() {
        EnemyHealth.OnEnemyDeath -= EnterBonusPhase;
    }

    private void EnterBonusPhase() {
        hasBonusPhase = true;
    }

    public void TryToEnterBonusPhase() {
        if (hasBonusPhase) {
            ApplySkill();
        }
    }

    private void SetPerfectPhaseWindow() {
        GameObject hurtChecker = 
            Instantiate(hurtCheckerPrefab, transform.position, Quaternion.identity);
        hurtChecker.transform.SetParent(transform);
        hurtChecker.GetComponent<HurtCheckerController>().SetUpHurtChecker(this);
        Destroy(hurtChecker, perfectPhaseWindow);
    }

    public void PerfectPhaseTriggered() {
        isPerfectPhase = true;
        Debug.Log("perfect phase");
    }

    private void HandleHeavyAttack() {
        if (handleHeavyAttackCo != null) {
            StopCoroutine(handleHeavyAttackCo);
        }

        handleHeavyAttackCo = StartCoroutine(HandleHeavyAttackCo());
    }

    private IEnumerator HandleHeavyAttackCo() {
        player.LockAttack();
        yield return new WaitForSeconds(lockAttackDuration);
        player.UnLockAttack();
        player.SetHeavyAttack(heavyAttackExpiryTime, GetNumOfHeavyAttack());
    }
}
