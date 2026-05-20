using System;
using System.Collections;
using UnityEngine;

public class Player : Entity {
    public static event Action OnPlayerDeath;

    public PlayerInputSet input { get; private set; }
    public PlayerVFX vfx { get; private set; }
    public PlayerSkillManager skillManager { get; private set; }

    public EntityStats stats { get; private set; }
    public EntityHealth health { get; private set; }

    public EntityStats stats { get; private set; }
    public EntityHealth health { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerBasicAttackState basicAttackState { get; private set; }
    public PlayerJumpAttackState jumpAttackState { get; private set; }
    public PlayerDeadState deadState { get; private set;}
    public PlayerCounterAttackState counterAttackState { get; private set; }

    [Header("Attack info")]
    public Vector2 jumpAttackSpeed = new Vector2(5, -6);
    public Vector2[] attackSpped;
    public float attackMovementDuration = 0.1f;
    public float comboResetTime = 0.5f;
     
    private Coroutine queueAttackCo;

    [Header("Movement info")]
    public float moveSpeed = 8;
    public float jumpSpeed = 12;
    public Vector2 wallJumpSpeed = new Vector2(6, 12);
    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.8f;
    [Range(0, 1)]
    public float wallSlideMultiplier = 0.7f;

    [Space]
    public float dashDuration = 0.25f;
    public float dashSpeed = 20;

    public Vector2 moveInput { get; private set; }

    protected override void Awake() {
        base.Awake();

        input = new PlayerInputSet();
        stats = GetComponent<EntityStats>();
        health = GetComponent<EntityHealth>();
        vfx = GetComponent<PlayerVFX>();
        skillManager = GetComponent<PlayerSkillManager>();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "JumpFall");
        fallState = new PlayerFallState(this, stateMachine, "JumpFall");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "JumpFall");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        basicAttackState = new PlayerBasicAttackState(this, stateMachine, "BasicAttack");
        jumpAttackState = new PlayerJumpAttackState(this, stateMachine, "JumpAttack");
        deadState = new PlayerDeadState(this, stateMachine, "Dead");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
    }

    protected override void Start() {
        stateMachine.Initialize(idleState);
    }

    public float GetMoveSpeed() {
        return moveSpeed * speedMultiplier;
    }

    public override void EntityDeath() {
        base.EntityDeath();

        if (OnPlayerDeath != null) {
            OnPlayerDeath.Invoke();
        }

        stateMachine.ChangeState(deadState);
    }

    public void EnterBasicAttackStateWithDelay() {
        if (queueAttackCo != null) {
            StopCoroutine(queueAttackCo);
        }

        queueAttackCo = StartCoroutine(EnterBasicAttackStateWithDelayCo());
    }
    
    private IEnumerator EnterBasicAttackStateWithDelayCo() {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState); 
    }

    private void OnEnable() {
        input.Enable();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable() {
        input.Disable();
    }
}
