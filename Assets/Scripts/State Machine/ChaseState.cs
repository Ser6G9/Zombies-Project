using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : ZombieState
{
    private EnemyManager enemy;
    private float lostPlayerTargetTime = 2f;
    private float timer;

    public override void Enter(EnemyManager enemy)
    {
        this.enemy = enemy;
        enemy.enemyAnimator.SetBool("isRunning", true);
        enemy.GetComponent<NavMeshAgent>().isStopped = false;
    }

    public override void Update()
    {
        if (enemy.player == null)
        {
            return;
        }

        if (enemy.JugadorALaVista() == false)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }

        // Asigna el target como destino objetivo:
        enemy.GetComponent<NavMeshAgent>().destination = enemy.player.transform.position;
        enemy.GetComponent<NavMeshAgent>().speed = enemy.chaseSpeed;

        float distance = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        /*if (distance < enemy.attackRange)
        {
            enemy.stateMachine.ChangeState(new AttackState());
        }*/

        // Si el player se aleja de su rango o si el zombie no lo ve por un tiempo, regresa a IdleState.
        if (distance > enemy.chaseDistance || (enemy.JugadorALaVista() == false && timer >= lostPlayerTargetTime))
        {
            enemy.stateMachine.ChangeState(new IdleState());
            timer = 0f;
        }
    }

    public override void Exit()
    {
        enemy.enemyAnimator.SetBool("isRunning", false);
        enemy.GetComponent<NavMeshAgent>().isStopped = true;
    }
}
