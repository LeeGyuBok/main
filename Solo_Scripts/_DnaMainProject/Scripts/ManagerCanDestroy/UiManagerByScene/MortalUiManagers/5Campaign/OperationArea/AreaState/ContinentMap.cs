using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentMap: IAreaState
{
    public float CameraYPos { get; } = 180f;
    public float CameraMoveSpeed { get; } = 70f;

    public void EnterState()
    {
        Debug.Log("enter ContinentMap");
        //대륙맵에서 노출되는 작전지역버튼들만 켜기
        OperationAreaUiManager.Instance.WorldContinentButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.ContinentAreaButtonsGameObject.SetActive(true);
        OperationAreaUiManager.Instance.OperationAreaButtonsGameObject.SetActive(false);
    }
    public void ExitState()
    {
        Debug.Log("exit ContinentMap");
    }
}
