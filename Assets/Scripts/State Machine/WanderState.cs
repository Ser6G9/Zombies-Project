using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : ZombieState
{
    private EnemyManager enemy;
    private float wanderRadius = 30f;
    private float wanderTime = 5f;
    private float timer;
    private Vector3 destinationPosition;

    public override void Enter(EnemyManager enemy)
    {
        this.enemy = enemy;
        timer = wanderTime;

        enemy.GetComponent<NavMeshAgent>().isStopped = false;

        SetNewDestination();
        enemy.enemyAnimator.SetBool("isWalking", true);
    }

    public override void Update()
    {
        if (enemy.player != null && enemy.JugadorALaVista())
        {
            enemy.stateMachine.ChangeState(new ChaseState());
            return;
        }

        timer += Time.deltaTime;
        if (timer >= wanderTime)
        {
            SetNewDestination();
            timer = 0;
        }

        // Al llegar a su detino, regresa al estado por defecto.
        if (Vector3.Distance(enemy.transform.position, destinationPosition) <= 1.0f)
        {
            enemy.stateMachine.ChangeState(new IdleState());
        }
    }

    public override void Exit()
    {
        enemy.enemyAnimator.SetBool("isWalking", false);
    }

    private void SetNewDestination()
    {
        // Selecciona un destino aletorio dentro de su radio de ruta.
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += enemy.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, 1))
        {
            destinationPosition = hit.position;
            enemy.GetComponent<NavMeshAgent>().SetDestination(destinationPosition);
            enemy.GetComponent<NavMeshAgent>().speed = enemy.walkSpeed;
        }
    }
}
