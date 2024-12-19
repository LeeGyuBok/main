using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationAreaMap: IAreaState
{
    public float CameraYPos { get; } = 40f;
    public float CameraMoveSpeed { get; } = 50f;

    public void EnterState()
    {
        Debug.Log("open operationArea Panel");
        //작전지역상태에서 노출되는 작전지역세부사항버튼만 켜기
        OperationAreaUiManager.Instance.WorldContinentButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.ContinentAreaButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.OperationAreaButtonsGameObject.SetActive(true);
    }
    public void ExitState()
    {
        Debug.Log("exit OperationAreaMap");
    }
}
