using UnityEngine;

public class PlayerBasicAttackState : PlayerState {
    private float attackMovementTimer;
    private float lastTimeAttack;

    private bool comboQueued;
    private const int FirstComboIndex = 1;
    private int comboIndex = 1;
    private int comboLimit = 3;

    private int attackDir;

    public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : 
        base(player, stateMachine, animBoolName) {
        if (comboLimit != player.attackSpeed.Length) {
            Debug.LogWarning("Combo limit is adjusted to match attack speed array!");
            comboLimit = player.attackSpeed.Length;
        }
    }

    public override void Enter() {
        base.Enter();

        comboQueued = false;
        ResetComboIndexIfNeeded();
        anim.SetInteger("BasicAttackIndex", comboIndex); 

        attackDir = player.moveInput.x != 0 ? (int) player.moveInput.x : player.facingDir;
        ApplyAttackMovement();
    }

    public override void Update() {
        base.Update();

        HandleAttackMovement();

        if (input.Player.Attack.WasPressedThisFrame()) {
            QueueNextAttack();
        }

        if (triggerCalled) {
            HandleStateExit();
        }
    }

    public override void Exit() {
        base.Exit(); 

        lastTimeAttack = Time.time;
        comboIndex += 1;
    }

    private void HandleStateExit() {
        if (comboQueued) {
            anim.SetBool("BasicAttack", false);
            player.EnterBasicAttackStateWithDelay();
        } else {
            stateMachine.ChangeState(player.idleState);
        }
    }

    private void QueueNextAttack() {
        if (comboIndex < comboLimit) {
            comboQueued = true;
        }
    }

    private void ResetComboIndexIfNeeded() {
        if (comboIndex > comboLimit || Time.time > lastTimeAttack + player.comboResetTime) {
            comboIndex = FirstComboIndex;
        }
    }

    private void ApplyAttackMovement() {
        attackMovementTimer = player.attackMovementDuration;
        player.Move(
                attackDir * player.attackSpeed[comboIndex - FirstComboIndex].x, 
                player.attackSpeed[comboIndex - FirstComboIndex].y
            );
    }

    private void HandleAttackMovement() {
        attackMovementTimer -= Time.deltaTime;

        if (attackMovementTimer < 0) {
            player.Move(0, rb.linearVelocity.y);
        }
    }
}
 