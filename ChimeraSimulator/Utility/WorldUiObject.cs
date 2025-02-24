using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldUiObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private WorldUiObject worldUiObject;

    private void Awake()
    {
        worldUiObject = this;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("click" + gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("enter" + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("exit" + gameObject.name);
    }
}
