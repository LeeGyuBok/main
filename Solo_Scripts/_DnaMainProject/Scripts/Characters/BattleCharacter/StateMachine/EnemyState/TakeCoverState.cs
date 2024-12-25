using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeCoverState : IState
{
    private EnemyOperator friendlyOperator;

    public TakeCoverState(EnemyOperator character)
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
