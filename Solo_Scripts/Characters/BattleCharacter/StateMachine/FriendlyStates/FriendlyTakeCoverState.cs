using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyTakeCoverState : IState
{
    private FriendlyOperator friendlyOperator;

    public FriendlyTakeCoverState(FriendlyOperator character)
    {
        friendlyOperator = character;
    }
    
    public void Enter()
    {
        //
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
}
