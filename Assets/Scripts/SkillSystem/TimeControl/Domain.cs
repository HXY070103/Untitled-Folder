using UnityEngine;

public class Domain : MonoBehaviour {
    private SkillTimeControl domainManager;
    private Vector3 targetScale;
    private bool isShrinking;
 
    public void SetUpDomain(SkillTimeControl domainManager) {
        this.domainManager = domainManager;
        targetScale = Vector3.one * domainManager.GetMaxSize();
        isShrinking = false;

        if (domainManager.Unlocked("Synchronized Field")) {
            transform.SetParent(domainManager.transform);
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
                transform.localScale, targetScale, domainManager.expandSpeed * Time.deltaTime);
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
        target?.SlowDown(domainManager.GetSlowDownPercentage());
    }

    private void EndSlowDown(Collider2D collision) {
        ISlowable target = collision.GetComponent<ISlowable>();

        if (domainManager.Unlocked("Synchronized Field+")) {
            target?.RecoverSpeedWithDelay(domainManager.recoverSpeedDelay);
        } else {
            target?.RecoverSpeed();
        }
    }

    private void StartTemporalCorrosionEffect(EnemySkillEffectReceiver receiver) {
        if (!domainManager.Unlocked("Temporal Corrosion")) {
            return;
        }

        SkillEffectBase effect;

        if (domainManager.Unlocked("Temporal Corrosion+")) {
            int numOfDamage = (int) (domainManager.GetSkillDuration() / domainManager.damageInterval);
            float burstDamage = domainManager.damage * numOfDamage;
            effect = new BurstDamage("BurstDamage", burstDamage);
        } else {
            effect = new Dot("Dot", domainManager.damage, domainManager.damageInterval);
        }

        receiver.ApplySkillEffect(effect);
    }

    private void EndTemporalCorrosionEffect(EnemySkillEffectReceiver receiver) {
        if (!domainManager.Unlocked("Temporal Corrosion")) {
            return;
        }

        if (domainManager.Unlocked("Temporal Corrosion+")) {
            receiver.RemoveSkillEffect("BurstDamage");
        } else {
            receiver.RemoveSkillEffect("Dot");
        }
    }

    private void StartVulnerableFieldEffect(EnemySkillEffectReceiver receiver) {
        if (domainManager.Unlocked("Vulnerable Field")) {
            SkillEffectBase effect1 = new Vulnerability("Vulnerability", domainManager.damageMultiplier);    
            receiver.ApplySkillEffect(effect1);
        }

        if (domainManager.Unlocked("Vulnerable Field+")) {
            SkillEffectBase effect2 = new ArmorBreak("ArmorBreak");    
            receiver.ApplySkillEffect(effect2);
        }
    }

    private void EndVulnerableFieldEffect(EnemySkillEffectReceiver receiver) {
        if (domainManager.Unlocked("Vulnerable Field")) {
            receiver.RemoveSkillEffect("Vulnerability");
        }

        if (domainManager.Unlocked("Vulnerable Field+")) {   
            receiver.RemoveSkillEffect("ArmorBreak");
        }
    }
}
