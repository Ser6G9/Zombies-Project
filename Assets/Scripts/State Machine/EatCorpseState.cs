using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EatCorpseState : ZombieState
{
    private EnemyManager enemy;
    private float eatTime = 5f;
    private float timer = 0f;
    private bool isEating = false;
    private float eatRange = 2f;

    public override void Enter(EnemyManager enemy)
    {
        this.enemy = enemy;
        timer = 0f;
        isEating = false;

        // Asegura que tenga un objetivo cadáver
        if (enemy.corpseTarget != null)
        {
            enemy.GetComponent<NavMeshAgent>().isStopped = false;
            enemy.GetComponent<NavMeshAgent>().SetDestination(enemy.corpseTarget.transform.position);
            enemy.enemyAnimator.SetBool("isGoingToEat", true);
        }
    }

    public override void Update()
    {
        if (enemy.corpseTarget != null)
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.corpseTarget.transform.position);
            if (distance <= eatRange)
            {
                isEating = true;
            }

            if (isEating)
            {
                enemy.enemyAnimator.SetBool("isGoingToEat", false);
                enemy.enemyAnimator.SetBool("isEating", true);
                
                enemy.GetComponent<NavMeshAgent>().isStopped = true;
                
                // Empieza a comer y se cura la vida al máximo cuando acabe.
                timer += Time.deltaTime;
                
                if (timer >= eatTime)
                {
                    //enemy.enemyAnimator.SetBool("isEating", false);
                    enemy.health = enemy.maxHealth;
                    enemy.healthBar.value = enemy.health;
                    enemy.enemyAnimator.SetBool("isEating", false);
                    enemy.stateMachine.ChangeState(new IdleState());
                }
            }
        }
        else
        {
            enemy.stateMachine.ChangeState(new IdleState());
        }
        
    }

    public override void Exit()
    {
        enemy.corpseTarget = null;
        enemy.enemyAnimator.SetBool("isGoingToEat", false);
        enemy.enemyAnimator.SetBool("isEating", false);
    }
}
