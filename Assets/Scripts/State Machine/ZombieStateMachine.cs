using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStateMachine : MonoBehaviour
{
    private ZombieState currentState;
    private EnemyManager enemy;
    
    public ZombieStateMachine(EnemyManager enemy)
    {
        this.enemy = enemy;
    }

    public void ChangeState(ZombieState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(enemy);
    }

    public void Update()
    {
        currentState?.Update();
    }


    

    /*
    void Update()
    {
        currentState?.Update();
        HandleAnyStateTransitions();
    }

    private void HandleAnyStateTransitions()
    {
        // Example of global transitions
        if (zombieHP <= Random.Range(0, 25))
        {
            ChangeState(new FakeDeadState(this, gameObject));
        }
    }

    public Transform GetPlayer() => player;*/
}
