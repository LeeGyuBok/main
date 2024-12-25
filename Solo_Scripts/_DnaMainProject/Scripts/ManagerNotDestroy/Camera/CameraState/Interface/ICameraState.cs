using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraState
{
    public float cameraSpeed { get;}
    public void EnterState();
    public void ExitState();
}
