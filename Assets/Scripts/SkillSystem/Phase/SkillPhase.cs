using System.Collections;
using UnityEngine;

public class SkillPhase : SkillBase {
    private EntityHealth health;
    private Player player;
    private PlayerVFX vfx;

    [Header("Skill info")]
    [SerializeField] private float damageMultiplier = 2;
    [SerializeField] private float speedUpPercentage = 1.5f;
    [SerializeField] private float perfectPhaseWindow = 0.2f;
    [SerializeField] private float lockAttackDuration = 0.1f;
    [SerializeField] private float heavyAttackExpiryTime = 10;

    private Coroutine handleHeavyAttackCo;

    [Header("Weakness marking info")]
    [SerializeField] private GameObject phaseTrackerPrefab;
    private GameObject phaseTracker;

    [Header("Shadow execution info")]
    [SerializeField] private float bonusPhaseDuration = 1;
    private bool isBonusPhase = false;

    [Header("Chain strikes info")]
    [SerializeField] private int bonusNumOfHeavyAttack = 3;

    [Header("Perfect echo info")]
    [SerializeField] private float bonusHP = 10;

    private bool inPerfectPhaseWindow = false;
    private bool isPerfectPhase = false;
    private Coroutine handlePerfectPhaseWindowCo;
    public bool InPerfectPhaseWindow => inPerfectPhaseWindow;

    public float GetSpeedUpPercentage() {
        return speedUpPercentage * stats.GetSkillEffect();
    }

    public float GetDamageMultiplier() {
        if (UnlockedForBetterUpgrade("Fixed Attack"))
        {
            return damageMultiplier * stats.GetSkillEffect();
        }
        return damageMultiplier * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
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
        SetPerfectWindow();

        HandleHeavyAttack();

        player.SpeedUp(GetSpeedUpPercentage());
        vfx.DoFadeEffect();
        health.EnterInvulnerableState();

        if (UnLocked("Weakness Marking")) {
            CreatePhaseTracker();
        }
    }

    protected override void EndSkill() {
        base.EndSkill();

        player.gameObject.layer = LayerMask.NameToLayer("Player");
        
        player.RecoverSpeed();
        vfx.ResetTransparency();
        health.ExitInvulnerableState();

        isBonusPhase = false;

        if (UnLocked("Weakness Marking")) {
            DestroyPhaseTracker();
        }

        if (UnLocked("Perfect Echo") && isPerfectPhase) {
            health.Healing(bonusHP);
        }

        if (UnlockedForBetterUpgrade("Perfect Timing"))
        {
            DecreaseDecay(1);
        }
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

    private int GetNumOfHeavyAttack() {
        if (UnLocked("Chain Strikes") && isPerfectPhase) {
            return bonusNumOfHeavyAttack;
        }

        return 1;
    }

    public override float GetSkillDuration() {
        if (isBonusPhase) {
            return bonusPhaseDuration;
        }

        return base.GetSkillDuration();
    }

    public void EnableBonusPhase() {
        EnemyHealth.OnEnemyDeath += EnterBonusPhase;
    }

    public void DisableBonusPhase() {
        EnemyHealth.OnEnemyDeath -= EnterBonusPhase;
    }

    public void TryToEnterBonusPhase() {
        if (isBonusPhase) {
            ApplySkill();
        }
    }

    private void EnterBonusPhase() {
        isBonusPhase = true;
    }

    private void CreatePhaseTracker() {
        phaseTracker = Instantiate(phaseTrackerPrefab, transform.position, Quaternion.identity);
        phaseTracker.transform.SetParent(transform);
        phaseTracker.GetComponent<PhaseTrackerController>().SetUpPhaseTracker(this);
    }

    private void DestroyPhaseTracker() {
        Destroy(phaseTracker);
    }

    public void PerfectPhaseTriggered() {
        isPerfectPhase = true;
        Debug.Log("perfect phase");
    }

    private void SetPerfectWindow() {
        if (handlePerfectPhaseWindowCo != null) {
            StopCoroutine(handlePerfectPhaseWindowCo);
        }

        handlePerfectPhaseWindowCo = StartCoroutine(HandlePerfectPhaseWindow());
    }

    private IEnumerator HandlePerfectPhaseWindow() {
        inPerfectPhaseWindow = true;
        yield return new WaitForSeconds(GetPerfectPhaseWindow());
        inPerfectPhaseWindow = false;
    }

    public float GetPerfectPhaseWindow() {
        return perfectPhaseWindow * stats.GetSkillEffect() * decreaseRate[decreaseIndex];
    }
}