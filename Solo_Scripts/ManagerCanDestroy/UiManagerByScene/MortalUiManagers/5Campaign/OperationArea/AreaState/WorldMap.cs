using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap: IAreaState
{
    public float CameraYPos { get; } = 320f;
    
    public void EnterState()
    {
        Debug.Log("enter WorldMap");
        OperationAreaUiManager.Instance.WorldContinentButtonsGameObject.SetActive(true);
        OperationAreaUiManager.Instance.ContinentAreaButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.OperationAreaButtonsGameObject.SetActive(false);
    }
    public void ExitState()
    {
        
    }
}
