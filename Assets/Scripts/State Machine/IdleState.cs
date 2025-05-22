using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : ZombieState
{
    private EnemyManager enemy;
    public float waitToWanderTime = 3f;
    public float timer;

    public override void Enter(EnemyManager enemy)
    {
        this.enemy = enemy;
        enemy.enemyAnimator.SetBool("isRunning", false);
        enemy.enemyAnimator.SetBool("isWalking", false);
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        
        if (enemy.player != null && enemy.JugadorALaVista())
        {
            enemy.stateMachine.ChangeState(new ChaseState());
        } 
        else if (enemy.player != null && timer >= waitToWanderTime)
        {
            enemy.stateMachine.ChangeState(new WanderState());
        }
    }

    public override void Exit()
    {
        timer = 0;
    }
}
