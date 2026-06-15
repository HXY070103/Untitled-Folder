using UnityEngine;

public class Domain : MonoBehaviour {
    [Header("Domain info")]
    [SerializeField] private float expandSpeed = 2;
    [SerializeField] private float damage = 1;
    [SerializeField] private float damageInterval = 0.5f;

    private SkillTimeControl domainManager;
    private Vector3 targetScale;
    private bool isShrinking;
 
    public void SetUpDomain(SkillTimeControl domainManager) {
        this.domainManager = domainManager;
        targetScale = Vector3.one * domainManager.GetMaxSize();
        isShrinking = false;

        if (domainManager.UnLocked("Synchronized Field")) {
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
            transform.localScale = 
                Vector3.Lerp(transform.localScale, targetScale, expandSpeed * Time.deltaTime);
        }

        if (isShrinking && sizeDifference < 0.1f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        StartSlowDown(collision);

        if (domainManager.UnLocked("Temporal Corrosion")) {
            ApplyDomainDamage(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        EndSlowDown(collision);

        if (domainManager.UnLocked("Temporal Corrosion")) {
            EndDomainDamage(collision);
        }
    }

    private void StartSlowDown(Collider2D collision) {
        ISlowable target = collision.GetComponent<ISlowable>();
        target?.SlowDown(domainManager.GetSlowDownPercentage());
    }

    private void EndSlowDown(Collider2D collision) {
        ISlowable target = collision.GetComponent<ISlowable>();
        target?.RecoverSpeed();
    }

    private void ApplyDomainDamage(Collider2D collision) {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        enemyHealth?.StartDot(damage, damageInterval, float.PositiveInfinity);
    }

    private void EndDomainDamage(Collider2D collision) {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        enemyHealth?.StopDot();
    }
}
