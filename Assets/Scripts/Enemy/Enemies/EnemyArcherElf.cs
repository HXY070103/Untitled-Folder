using System;
using UnityEngine;

public class EnemyArcherElf : Enemy {
    [Header("Archer elf specific info")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowStartPoint;
    [SerializeField] private float arrowSpeed = 5;

    protected override void Awake() {
        base.Awake();

        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        battleState = new EnemyArcherElfBattleState(this, stateMachine, "Battle");
        preAttackState = new EnemyPreAttackState(this, stateMachine, "Battle");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");
        deadState = new EnemyDeadState(this, stateMachine, "Idle");
        stunnedState = new EnemyStunnedState(this, stateMachine, "Stunned");

        anim.SetBool("HasStunnedRecovery", false);
    }

    protected override void Start() {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void SpecialAttack() {
        base.SpecialAttack();
        
        Shoot();
    }

    private void Shoot() {
        GameObject arrow = 
            Instantiate(arrowPrefab, arrowStartPoint.position, Quaternion.identity);
        arrow.GetComponent<EnemyArcherElfArrow>().SetUpArrow(facingDir * arrowSpeed, combat);
    }
}
