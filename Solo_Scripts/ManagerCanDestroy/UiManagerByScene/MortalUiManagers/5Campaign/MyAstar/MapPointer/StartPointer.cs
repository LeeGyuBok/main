using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartPointer : MonoBehaviour, IMapLocationPointer
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

    public GameObject myPanel { get; private set; }
    public string currentScene { get; private set; }
}
