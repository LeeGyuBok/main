using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager_SO : MonoBehaviour
{
    private static ItemManager_SO instance;
    public static ItemManager_SO Instance
    {
        get
        {
            return instance;
        }
    }

    public static ItemPool_SO Items { get; private set;}

    private void Awake()
    {
        if (instance == null)
        {
            instance = FindFirstObjectByType<ItemManager_SO>();
            if (instance == null)
            {
                GameObject singletonObject = new GameObject("ItemManager_SO");
                instance = singletonObject.AddComponent<ItemManager_SO>();
            }
            DontDestroyOnLoad(instance);
        }
    }

    private void Start()
    {
        //상호작용가능 아이템오브젝트들의 개수
        /*int interactionItemObjectCount = FindObjectsOfType<ItemObject_Interaction>().Length;
        Debug.Log(interactionItemObjectCount);*/
    }

    public Item_SO GetItem(int itemCode)
    {
        return ItemPool_SO.Instance.DropItem(itemCode);
    }

    public void PickUpItem(Item_SO item)
    {
        //단순히 포문 돌리는것보다 딕셔너리화해서 찾는게 나을지도 또는 이진탐색? - 답은 딕셔너리다
        ItemPool_SO.Instance.GoPool_Item(item);
    }
    
    
}
