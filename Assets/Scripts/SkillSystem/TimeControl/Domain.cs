using UnityEngine;

public class Domain : MonoBehaviour {
    [SerializeField] private float expandSpeed = 2;

    private SkillTimeControl domainManager;
    private Vector3 targetScale;
    private bool isShrinking;
 
    public void SetUpDomain(SkillTimeControl domainManager) {
        this.domainManager = domainManager;
        targetScale = Vector3.one * domainManager.GetMaxSize();
        isShrinking = false;
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
        ISlowable target = collision.GetComponent<ISlowable>();
        
        if (target == null) {
            return;
        }

        target.SlowDown(domainManager.GetSlowDownPercentage());
    }

    private void OnTriggerExit2D(Collider2D collision) {
        ISlowable target = collision.GetComponent<ISlowable>();

        if (target == null) {
            return;
        }

        target.RecoverSpeed();
    }
}
