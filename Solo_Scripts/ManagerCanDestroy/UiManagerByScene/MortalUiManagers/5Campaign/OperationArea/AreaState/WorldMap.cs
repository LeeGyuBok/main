using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap: IAreaState
{
    public float CameraYPos { get; } = 320f;
    public float CameraMoveSpeed { get; } = 100f;
    
    public void EnterState()
    {
        Debug.Log("enter WorldMap");
        //월드맵에서 노출되는 대륙버튼들만 켜기
        OperationAreaUiManager.Instance.WorldContinentButtonsGameObject.SetActive(true);
        OperationAreaUiManager.Instance.ContinentAreaButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.OperationAreaButtonsGameObject.SetActive(false);
    }
    public void ExitState()
    {

    }
}
