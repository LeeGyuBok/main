using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

[RequireComponent(typeof(SphereCollider))]
//ItemObject_Interaction is a Interaction. 아이템 오브젝트 상호작용은 상호작용이다. <- is a 관계
public class ItemObject_Interaction : Interaction
{
    [SerializeField] private ItemData_SO dataSo;

    private SphereCollider contactCollider;
    
    // Start is called before the first frame update

    private void Awake()
    {
        
        contactCollider = GetComponent<SphereCollider>();
        contactCollider.isTrigger = true;
        contactCollider.radius = 1f;
    }

    private void Start()
    {
        AssignInteractionButton();
    }

    protected override void DoInteraction()
    {
        //아이템코드에 따른 아이템 드랍
        
        /*if (gameObject.TryGetComponent(out IitemcodeNumbering possibleInteraction))
        {
            //일단 아이템드랍 갯수를 데이터에서 확인
            //Debug.Log(ItemManager_SO.Instance.Items.ItemDataBase);//데이터베이스가 안되있다.
            int loop = ItemManager_SO.Instance.Items.ItemDataBase[possibleInteraction.ItemCode].DropQuantity;
            Debug.Log(loop);
            
            //드랍갯수만큼 아이템만들어서 넣기
            for (int i = 0; i < loop; i++)
            {
                Item_SO item = ItemManager_SO.Instance.GetItem(possibleInteraction.ItemCode);
                InsertItemToInventory(item);    
            }
        }*/
        
        int loopNew = dataSo.DropQuantity;
        Debug.Log(loopNew);
            
        //드랍갯수만큼 아이템만들어서 넣기
        for (int i = 0; i < loopNew; i++)
        {
            if (int.TryParse(dataSo.ItemCode, out int itemCode))
            {
                Item_SO item = ItemManager_SO.Instance.GetItem(itemCode);
                InsertItemToInventory(item);
            }
            else
            {
                Debug.Log("Critical Error: Incorrect ItemCode");
            }
        }
    }

    //바닥에 떨어지게
    private GameObject DropItem(Item_SO item)
    {
        //여기는 프리팹이 정해지면... 가능할지도
        //GameObject drop = Instantiate(new GameObject(item.data.ItemName), gameObject.transform.position, Quaternion.identity);
        GameObject drop = new GameObject(item.data.ItemName);
        drop.transform.position = gameObject.transform.position;
        drop.transform.rotation = Quaternion.identity;
        return drop;
    }
    
    //인벤토리에 들어오게?
    private void InsertItemToInventory(Item_SO item)
    {
        //아이템을 인벤토리에 넣어주는 로직 아직 인벤토리가 없어욧
        Inventory.Instance.PublicItemGotoInventory(item);
    }
}
