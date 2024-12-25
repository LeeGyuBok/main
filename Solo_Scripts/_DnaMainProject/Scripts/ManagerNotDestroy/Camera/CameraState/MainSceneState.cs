using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneState : ICameraState
{
    public float cameraSpeed { get; } = 5f;

    public void EnterState()
    {

    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }
}
