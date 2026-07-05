using UnityEngine;

public class EnemyDeadState : EnemyState {
    private Collider2D col => enemy.GetComponent<Collider2D>();
    private EnemySkillEffectReceiver receiver => enemy.GetComponentInChildren<EnemySkillEffectReceiver>();

    public EnemyDeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
    }
    
    public override void Enter() {
        base.Enter();

        anim.enabled = false;
        col.enabled = false;

        rb.gravityScale = 12; 
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);

        stateMachine.SwitchOff();
        tc.SwitchOff();
        //receiver.SwitchOff();
        enemy.StopAllCoroutines();
        
        enemy.DestoryGameObjectWithDelay(10);
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }
}
