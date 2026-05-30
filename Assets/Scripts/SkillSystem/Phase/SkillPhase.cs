using System.Collections;
using UnityEngine;

public class SkillPhase : SkillBase {
    private PlayerVFX vfx;
    private EntityHealth health;
    private Player player;

    [SerializeField] private GameObject phaseTrackerPrefab;
    [SerializeField] private float damageMultiplier = 2;
    [SerializeField] private float speedUpPercentage = 1.5f;
    [SerializeField] private float lockAttackDuration = 0.1f;
    [SerializeField] private float perfectPhaseWindow = 0.2f;

    private GameObject phaseTracker;
    private bool inPerfectPhaseWindow;
    public bool isPerfectPhase;
    private Coroutine handlePerfectPhaseWindowCo;

    public bool InPerfectPhaseWindow => inPerfectPhaseWindow;

    public float GetSpeedUpPercentage() {
        return speedUpPercentage * stats.GetSkillEffect();
    }

    public float GetDamageMultiplier() {
        return damageMultiplier * stats.GetSkillEffect();
    }

    protected override void Awake() {
        base.Awake();

        vfx = GetComponentInParent<PlayerVFX>();
        health = GetComponentInParent<EntityHealth>();
        player = GetComponentInParent<Player>();
    }

    protected override void StartSkill() {
        base.StartSkill();
        
        isPerfectPhase = false;
        SetPerfectWindow();

        player.SetNextAttackToHeavyAttack();
        player.LockAttack(lockAttackDuration);

        player.SpeedUp(GetSpeedUpPercentage());
        vfx.DoFadeEffect();
        health.EnterInvulnerableState();

        if (UnLocked("Weakness Marking")) {
            CreatePhaseTracker();
        }
    }

    protected override void EndSkill() {
        base.EndSkill();
        
        player.RecoverSpeed();
        vfx.ResetTransparency();
        health.ExitInvulnerableState();

        if (UnLocked("Weakness Marking")) {
            DestroyPhaseTracker();
        }
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
    }

    private void SetPerfectWindow() {
        if (handlePerfectPhaseWindowCo != null) {
            StopCoroutine(handlePerfectPhaseWindowCo);
        }

        handlePerfectPhaseWindowCo = StartCoroutine(HandlePerfectWindow());
    }

    private IEnumerator HandlePerfectWindow() {
        inPerfectPhaseWindow = true;
        yield return new WaitForSeconds(perfectPhaseWindow);
        inPerfectPhaseWindow = false;
    }
}
