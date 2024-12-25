using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationAreaDetail : IAreaState
{
    public float CameraYPos { get; } = 25f;
    public float CameraMoveSpeed { get; } = 20f;

    //private GameObject selectedMapPrefab;
    public void EnterState()
    {
        //모든 버튼 끄기
        OperationAreaUiManager.Instance.WorldContinentButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.ContinentAreaButtonsGameObject.SetActive(false);
        OperationAreaUiManager.Instance.OperationAreaButtonsGameObject.SetActive(false);
        
        //맵켜기
        OperationAreaObjectManager.Instance.InstantiateOperationArea();
    }

    public void ExitState()
    {
        OperationAreaObjectManager.Instance.DestroySelectedOperationArea();
    }
}
