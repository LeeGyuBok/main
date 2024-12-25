using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignSceneState : ICameraState
{
    public float cameraSpeed { get; } = 100f;

    public void EnterState()
    {
        
    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }
}
