using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReturnPointer  : MonoBehaviour, IMapLocationPointer
{
    private void Awake()
    {
        SceneImmortalManager.Instance.RegisterObserver(this);
    }
    public void DataRenew(string data)
    {
        currentScene = data;
        if (data.Equals("10OperationAreaUi"))
        {
            //OperationAreaUiManager.Instance.
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public GameObject myPanel { get; }
    public string currentScene { get; private set; }
}
