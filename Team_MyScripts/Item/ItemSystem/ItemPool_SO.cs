using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumItemCode
{
    //열거형 본격적으로 처음쓰니까 어떻게 쓰는지 알아보자
    /* 열거형값을 정수로
     * EnumItemCode myItem = EnumItemCode.Alcohol;
     * 열거타입 변수명 = 열거타입.열거값
     * int itemValue = (int)myItem;
     * 정수타입 변수명 = (정수타입 형변환)열거타입의 변수명
     *
     * 정수형값을 열거형값으로
     * int itemValue = 4;
     * 정수타입 변수명
     * EnumItemCode myItem = (EnumItemCode)itemValue;
     * 열거타입 변수명 = (열거타입 형변환)정수타입의 변수명
     *
     * 이 Enum을 기준으로, Blank는 0, Blocked는 1이다.
     *
     * Blank = 숫자 로 지정도할 수 있다.
     */ 
    Blank,//0
    LastBlank,
    Alloy,//2
    ElectronicCircuit,
    Glass,//4
    Glue,
    Herb,//6
    Leather,
    Nail,//8
    Plastic,
    Rubber,//10
    Scrap,
    Screw,//12
    UnknownPart,
    Wood,//14
    EnergyDrink,
    HealthRestoreSyringe,//16
    InfectionRestoreSyringe,
    Liminex,//18,
    AssaultRifle,
    Axe,//20
    Bat,
    Knife,//22
    ShotGun
    
    
}

public enum EnumItemCategory
{
    Blank,
    Equipment,
    ConsumableItem,//2
    CommonItem,
    KeyItem,//4
    BluePrint
}

public enum EnumItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public class ItemPool_SO : MonoBehaviour
{
    public static ItemPool_SO Instance { get; private set; }
    
    
    [SerializeField] private List<ItemData_SO> itemDatabase;
    [SerializeField] private List<ConsumableStatus_SO> consumableStatusDatabase;
    
    public List<ItemData_SO> ItemDataBase => itemDatabase;
    public Queue<Item_SO>[] ItemPool { get; private set; }
    public Dictionary<int, Queue<Item_SO>> ItemPool_Dict { get; private set; }
    private Dictionary<int, EnumItemCode> IntCodeToItem { get; set; }
    
     private void Awake()
     {
         if (Instance == null)
         {
             Instance = this;
             DontDestroyOnLoad(gameObject);

             //아이템큐로 이루어진 배열.
             ItemPool = new Queue<Item_SO>[itemDatabase.Count];
             IntCodeToItem = new Dictionary<int, EnumItemCode>();
             ItemPool_Dict = new Dictionary<int, Queue<Item_SO>>();
             for (int i = 0; i < itemDatabase.Count; i++)
             {
                 //이러면 이넘코드에 맞게 딕셔너리 생성
                 ItemPool[i] = new Queue<Item_SO>();
                 IntCodeToItem[i] = (EnumItemCode)i;
                 ItemPool_Dict[i] = ItemPool[i];
                 /*//성공쓰, 디버그용
                  EnumItemCode myItem = (EnumItemCode)i;
                 Debug.Log(myItem.ToString());*/
                 
             }
         }
         else
         {
             Destroy(gameObject);
         }
     }

    //data.ItemID를 인수로 받아 아이템을 드랍한다.
    //item을 내준다.
    public Item_SO DropItem(int itemID)
    {
        Item_SO item;
        //큐가 비었어요
        if (ItemPool_Dict[itemID].Count <= 0)
        {
            //아이템을 새로만들어요
            if (itemID == (int)EnumItemCode.Liminex)
            {
                item = new Liminex(itemDatabase[itemID]) as Item_SO;
            }
            else if (itemID is <= 17 and >= 15)
            {
                item = new EquipItem_Consumable(itemDatabase[itemID], consumableStatusDatabase[itemID-15]) as Item_SO;
            }
            else
            {
                item = new Item_SO(itemDatabase[itemID]);    
            }
            //뱉어요
            return item;    
        }
        //아이템풀에서 꺼내요
        item = ItemPool_Dict[itemID].Dequeue();
        //뱉어요
        return item;

        /*//폐기코드
         foreach (ItemData_SO data in itemDatabase)
        {
            if (data.NpcCode.Equals(itemID.ToString()))
            {
                dropItemData = data;
                break;
            }
        }
        Item_SO item = new Item_SO(dropItemData);
        dropItemData = null;
        return item;*/
    }

    public void GoPool_Item(Item_SO item)
    {
        //디버깅용 코드
        if (item is null)
        {
            Debug.Log(null);
        }
        else
        {
            Debug.Log(item.data.ItemName);
        }
        
        if (int.TryParse(item.data.ItemCode, out int itemId))
        {
            ItemPool_Dict[itemId].Enqueue(item);
            //Debug.Log($"{item.data.ItemName} go to pool");
        }
        else
        {
            //Debug.Log("Critical Error: Incorrect ItemCode");
        }
    }
}
