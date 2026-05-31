using UnityEngine;

public class FireballController : MonoBehaviour {
    [Header("Movement info")]
    [SerializeField] private float chaseSpeed = 15;
    [SerializeField] private float minWaitingTime = 0.5f;
    [SerializeField] private float maxWaitingTime = 1;

    private SkillLifeSteal fireballManager;
    private bool canChase;

    public void SetUpFireball(SkillLifeSteal fireballManager) {
        this.fireballManager = fireballManager;
        canChase = false;

        float waitingTime = Random.Range(minWaitingTime, maxWaitingTime);
        Invoke(nameof(StartChasing), waitingTime);
    }

    private void Update() {
        if (canChase) {
            HandleChasing();
        }
    }

    private void HandleChasing() {
        Vector3 targetPosition = fireballManager.transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        
        if (distanceToTarget > 0.5f) {
            transform.position = 
                Vector3.Lerp(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
        } else {
            fireballManager.HealingAtEnd();
            Destroy(gameObject);
        }
    }

    private void StartChasing() {
        canChase = true;
    }
}
