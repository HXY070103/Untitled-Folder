using UnityEngine;

public class PlayerCombat : EntityCombat {
    [Header("Conuter attack info")]
    [SerializeField] private float counterRecoveryDuration = 0.25f;

    public bool CounterAttackPerformed() {
        bool hasPerformedCounter = false;

        foreach (var target in GetDetectedColliders()) {
            ICounterable counterable = target.GetComponent<ICounterable>();
            
            if (counterable == null) {
                continue;
            }

            if (counterable.CanBeCountered()) {
                counterable.HandleCounter();
                hasPerformedCounter = true;
            }
        }

        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDuration() {
        return counterRecoveryDuration;
    }
}
