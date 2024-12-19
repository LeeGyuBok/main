using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetPointer : MonoBehaviour, IMapLocationPointer
{
    private void Awake()
    {
        SceneImmortalManager.Instance.RegisterObserver(this);
    }
    public void DataRenew(string data)
    {
        currentScene = data;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public GameObject myPanel { get; }
    public string currentScene { get; private set; }
}
