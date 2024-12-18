using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAreaState
{
    public float CameraYPos { get; }
    public void EnterState();
    public void ExitState();
}
