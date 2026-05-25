using UnityEngine;

public class PlayerAnimationTrigger : EntityAnimationTrigger {
    private Player player;

    protected override void Awake() {
        base.Awake();

        player = GetComponentInParent<Player>();
    }

    private void ApplyAttackMovement() {
        player.heavyAttackState.ApplyAttackMovement();
    }
}
