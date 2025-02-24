using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Item/ItemData_SO", order = 2, fileName = "Item_ItemName")]
public class ItemData_SO : ScriptableObject
{
    [SerializeField] private string itemName;
    public string ItemName
    {
        get { return itemName; }
    }

    [SerializeField] private string koreanName;
    public string KoreanName
    {
        get { return koreanName; }
    }

    [SerializeField] private string itemCode;
    public string ItemCode
    {
        get { return itemCode; }
    }
    
    [SerializeField] private int itemCategory;
    public int ItemCategory
    {
        get { return itemCategory; }
    }

    [SerializeField] private int maxQuantity;
    public int MaxQuantity
    {
        get { return maxQuantity; }
    }

    [SerializeField] private int dropQuantity;
    public int DropQuantity
    {
        get { return dropQuantity; }
    }
    
    [SerializeField] private string koreanDetail;
    public string KoreanDetail
    {
        get { return koreanDetail; }
    }
}
