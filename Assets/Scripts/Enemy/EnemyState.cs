using UnityEngine;

public class EnemyState : EntityState {
    protected Enemy enemy;

    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        this.enemy = enemy;

        anim = enemy.anim;
        rb = enemy.rb;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void UpdateAnimationPatameters() {
        base.UpdateAnimationPatameters();

        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("MoveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);

        float battleAnimSpeedMultiplier = 
            enemy.moveAnimSpeedMultiplier * enemy.battleMoveSpeed / enemy.moveSpeed;
        anim.SetFloat("BattleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
    }
}