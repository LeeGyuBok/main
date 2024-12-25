using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyDeadState : IState
{
    private FriendlyOperator friendlyOperator;

    public FriendlyDeadState(FriendlyOperator character)
    {
        friendlyOperator = character;
    }
    
    public void Enter()
    {
        
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
}
