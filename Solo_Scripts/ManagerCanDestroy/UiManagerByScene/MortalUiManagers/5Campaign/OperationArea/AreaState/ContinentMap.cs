using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentMap: IAreaState
{
    public float CameraYPos { get; } = 180f;

    public void EnterState()
    {
        Debug.Log("enter ContinentMap");
        OperationAreaUiManager.Instance.WorldContinentButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.ContinentAreaButtonsGameObject.SetActive(true);
        OperationAreaUiManager.Instance.OperationAreaButtonsGameObject.SetActive(false);
    }
    public void ExitState()
    {
        Debug.Log("exit ContinentMap");
    }
}
