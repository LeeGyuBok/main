using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprint", fileName = "Blueprint_ItemName")]
public class BlueprintData_SO : ScriptableObject
{
    [SerializeField] private int code;
    public int Code
    {
        get
        {
            return code;
        }
    }

    [SerializeField] private string itemName;
    public string ItemName
    {
        get
        {
            return itemName;
        }
    }

    [SerializeField] private Sprite bpImage;
    public Sprite BpImage
    {
        get
        {
            return bpImage;
        }
    }

    [SerializeField] private List<ItemData_SO> parts;
    public List<ItemData_SO> Parts
    {
        get { return parts; }
    }

    [SerializeField] private List<int> partsCount;
    public List<int> PartsCount
    {
        get
        {
            return partsCount;
        }
    }
    
    [SerializeField] private int cost;
    public int Cost
    {
        get
        {
            return cost;
        }
    }
}
