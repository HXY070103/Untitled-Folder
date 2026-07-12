using UnityEngine;

public class FireballController : MonoBehaviour {
    [Header("Movement info")]
    [SerializeField] private float chaseSpeed = 15;
    [SerializeField] private float minWaitingTime = 0.5f;
    [SerializeField] private float maxWaitingTime = 1;

    private SkillLifeSteal skill;
    private bool canChase;

    public void SetUpFireball(SkillLifeSteal fireballManager) {
        this.skill = fireballManager;
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
        Vector3 targetPosition = skill.transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        
        if (distanceToTarget > 0.5f) {
            transform.position = 
                Vector3.Lerp(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
        } else {
            skill.HealingAtEnd();
            Destroy(gameObject);
        }
    }

    private void StartChasing() {
        canChase = true;
    }
}
