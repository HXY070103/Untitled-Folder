using UnityEngine;

public class EntityState {
    protected Entity entity;
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;

    protected float stateTimer;
    protected bool triggerCalled;

    public EntityState(Entity entity, StateMachine stateMachine, string animBoolName) {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter() {
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    public virtual void Update() {
        stateTimer -= Time.deltaTime * entity.LocalTimeScale;
        UpdateAnimationPatameters();
    }

    public virtual void Exit() {
        anim.SetBool(animBoolName, false);
    }
 
    public void AnimationTrigger() {
        triggerCalled = true;
    }

    public virtual void UpdateAnimationPatameters() {}
}
