using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignStageState : ICameraState
{
    public float cameraSpeed { get; } = 20f;

    public void EnterState()
    {

    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }
}
