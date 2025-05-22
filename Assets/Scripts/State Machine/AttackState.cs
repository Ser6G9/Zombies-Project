using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : ZombieState
{
    private EnemyManager enemy;

    public override void Enter(EnemyManager enemy)
    {
        this.enemy = enemy;
        enemy.GetComponent<NavMeshAgent>().isStopped = true; // El Zombie se detiene
    }

    public override void Update()
    {
        if (enemy.player == null)
        {
            return;
        }
        // El enemigo se mueve mientras ataca, así irá más lento para dar chance a esquivarlo
        enemy.GetComponent<NavMeshAgent>().speed = enemy.walkSpeed;
        
        // Si el jugador ya no está al alcance, vuelve a perseguirlo
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        if (distance > enemy.attackRange)
        {
            enemy.stateMachine.ChangeState(new ChaseState());
            return;
        }

        enemy.attackDelayTimer += Time.deltaTime;

        if (enemy.attackDelayTimer >= enemy.delayBetweenAttacks - enemy.howMuchEarlierStartAttackAnimation && enemy.attackDelayTimer <= enemy.delayBetweenAttacks)
        {
            enemy.enemyAnimator.SetTrigger("isAttacking");
        }

        if (enemy.attackDelayTimer >= enemy.delayBetweenAttacks)
        {
            enemy.player.GetComponent<PlayerManager>().Hit(enemy.damage);
            enemy.attackDelayTimer = 0f;
        }
    }

    public override void Exit()
    {
        enemy.GetComponent<NavMeshAgent>().isStopped = false; // Reanuda movimiento al salir del estado
    }
}
