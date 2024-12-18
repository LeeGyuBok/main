using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationAreaMap: IAreaState
{
    public float CameraYPos { get; } = 40f;

    public void EnterState()
    {
        Debug.Log("open operationArea Panel");
        OperationAreaUiManager.Instance.WorldContinentButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.ContinentAreaButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.OperationAreaButtonsGameObject.SetActive(true);
    }
    public void ExitState()
    {
        Debug.Log("exit OperationAreaMap");
    }
}
