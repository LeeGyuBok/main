using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private EnemyOperator friendlyOperator;

    public DeadState(EnemyOperator character)
    {
        friendlyOperator = character;
    }
    
    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}
