using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieState : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        ATTACK,
        CHASE
    };

    public enum EVENT
    {
        AnyState,
        Dead,
        FakeDead,
        Deambular,
        EatCorpse
    };

    public STATE name;
    protected EVENT state;

    public ZombieState()
    {
        state = EVENT.AnyState;
    }

    public virtual void Enter(){}

}
