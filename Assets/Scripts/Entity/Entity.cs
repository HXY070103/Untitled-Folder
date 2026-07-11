using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour {
    public event Action OnFlipped;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public TimeController tc {get; private set; }
    protected StateMachine stateMachine;

    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;

    private Coroutine knockbackCo;
    private bool isKnocked;

    [Header("Collision detection info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsWall;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    
    public bool groundDetected { get; private set; } 
    public bool wallDetected { get; private set; }

    protected virtual void Awake() {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        tc = new TimeController();
        stateMachine = new StateMachine();
    }
 
    protected virtual void Start() {
        tc.Initialize();
    }

    protected virtual void Update() {
        HandleCollisionDetection();
        tc.Tick();
        stateMachine.UpdateActiveState();
    }

    public virtual void EntityDeath() {}

    public void ReceiveKnockback(Vector2 knockback, float duration) {
        if (knockbackCo != null) {
            StopCoroutine(knockbackCo);
        }

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float duration) {
        isKnocked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        isKnocked = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void Move(float xVelocity, float yVelocity) {
        MoveRaw(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void MoveRaw(float xVelocity, float yVelocity) {
        if (isKnocked) {
            return;
        }

        rb.linearVelocity = new Vector2(xVelocity, yVelocity) * tc.localTimeScale;
    }

    public void HandleFlip(float xVelocity) {
        if (xVelocity > 0 && !facingRight || xVelocity < 0 && facingRight) {
            Flip();
        }
    }

    public void Flip() {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = -facingDir;

        if (OnFlipped != null) {
            OnFlipped.Invoke();
        }
    }

    public void CurrentStateAnimationTrigger() {
        stateMachine.currentState.AnimationTrigger();
    }

    private void HandleCollisionDetection() {
        groundDetected = 
            Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        wallDetected = 
            Physics2D.Raycast(
                primaryWallCheck.position, Vector2.right, facingDir * wallCheckDistance, whatIsWall)
            && Physics2D.Raycast(
                secondaryWallCheck.position, Vector2.right, facingDir * wallCheckDistance, whatIsWall);
    }

    protected virtual void OnDrawGizmos() {
        Gizmos.DrawLine(
                groundCheck.position, 
                groundCheck.position + new Vector3(0, -groundCheckDistance)
            );

        Gizmos.DrawLine(
                primaryWallCheck.position, 
                primaryWallCheck.position + new Vector3(facingDir * wallCheckDistance, 0)
            );
        
        Gizmos.DrawLine(
                secondaryWallCheck.position, 
                secondaryWallCheck.position + new Vector3(facingDir * wallCheckDistance, 0)
            );
    }
}
