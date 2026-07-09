using System;
using System.Collections;
using UnityEngine;

public class Player : Entity {
    public static event Action OnPlayerDeath;
    public static Player instance;

    public PlayerInputSet input { get; private set; }
    public PlayerVFX vfx { get; private set; }
    public PlayerSFX sfx { get; private set; }
    public PlayerSkillManager skillManager { get; private set; }
    public PlayerStats stats { get; private set; }
    public PlayerHealth health { get; private set; }
    public PlayerCombat combat { get; private set; }

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
    public PlayerHeavyAttackState heavyAttackState { get; private set; }

    [Header("Attack info")]
    public Vector2 heavyAttackSpeed = new Vector2(5, 1.5f);
    public Vector2 jumpAttackSpeed = new Vector2(5, -6);
    public Vector2[] attackSpeed;
    public float attackMovementDuration = 0.1f;
    public float comboResetTime = 0.5f;
    public float attackAnimSpeedMultiplier = 1;
    
    private Coroutine queueAttackCo;
    private Coroutine setHeavyAttackCo;
    private bool isHeavyAttack = false;
    private int numOfHeavyAttack = 0;
    private bool canAttack = true;

    public bool IsHeavyAttack => isHeavyAttack;
    public int NumOfHeavyAttack => numOfHeavyAttack;
    public bool CanAttack => canAttack;


    [Header("Movement info")]
    public float moveSpeed = 8;
    public float jumpSpeed = 12;
    public float wallJumpDuration = 0.2f;
    public Vector2 wallJumpSpeed = new Vector2(6, 12);
    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.8f;
    [Range(0, 1)]
    public float wallSlideMultiplier = 0.7f;
    public float dashDuration = 0.25f;
    public float dashSpeed = 20;
    public float dashCooldown = 1;

    private float lastTimeDashed;

    public Vector2 moveInput { get; private set; }

    protected override void Awake() {
        base.Awake();

        instance = this;
        input = new PlayerInputSet();
        stats = GetComponent<PlayerStats>();
        health = GetComponent<PlayerHealth>();
        combat = GetComponent<PlayerCombat>();
        vfx = GetComponent<PlayerVFX>();
        sfx = GetComponent<PlayerSFX>();
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
        heavyAttackState = new PlayerHeavyAttackState(this, stateMachine, "HeavyAttack");

        lastTimeDashed = float.NegativeInfinity;
    }

    protected override void Start() {
        stateMachine.Initialize(idleState);
    }

    public void UpdateDashTimer() {
        lastTimeDashed = Time.time;
    }

    public bool InDashCooldown() {
        return Time.time < lastTimeDashed + dashCooldown;
    }

    public override void EntityDeath() {
        base.EntityDeath();

        if (OnPlayerDeath != null) {
            OnPlayerDeath.Invoke();
        }

        stateMachine.ChangeState(deadState);
    }

    public void TeleportPlayer(Vector3 position)
    {
        transform.position = position;
    }

    public void SetAttackSpeed(float attackAnimSpeedMultiplier) {
        this.attackAnimSpeedMultiplier = attackAnimSpeedMultiplier;
    }

    public void ResetAttackSpeed() {
        attackAnimSpeedMultiplier = 1;
    }

    public void SpeedUp(float speedUpPercentage) {
        tc.SetLocalTimeScale(speedUpPercentage);
        anim.speed = speedUpPercentage;
    }

    public void RecoverSpeed() {
        tc.ResetLocalTimeScale();
        anim.speed = 1;
    }

    public void SetHeavyAttack(float duration, int numOfHeavyAttack) {
        if (setHeavyAttackCo != null) {
            StopCoroutine(setHeavyAttackCo);
        }

        setHeavyAttackCo = StartCoroutine(SetHeavyAttackCo(duration, numOfHeavyAttack));
    }

    private IEnumerator SetHeavyAttackCo(float duration, int numOfHeavyAttack) {
        isHeavyAttack = true;
        this.numOfHeavyAttack = numOfHeavyAttack;
        yield return new WaitForSeconds(duration);
        isHeavyAttack = false;
        this.numOfHeavyAttack = 0;
    }

    public void UpdateHeavyAttack() {
        if (numOfHeavyAttack > 1) {
            numOfHeavyAttack -= 1;
        } else {
            isHeavyAttack = false;
        }
    }

    public void LockAttack() {
        canAttack = false;
    }

    public void UnLockAttack() {
        canAttack = true;
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
