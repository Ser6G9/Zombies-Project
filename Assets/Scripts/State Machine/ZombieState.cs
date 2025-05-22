using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieState
{
    public virtual void Enter(EnemyManager enemy) {}
    public virtual void Exit() {}
    public virtual void Update() {}
}
