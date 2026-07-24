using System.Diagnostics;
using UnityEngine;

public class EnemyNecromancerBattleState : EnemyBattleState {
    private EnemyNecromancer enemyNecromancer;

    private int attackIndex = 1;
    private float spellCastIndex = 1;

    private Transform currentPosition;
    private float lastTimeSwitchPosition = float.NegativeInfinity;
    private bool isMoveToPosition = false;

    public EnemyNecromancerBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName) {
        enemyNecromancer = enemy as EnemyNecromancer;
    }

    public override void Enter() {
        base.Enter();

        enemyNecromancer.HandleFlip(DirectionToPlayer());

        if (CanSwitchPosition()) {
            SwitchPosition();
            UpdateSwitchPositionTimer();

            if (enemyNecromancer.isPhaseTwo) {
                enemyNecromancer.StartSummon();
            }
        }
    }

    public override void Update() {
        base.Update();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void HandleBattle() {
        if (isMoveToPosition) {
            MoveToPosition();
            return;
        }

        if (OutOfMoveRange()) {
            isMoveToPosition = true;
        } 

        if (WithInAttackRange() && enemyNecromancer.PlayerDetected() && CanAttack()) {
            HandleAttackLoop();
            UpdateAttackTimer();
            UpdateAttackCounter();
        } else {
            if (DirectionToPlayer() != enemyNecromancer.facingDir) {
                enemyNecromancer.HandleFlip(DirectionToPlayer());
            }

            if (WithInAttackRange() || enemyNecromancer.wallDetected || WillOutOfMoveRange()) {
                enemyNecromancer.MoveRaw(0, 0);
                return;
            }

            float xVelocity = DirectionToPlayer() * enemyNecromancer.battleMoveSpeed;
            enemy.Move(xVelocity, rb.linearVelocity.y);
            enemy.sfx?.TryPlayMove(0.2f);
        }
    }

    private void MoveToPosition() {
        Vector2 diff = currentPosition.position - enemyNecromancer.transform.position;

        if (diff.magnitude < 0.1f) {
            enemyNecromancer.MoveRaw(0, 0);
            isMoveToPosition = false;
            return;
        }

        Vector2 velocity = diff.normalized * enemyNecromancer.battleMoveSpeed;
        enemyNecromancer.Move(velocity.x, velocity.y);
    }

    protected override bool WithInAttackRange() {
        Vector2 diff = player.position - enemyNecromancer.transform.position;
        return diff.magnitude < enemyNecromancer.attackDistance;
    }

    private void HandleAttackLoop() {
        switch (attackIndex) {
            case 1 or 2: 
                ApplyBasicAttack();
                break;
            case 3: 
                HandleSpellCastLoop();
                UpdateSpellCastCounter();
                break;
        }
    }

    private bool OutOfMoveRange() {
        float xDiff = Mathf.Abs(enemyNecromancer.transform.position.x - currentPosition.position.x);
        float yDiff = Mathf.Abs(enemyNecromancer.transform.position.y - currentPosition.position.y);

        return xDiff > enemyNecromancer.xOffset || yDiff > enemyNecromancer.yOffset;
    }

    private bool WillOutOfMoveRange(){
        float xDiff = Mathf.Abs(enemyNecromancer.transform.position.x - currentPosition.position.x);
        return xDiff > enemyNecromancer.xOffset - 0.2f;
    }

    private void UpdateSwitchPositionTimer() {
        lastTimeSwitchPosition = tc.time;
    }

    private bool CanSwitchPosition() {
        return tc.time > lastTimeSwitchPosition + enemyNecromancer.switchPositionCooldown;
    }

    private void SwitchPosition() {
        Transform newPosition = 
            enemyNecromancer.positions[Random.Range(0, enemyNecromancer.positions.Count)];
        currentPosition = newPosition;
        isMoveToPosition = true;
    }

    private void HandleSpellCastLoop() {
        switch (spellCastIndex) {
            case 1 or 2: 
                enemyNecromancer.SetIsTraking(false);
                break; 
            case 3: 
                enemyNecromancer.SetIsTraking(true);
                break;
        }

        enemyNecromancer.SetAttack(
                enemyNecromancer.spellCastWindup, 
                enemyNecromancer.spellCastCooldown, 
                enemyNecromancer.spellCastState
            );
        stateMachine.ChangeState(enemyNecromancer.preAttackState);
    }

    private void UpdateSpellCastCounter() {
        spellCastIndex += 1;

        if (spellCastIndex > enemyNecromancer.numOfSpellCast) {
            spellCastIndex = 1;
        }
    }

    private void UpdateAttackCounter() {
        attackIndex += 1;

        if (attackIndex > enemyNecromancer.numOfAttack) {
            attackIndex = 1;
        }
    }
}
