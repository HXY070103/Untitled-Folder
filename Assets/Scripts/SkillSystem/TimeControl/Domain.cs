using UnityEngine;

public class Domain : MonoBehaviour {
    private SkillTimeControl skill;
    private Vector3 targetScale;
    private bool isShrinking;
 
    public void SetUpDomain(SkillTimeControl skill) {
        this.skill = skill;
        targetScale = Vector3.one * skill.GetMaxSize();
        isShrinking = false;

        if (skill.Unlocked("Synchronized Field")) {
            transform.SetParent(skill.transform);
        }
    }
    
    public void ShrinkDomain() {
        targetScale = Vector3.zero;
        isShrinking = true;
    }

    private void Update() {
        HandleScaling();
    }

    private void HandleScaling() {
        float sizeDifference = Mathf.Abs(transform.lossyScale.x - targetScale.x);

        if (sizeDifference > 0.1f) {
            transform.localScale = Vector3.Lerp(
                transform.localScale, targetScale, skill.expandSpeed * Time.deltaTime);
        }

        if (isShrinking && sizeDifference < 0.1f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        StartSlowDown(collision);

        EnemySkillEffectReceiver receiver
            = collision.GetComponent<EnemySkillEffectReceiver>();

        if (receiver == null) {
            return;
        }

        StartTemporalCorrosionEffect(receiver);
        StartVulnerableFieldEffect(receiver);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        EndSlowDown(collision);

        EnemySkillEffectReceiver receiver
            = collision.GetComponent<EnemySkillEffectReceiver>();
        
        if (receiver == null) {
            return;
        }

        EndTemporalCorrosionEffect(receiver);
        EndVulnerableFieldEffect(receiver);
    }

    private void StartSlowDown(Collider2D collision) {
        ISlowable target = collision.GetComponent<ISlowable>();
        target?.SlowDown(skill.GetSlowDownPercentage());
    }

    private void EndSlowDown(Collider2D collision) {
        ISlowable target = collision.GetComponent<ISlowable>();

        if (skill.Unlocked("Synchronized Field+")) {
            target?.RecoverSpeedWithDelay(skill.recoverSpeedDelay);
        } else {
            target?.RecoverSpeed();
        }
    }

    private void StartTemporalCorrosionEffect(EnemySkillEffectReceiver receiver) {
        if (!skill.Unlocked("Temporal Corrosion")) {
            return;
        }

        SkillEffectBase effect;

        if (skill.Unlocked("Temporal Corrosion+")) {
            int numOfDamage = (int) (skill.GetSkillDuration() / skill.damageInterval);
            float burstDamage = skill.damage * numOfDamage;
            effect = new BurstDamage("BurstDamage", burstDamage);
        } else {
            effect = new Dot("Dot", skill.damage, skill.damageInterval);
        }

        receiver.ApplySkillEffect(effect);
    }

    private void EndTemporalCorrosionEffect(EnemySkillEffectReceiver receiver) {
        if (!skill.Unlocked("Temporal Corrosion")) {
            return;
        }

        if (skill.Unlocked("Temporal Corrosion+")) {
            receiver.RemoveSkillEffect("BurstDamage");
        } else {
            receiver.RemoveSkillEffect("Dot");
        }
    }

    private void StartVulnerableFieldEffect(EnemySkillEffectReceiver receiver) {
        if (skill.Unlocked("Vulnerable Field")) {
            SkillEffectBase effect1 = new Vulnerability("Vulnerability", skill.damageMultiplier);    
            receiver.ApplySkillEffect(effect1);
        }

        if (skill.Unlocked("Vulnerable Field+")) {
            SkillEffectBase effect2 = new ArmorBreak("ArmorBreak");    
            receiver.ApplySkillEffect(effect2);
        }
    }

    private void EndVulnerableFieldEffect(EnemySkillEffectReceiver receiver) {
        if (skill.Unlocked("Vulnerable Field")) {
            receiver.RemoveSkillEffect("Vulnerability");
        }

        if (skill.Unlocked("Vulnerable Field+")) {   
            receiver.RemoveSkillEffect("ArmorBreak");
        }
    }
}
