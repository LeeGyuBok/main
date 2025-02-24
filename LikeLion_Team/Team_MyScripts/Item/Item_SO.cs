using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SO
{
    public static int order { get; private set; } = 0;
    public Sprite Icon;
    //영어이름, 한국어이름, 아이템코드, 최대소지개수, 드랍개수, 설명 - 공톧데이터
    public ItemData_SO data { get; private set; }
    public int InitialCode { get; private set; } = order;

    public Item_SO(ItemData_SO data_SO)
    {
        //이 데이터 so만 달라지면 다 다른아이템이에요
        data = data_SO;
        Icon = Resources.Load<Sprite>($"Icons/{data_SO.ItemName}");
        InitialCode = order;
        order++;
    }

    public void ShowInfo()
    {
        Debug.Log(data.ItemCode);
        Debug.Log(data.ItemName);
        Debug.Log(data.KoreanName);
        Debug.Log(data.MaxQuantity);
        Debug.Log(data.DropQuantity);
        Debug.Log(Icon.name);
        Debug.Log("cccc");
    }
    
}
