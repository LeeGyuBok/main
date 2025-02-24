using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    
    //인벤토리

    [SerializeField] private GameObject equipments;
    [SerializeField] private GameObject consumable;
    [SerializeField] private GameObject items;
    [SerializeField] private GameObject keyItems;
    [SerializeField] private GameObject credit;
    public GameObject CreditObject => credit;

    /// <summary>
    /// Order: Equipment, Consumable, Item(Common), Key
    /// </summary>
    private List<List<Button>> buttonListGroup;
    
    private List<Button> inventory_EquipmentsButton;
    private List<Button> inventory_ConsumableButton;
    private List<Button> inventory_ItemsButton;
    private List<Button> inventory_KeyItemButton;
    
    /// <summary>
    /// Order: Equipment, Consumable, Item(Common), Key
    /// </summary>
    public List<List<Stack<Item_SO>>> ItemDataGroup { get; private set; }
    
    private List<Stack<Item_SO>> inventory_EquipmentsItemData;
    private List<Stack<Item_SO>> inventory_ConsumableItemData;
    private List<Stack<Item_SO>> inventory_ItemData;
    private List<Stack<Item_SO>> inventory_KeyItemData;
    
    /// <summary>
    /// Order: Equipment, Consumable, Item(Common), Key
    /// </summary>
    private List<Dictionary<Button, Stack<Item_SO>>> dictGroup;//어따쓸고..?
    
    public Dictionary<Button, Stack<Item_SO>> inventory_Equipments { get; private set; }
    public Dictionary<Button, Stack<Item_SO>> inventory_Consumable{ get; private set; }
    public Dictionary<Button, Stack<Item_SO>> inventory_Items{ get; private set; }
    public Dictionary<Button, Stack<Item_SO>> inventory_KeyItem{ get; private set; }

    
    
    private List<Stack<Item_SO>> targetInventoryItemDatas;
    private List<Button> targetInventoryButtons;
    private Dictionary<Button, Stack<Item_SO>> targetInventory;
    
    
    private Dictionary<Stack<Item_SO>, Button> stackToButtonDict;
    public Dictionary<Button, Dictionary<Button, Stack<Item_SO>>> buttonToButtonDict { get; private set; }
    
    [SerializeField] private GameObject inventorySpace;
    [SerializeField] private GameObject itemSimpleView;
    [SerializeField] private GameObject itemDetailView;
    
    public GameObject ItemSimpleView => itemSimpleView;
    public GameObject ItemDetailView => itemDetailView;

    public int Credit { get; private set; } = 400000;

    private bool initialized = false;
    //인벤토리

    //디테일창
    public bool IsHovered { get; private set; } = false;
    //디테일창
    
    //제작관련
    public GameObject EmptyInventory { get; private set; }
    //제작관련
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //인벤토리
            
            if (equipments.transform.childCount == 0)
            {
                //장비창 조정할 때 인벤토리 스페이스만 바꿔주면됨
                Instantiate(inventorySpace, equipments.transform);
                inventory_EquipmentsItemData = new List<Stack<Item_SO>>();
                inventory_EquipmentsButton = new List<Button>();
                for (int i = 0; i < equipments.transform.GetChild(0).childCount; i++)
                {
                    Button button = equipments.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                    inventory_EquipmentsButton.Add(button);
                    inventory_EquipmentsItemData.Add(new Stack<Item_SO>());
                }
            }
            if (consumable.transform.childCount == 0)
            {
                Instantiate(inventorySpace, consumable.transform);
                inventory_ConsumableItemData = new List<Stack<Item_SO>>();
                inventory_ConsumableButton = new List<Button>();
                for (int i = 0; i < consumable.transform.GetChild(0).childCount; i++)
                {
                    Button button = consumable.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                    inventory_ConsumableButton.Add(button);
                    inventory_ConsumableItemData.Add(new Stack<Item_SO>());
                }
            }
            if (items.transform.childCount == 0)
            {
                Instantiate(inventorySpace, items.transform);
                inventory_ItemData = new List<Stack<Item_SO>>();
                inventory_ItemsButton = new List<Button>();
                for (int i = 0; i < items.transform.GetChild(0).childCount; i++)
                {
                    Button button = items.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                    inventory_ItemsButton.Add(button);
                    inventory_ItemData.Add(new Stack<Item_SO>());
                }
            }
            if (keyItems.transform.childCount == 0)
            {
                Instantiate(inventorySpace, keyItems.transform);
                inventory_KeyItemData = new List<Stack<Item_SO>>();
                inventory_KeyItemButton = new List<Button>();
                for (int i = 0; i < keyItems.transform.GetChild(0).childCount; i++)
                {
                    Button button = keyItems.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                    inventory_KeyItemButton.Add(button);
                    inventory_KeyItemData.Add(new Stack<Item_SO>());
                }
            }
            
            inventory_Equipments = new Dictionary<Button, Stack<Item_SO>>();
            inventory_Consumable = new Dictionary<Button, Stack<Item_SO>>();
            inventory_Items = new Dictionary<Button, Stack<Item_SO>>();
            inventory_KeyItem = new Dictionary<Button, Stack<Item_SO>>();

            targetInventoryItemDatas = new List<Stack<Item_SO>>();
            targetInventoryButtons = new List<Button>();
            targetInventory = new Dictionary<Button, Stack<Item_SO>>();

            buttonToButtonDict = new Dictionary<Button, Dictionary<Button, Stack<Item_SO>>>();

            stackToButtonDict = new Dictionary<Stack<Item_SO>, Button>();
            //인벤토리*/
            UiManager.Inventory = Instance;
        }
        else
        {
            if (equipments.transform.childCount != 0)
            {
                Debug.Log(equipments.transform.childCount);
                for (int i = 0; i < equipments.transform.childCount; i++)
                {
                    DestroyImmediate(equipments.transform.GetChild(0));
                }
                //장비창 조정할 때 인벤토리 스페이스만 바꿔주면됨
                Instantiate(inventorySpace, equipments.transform);
                inventory_EquipmentsItemData = new List<Stack<Item_SO>>();
                inventory_EquipmentsButton = new List<Button>();
                for (int i = 0; i < equipments.transform.GetChild(0).childCount; i++)
                {
                    Button button = equipments.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                    inventory_EquipmentsButton.Add(button);
                    inventory_EquipmentsItemData.Add(new Stack<Item_SO>());
                }
            }
            if (consumable.transform.childCount != 0)
            {
                for (int i = 0; i < consumable.transform.childCount; i++)
                {
                    DestroyImmediate(consumable.transform.GetChild(0));
                }
                Instantiate(inventorySpace, consumable.transform);
                inventory_ConsumableItemData = new List<Stack<Item_SO>>();
                inventory_ConsumableButton = new List<Button>();
                for (int i = 0; i < consumable.transform.GetChild(0).childCount; i++)
                {
                    Button button = consumable.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                    inventory_ConsumableButton.Add(button);
                    inventory_ConsumableItemData.Add(new Stack<Item_SO>());
                }
            }
            if (items.transform.childCount != 0)
            {
                for (int i = 0; i < items.transform.childCount; i++)
                {
                    DestroyImmediate(items.transform.GetChild(0));
                }
                Instantiate(inventorySpace, items.transform);
                inventory_ItemData = new List<Stack<Item_SO>>();
                inventory_ItemsButton = new List<Button>();
                for (int i = 0; i < items.transform.GetChild(0).childCount; i++)
                {
                    Button button = items.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                    inventory_ItemsButton.Add(button);
                    inventory_ItemData.Add(new Stack<Item_SO>());
                }
            }
            if (keyItems.transform.childCount != 0)
            {
                for (int i = 0; i < keyItems.transform.childCount; i++)
                {
                    DestroyImmediate(keyItems.transform.GetChild(0));
                }
                Instantiate(inventorySpace, keyItems.transform);
                inventory_KeyItemData = new List<Stack<Item_SO>>();
                inventory_KeyItemButton = new List<Button>();
                for (int i = 0; i < keyItems.transform.GetChild(0).childCount; i++)
                {
                    Button button = keyItems.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Button>();
                    inventory_KeyItemButton.Add(button);
                    inventory_KeyItemData.Add(new Stack<Item_SO>());
                }
            }
            
            inventory_Equipments = new Dictionary<Button, Stack<Item_SO>>();
            inventory_Consumable = new Dictionary<Button, Stack<Item_SO>>();
            inventory_Items = new Dictionary<Button, Stack<Item_SO>>();
            inventory_KeyItem = new Dictionary<Button, Stack<Item_SO>>();

            targetInventoryItemDatas = new List<Stack<Item_SO>>();
            targetInventoryButtons = new List<Button>();
            targetInventory = new Dictionary<Button, Stack<Item_SO>>();

            buttonToButtonDict = new Dictionary<Button, Dictionary<Button, Stack<Item_SO>>>();

            stackToButtonDict = new Dictionary<Stack<Item_SO>, Button>();
            //인벤토리*/
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!initialized)
        {
             //GameObject inventoryGameObject = UiManager.Instance.uiTabContents[1];
        
            buttonListGroup = new List<List<Button>>
            {
                inventory_EquipmentsButton, inventory_ConsumableButton,
                inventory_ItemsButton, inventory_KeyItemButton
            };
            
            ItemDataGroup = new List<List<Stack<Item_SO>>>
            {
                inventory_EquipmentsItemData, inventory_ConsumableItemData,
                inventory_ItemData, inventory_KeyItemData
            };
            
            foreach (List<Stack<Item_SO>> dataGroup in ItemDataGroup)
            {
                for (int i = 0; i < dataGroup.Count; i++)
                {
                    if (i == 3)
                    {
                        //해당 공간의 마지막 칸을 의미
                        dataGroup[i].Push(ItemManager_SO.Instance.GetItem(1));
                        //Debug.Log(dataGroup[^1/*== dataGroup.Count-1*/].Peek().data.ItemName);    
                    }
                    else
                    {
                        dataGroup[i].Push(ItemManager_SO.Instance.GetItem(0));
                    }
                }
            }

            dictGroup = new List<Dictionary<Button, Stack<Item_SO>>>
            {
                inventory_Equipments, inventory_Consumable, inventory_Items, inventory_KeyItem
            };
            
            for (int i = 0; i < inventory_EquipmentsButton.Count; i++)
            {
                //Debug.Log(inventory_EquipmentsButton[i]);
                //Debug.Log(inventory_EquipmentsItemData[i]);
                int index = i;
                inventory_EquipmentsButton[i].onClick.AddListener(() => Instance.ShowDetail(inventory_EquipmentsButton[index]));
                inventory_EquipmentsButton[i].AddComponent<MouseHover>();
                inventory_Equipments[inventory_EquipmentsButton[i]] = inventory_EquipmentsItemData[i];
                buttonToButtonDict[inventory_EquipmentsButton[i]] = inventory_Equipments;
                UpdateInventory(inventory_EquipmentsButton[i], inventory_Equipments[inventory_EquipmentsButton[i]]);
            }

            for (int i = 0; i < inventory_ConsumableButton.Count; i++)
            {
                int index = i;
                inventory_ConsumableButton[i].onClick.AddListener(() => Instance.ShowDetail(inventory_ConsumableButton[index]));
                inventory_ConsumableButton[i].AddComponent<MouseHover>();
                inventory_Consumable[inventory_ConsumableButton[i]] = inventory_ConsumableItemData[i];
                buttonToButtonDict[inventory_ConsumableButton[i]] = inventory_Consumable;
                UpdateInventory(inventory_ConsumableButton[i],  inventory_Consumable[inventory_ConsumableButton[i]]);
            }

            for (int i = 0; i < inventory_ItemsButton.Count; i++)
            {
                int index = i;
                inventory_ItemsButton[i].onClick.AddListener(() => Instance.ShowDetail(inventory_ItemsButton[index]));
                inventory_ItemsButton[i].AddComponent<MouseHover>();
                inventory_Items[inventory_ItemsButton[i]] = inventory_ItemData[i];
                buttonToButtonDict[inventory_ItemsButton[i]] = inventory_Items;
                UpdateInventory(inventory_ItemsButton[i], inventory_Items[inventory_ItemsButton[i]]);
            }
            
            for (int i = 0; i < inventory_KeyItemButton.Count; i++)
            {
                int index = i;
                inventory_KeyItemButton[i].onClick.AddListener(() => Instance.ShowDetail(inventory_KeyItemButton[index]));
                inventory_KeyItemButton[i].AddComponent<MouseHover>();
                inventory_KeyItem[inventory_KeyItemButton[i]] = inventory_KeyItemData[i];
                buttonToButtonDict[inventory_KeyItemButton[i]] = inventory_KeyItem;
                UpdateInventory(inventory_KeyItemButton[i], inventory_KeyItem[inventory_KeyItemButton[i]]);
            }

            initialized = true;
        }


        if (EmptyInventory != null)
        {
            GameObject setter = Craft.Instance.InventorySetter;
            for (int i = 0; i < setter.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                setter.transform.GetChild(i).SetParent(EmptyInventory.transform);
            }
            EmptyInventory = null;
        }
        UpdateCredit();
    }
    
    //인벤토리
    public void PublicItemGotoInventory(Item_SO item)
    {
        ItemGotoInventory(item);
    }
    private void ItemGotoInventory(Item_SO item)
    {
        if (ItemCategorize(item)) return;//true를 리턴받으면 그냥 이 함수 종료 -> default 구문까지 내려왔다는 뜻

        if (CircuitInventory(item)) return;//일단 넣으면 순환할 필요 없음.
        
        //여기는 모든칸이 다 찼을 때 로직 넣기. 일단 아이템 삭제가 아니라 아이템풀에다가 다시 던져넣으면될듯?
        //Debug.Log("PassLoop");
        ItemPool_SO.Instance.GoPool_Item(item);
    }

    private bool CircuitInventory(Item_SO item)
    {
        for (int i = 0; i < targetInventoryItemDatas.Count; i++)//인벤토리 내부순회
        {
            if (EnterInventory_Blank(item, i, out var currentItem)) //빈칸이면 넣는다. 빈칸이 아니면? 
            {
                return true;
            }
            //같은 아이템인지 확인한다.
            if (currentItem.data.ItemName.Equals(item.data.ItemName))//같은 아이템인 경우
            {
                //같은 아이템인데 Quantity가 MaxQuantity보다 작은 경우
                if (ItemStackCountWithoutBlank(targetInventoryItemDatas[i]) < item.data.MaxQuantity)
                {
                    targetInventoryItemDatas[i].Push(item); //일단 넣어
                    UpdateItemQuantity(targetInventoryButtons[i]); //수량만 변화
                    return true;
                }
            }
            else//다른 아이템인경우
            {
                continue;
            }
        }

        return false;
    }

    private bool ItemCategorize(Item_SO item)
    {
        switch (item.data.ItemCategory)
        {
            //획득한
            case (int)EnumItemCategory.Equipment:
                //아이템 카테고리가 장비아이템일 때 -> 일반아이템과 동일취급
                if (!targetInventoryItemDatas.Equals(ItemDataGroup[2]))
                {
                    //Debug.Log("E");
                    targetInventoryItemDatas = ItemDataGroup[2];
                    targetInventoryButtons = buttonListGroup[2];   
                }
                break;
            case (int)EnumItemCategory.ConsumableItem:
                //아이템 카테고리가 소비아이템일 때
                if (!targetInventoryItemDatas.Equals(ItemDataGroup[1]))
                {
                    //Debug.Log("C");
                    targetInventoryItemDatas = ItemDataGroup[1];
                    targetInventoryButtons = buttonListGroup[1];    
                }
                break;
            case (int)EnumItemCategory.CommonItem:
                //아이템 카테고리가 일반 아이템일 때
                if (!targetInventoryItemDatas.Equals(ItemDataGroup[2]))
                {
                    //Debug.Log("CI");
                    targetInventoryItemDatas = ItemDataGroup[2];
                    targetInventoryButtons = buttonListGroup[2];    
                }
                break;
            case (int)EnumItemCategory.KeyItem:
                //아이템 카테고리가 중요 아이템일 때
                if (!targetInventoryItemDatas.Equals(ItemDataGroup[3]))
                {
                    //Debug.Log("K");
                    targetInventoryItemDatas = ItemDataGroup[3];
                    targetInventoryButtons = buttonListGroup[3];    
                }
                break;
            default:
                //아이템 카테고리가 빈칸, 마지막빈칸일때 혹은 그외의 아이템코드일 때
                //Debug.LogError($"{item.data.ItemCategory} is not defined.");
                return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item">item, maybe enter to inventory</param>
    /// <param name="i">inventory space number</param>
    /// <param name="currentItem">item, already in inventory space number</param>
    /// <returns></returns>
    private bool EnterInventory_Blank(Item_SO item, int i, out Item_SO currentItem)
    {
        currentItem = targetInventoryItemDatas[i].Peek();
        if (currentItem.data.ItemName.Equals(EnumItemCode.Blank.ToString()) || currentItem.data.ItemName.Equals(EnumItemCode.LastBlank.ToString()))//비어있는 칸에 넣는 것
        {
            targetInventoryItemDatas[i].Push(item);
            UpdateInventory(i);
            return true;
        }
        return false;
    }
    
    private Button UpdateInventory(int inventorySpaceNumber)
    {
        //Debug.Log(InventorySpace.Length);
        Button currentInventorySpace = targetInventoryButtons[inventorySpaceNumber];
        Stack<Item_SO> currentItemStack = targetInventoryItemDatas[inventorySpaceNumber];
        targetInventory[currentInventorySpace] = currentItemStack;
        UpdateItemImage(currentInventorySpace);
        UpdateItemQuantity(currentInventorySpace);
        return currentInventorySpace;
    }
    
    private Button UpdateInventory(Button button, Stack<Item_SO> stack)
    {
        //Debug.Log(InventorySpace.Length);
        targetInventory[button] = stack;
        UpdateItemImage(button);
        UpdateItemQuantity(button);
        return button;
    }
    
    /// <summary>
    /// Update itemImage in inventory. must function before UpdateItemQuantity
    /// </summary>
    /// <param name="button"></param>
    private void UpdateItemImage(Button button)
    {
        if (button.TryGetComponent(out Image inventorySpaceImage))
        {
            //Debug.Log(targetInventory[button].Peek().data.ItemName);
            inventorySpaceImage.sprite = targetInventory[button].Peek().Icon;    
        }
    }
    /// <summary>
    /// Update DropQuantity in inventory. must function after UpdateItemImage
    /// </summary>
    /// <param name="currentInventorySpace"></param>
    private void UpdateItemQuantity(Button currentInventorySpace)
    {
        /*GetComponent는 해당 타입의 컴퍼넌트를 찾지 못했을 때 memory allocation을 발생시켜
        Garbage Collection을 야기할 수 있다.
        하지만 TryGetComponent는 해당 컴퍼넌트를 찾지 못했을 때 memory allocation이 발생하지 않고 Garbage Collection을 걱정할 필요없다.*/
        if (currentInventorySpace.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI itemQuantity))
        {
            Item_SO item = targetInventory[currentInventorySpace].Peek();
            //Debug.Log(item.data.ItemName);
            //아이템이 빈칸 또는 막혀있음 이면 
            if (item.data.ItemCode.Equals(0.ToString()) || item.data.ItemCode.Equals(1.ToString()))
            {
                //수량 표시 X
                UpdateItemImage(currentInventorySpace);
                itemQuantity.text = "";
                return;
            }
            itemQuantity.text = $"{ItemStackCountWithoutBlank(targetInventory[currentInventorySpace])}";
        }
        else
        {
            //Debug.LogError("CriticalError");
        }
        
        
        /*//수락한 퀘스트가 있는 경우에만 체크. List.Any() -> 리스트가 비어있는지 확인. 요소가 있으면 true 없으면 false
        if (!PlayerQuestWindow.Instance.questList.Equals(null))
        {
            PlayerQuestWindow.Instance.QuestStatusCheck();    
        }*/
    }
    //인벤토리 공간을 늘려요.
    private void AddNewInventorySpace()
    {
        List<GameObject> uiTabContents = UiManager.Instance.uiTabContents;
        //버튼을 동적으로 추가하기. 주의할점은 transform에다가 추가해야한다는 것이다!
        /*GameObject space = */Instantiate(inventorySpace, uiTabContents[1].gameObject.transform);
        //Debug.Log(space.transform.childCount);
    }
    //인벤토리
    
    private void ShowDetail(Button button)
    {
        if (Instance.gameObject.activeInHierarchy)
        {
            /*//이 버튼이 어느 카테고리에 속해있는지 확인
        Debug.Log(button.gameObject.transform.parent.parent.parent.parent.gameObject.name);*/
        
            /*코드해석
            buttonToDict에서 button에 해당하는 dictionary를 가져오려고 시도한다.
            가져와지면 그 dictionary를 targetDict로 뱉는다.*/
            if (buttonToButtonDict.TryGetValue(button, out var targetDict))
            {
                // targetDict에서 필요한 값을 찾음
                // 디테일창을 띄울 위치는 화면위치의 마우스위치이므로 월드좌표계 미사용
                //데이터 세팅
                Debug.Log($"찾은 값: {targetDict[button].Peek().data.ItemName}, Count: {ItemStackCountWithoutBlank(targetDict[button])}");
                if (!itemDetailView.activeInHierarchy)
                {
                    if (targetDict[button].Peek().data.ItemCode.Equals(((int)EnumItemCode.Blank).ToString()) || targetDict[button].Peek().data.ItemCode.Equals(((int)EnumItemCode.LastBlank).ToString()))
                    {
                        //빈칸이면
                        return;
                    }
                    itemDetailView.transform.GetChild(0).gameObject.GetComponent<Image>().sprite =
                        targetDict[button].Peek().Icon;
                    itemDetailView.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text =
                        targetDict[button].Peek().data.ItemName;
                    itemDetailView.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                        targetDict[button].Peek().data.KoreanDetail;
                    
                    itemDetailView.SetActive(true);
                    itemDetailView.transform.position = Input.mousePosition;
                    itemSimpleView.SetActive(false);
                }
                else
                {
                    itemDetailView.SetActive(false);
                    itemSimpleView.SetActive(true);
                }
            }
            else
            {
                Debug.Log("버튼에 해당하는 딕셔너리를 찾을 수 없습니다.");
            }
        }
        
    }

    public void ShowSimple(Button button)
    {
        if (Instance.gameObject.activeInHierarchy)
        {
            if (!itemSimpleView.activeInHierarchy)
            {
                //데이터 세팅
                if (buttonToButtonDict.TryGetValue(button, out var targetDict))
                {
                    if (targetDict[button].Peek().data.ItemCode.Equals(((int)EnumItemCode.Blank).ToString()) || targetDict[button].Peek().data.ItemCode.Equals(((int)EnumItemCode.LastBlank).ToString()))
                    {
                        //빈칸이면
                        return;
                    }
                    itemSimpleView.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                        targetDict[button].Peek().data.ItemName;
                    itemSimpleView.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text =
                        targetDict[button].Peek().data.KoreanDetail;
                }

                itemSimpleView.SetActive(true);
                itemSimpleView.transform.position = Input.mousePosition;
            }    
        }
        
    }

    public void HideDetail()
    {
        if (itemSimpleView.activeInHierarchy)
        {
            itemSimpleView.SetActive(false);
            
        }
        if (itemDetailView.activeInHierarchy)
        {
            itemDetailView.SetActive(false);
        }
    }

    public void MoveDetail()
    {
        if (itemSimpleView.activeInHierarchy)
        {
            itemSimpleView.transform.position = Input.mousePosition;
            return;
        }

        if (itemDetailView.activeInHierarchy)
        {
            itemDetailView.transform.position = Input.mousePosition;
        }
    }

    public void EquipItem(Button clickedButton)
    {
        if (buttonToButtonDict.TryGetValue(clickedButton, out var targetDict))
        {
            //Debug.Log(clickedButton.transform.parent.parent.parent.parent.gameObject.name);
            
            //누른버튼이 장비칸의 버튼이 아니면
            if (!clickedButton.transform.parent.parent.gameObject.Equals(equipments))
            {
                switch (targetDict[clickedButton].Peek().data.ItemCategory)
                {
                    case (int)EnumItemCategory.Equipment:
                        //equipItem으로 이동하는 로직
                        //Debug.Log(targetDict[clickedButton].Peek().data.ItemName);
                        targetInventoryItemDatas = ItemDataGroup[0];
                        targetInventoryButtons = buttonListGroup[0];   
                        CircuitInventory(targetDict[clickedButton].Pop());
                        /*//일반 아이템창을 정리. 
                        TidyInventoryData(ItemDataGroup[2]);
                        TidyButton(buttonListGroup[2]);*/
                        UpdateInventory(clickedButton, targetDict[clickedButton]);
                        return;
                    case (int)EnumItemCategory.ConsumableItem:
                        //equipItem으로 이동하는 로직
                        //Debug.Log(targetDict[clickedButton].Peek().data.ItemName);
                        targetInventoryItemDatas = ItemDataGroup[0];
                        targetInventoryButtons = buttonListGroup[0];
                        int loop = ItemStackCountWithoutBlank(targetDict[clickedButton]);
                        for (int i = 0; i < loop; i++)
                        {
                            //이 코드에서, 클릭한 버튼의 스택에서 아이템을 꺼내서 타겟인벤토리[0]에 넣는다.
                            CircuitInventory(targetDict[clickedButton].Pop());
                        }
                        /*//소비 아이템창을 정리
                        TidyInventoryData(ItemDataGroup[1]);
                        TidyButton(buttonListGroup[1]);*/
                        UpdateInventory(clickedButton, targetDict[clickedButton]);
                        return;
                }
            }
            else//누른 버튼이 장비칸의 버튼이면
            {
                switch (targetDict[clickedButton].Peek().data.ItemCategory)
                {
                    case (int)EnumItemCategory.Equipment:
                        //equipItem으로 이동하는 로직
                        //Debug.Log(targetDict[clickedButton].Peek().data.ItemName);
                        targetInventoryItemDatas = ItemDataGroup[2];
                        targetInventoryButtons = buttonListGroup[2];   
                        CircuitInventory(targetDict[clickedButton].Pop());
                        UpdateInventory(clickedButton, targetDict[clickedButton]);
                        return;
                    case (int)EnumItemCategory.ConsumableItem:
                        //equipItem으로 이동하는 로직
                        //Debug.Log(targetDict[clickedButton].Peek().data.ItemName);
                        targetInventoryItemDatas = ItemDataGroup[1];
                        targetInventoryButtons = buttonListGroup[1];   
                        int loop = ItemStackCountWithoutBlank(targetDict[clickedButton]);
                        for (int i = 0; i < loop; i++)
                        {
                            CircuitInventory(targetDict[clickedButton].Pop());
                        }
                        /*//장비칸의 소비아이템창을 따로 만들어야해서 잠시 보류
                        TidyButton();*/
                        UpdateInventory(clickedButton, targetDict[clickedButton]);
                        return;
                }
            }
        }
        Debug.Log("No equipItem");
    }

    private int ItemStackCountWithoutBlank<T>(Stack<T> stack)
    {
        return stack.Count - 1;
    }
    private int ItemStackCountWithoutBlank(int stackCount)
    {
        return stackCount - 1;
    }

    /// <summary>
    /// Stack Tidy
    /// </summary>
    private void TidyInventoryData(List<Stack<Item_SO>> list)
    {
        //0번째 칸이 아닌 1번째 칸부터 확인. 0번째 칸 즉 n-1번째 칸이 빈칸이면(lastblank는 들어올 수 없음.)
        for (int i = 1; i < list.Count; i++)
        {
            //Debug.Log(list[i-1].Peek().data.ItemCode);
            //Debug.Log(!list[i].Peek().data.ItemCode.Equals(EnumItemCode.Blank.ToString()));
            //이 전칸이 빈칸이면.. 지금 여기가 안걸림
            if (list[i-1].Peek().data.ItemCode.Equals(EnumItemCode.Blank.ToString())/* && !list[i].Peek().data.ItemCode.Equals(EnumItemCode.Blank.ToString())*/)
            {
                //Debug.Log("TidyStart");
                //빈칸, 마지막 빈칸 이면 멈춘다.
                if (list[i].Peek().data.ItemCode.Equals(EnumItemCode.LastBlank.ToString()))
                {
                    return;   
                }
                /*//비어있는 스택을 임시 스택에 할당. + 아래 구문으로 줄일 수 있음.
                Stack<Item_SO> tempStack = list[i-1];
                list[i - 1] = list[i];
                list[i] = tempStack;*/
                (list[i - 1], list[i]) = (list[i], list[i - 1]);
                i--;//한번더 확인
            }
        }
    }

    private void TidyButton(List<Button> list)
    {
        foreach (var t in list)
        {
            Dictionary<Button, Stack<Item_SO>> inventoryButton = buttonToButtonDict[t];
            Stack<Item_SO> stack = inventoryButton[t];
            UpdateInventory(t, stack);
        }
    }

    //인자로 받은 아이템이름과 같은 아이템이 있는지 확인하고 수량을 확인하기
    public bool CheckItem(string itemName, int count)
    {
        foreach (List<Stack<Item_SO>> data in ItemDataGroup)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Peek().data.ItemName.Equals(itemName))
                {
                    if (ItemStackCountWithoutBlank(data[i])>=count)
                    {
                        return true;
                    }
                }
            }       
        }
        //Debug.Log("Can't Find");
        return false;
    }
    
    //인자로 받은 아이템이름과 같은 아이템을 꺼내기
    public Item_SO FindAndReturnItem(string itemName)
    {
        foreach (List<Stack<Item_SO>> data in ItemDataGroup)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Peek().data.ItemName.Equals(itemName))
                {
                    Item_SO item = data[i].Pop();
                    ItemCategorize(item);
                    for (int j = 0; j < targetInventoryItemDatas.Count; j++)
                    {
                        //마지막 1개라면 위에서 이미 팝 했으므로 여기서 픽해서 아이템 네임 비교하면 안됨. 
                        if (targetInventoryItemDatas[i].Peek().data.ItemName.Equals(itemName))
                        {
                            
                            //Debug.Log(targetInventoryItemDatas[i].Peek().data.ItemName);
                            UpdateInventory(targetInventoryButtons[i], targetInventoryItemDatas[i]);
                            break;
                        }
                        if (targetInventoryItemDatas[i].Peek().data.ItemName.Equals("Blank") || targetInventoryItemDatas[i].Peek().data.ItemName.Equals("LastBlank"))
                        {
                            UpdateInventory(targetInventoryButtons[i], targetInventoryItemDatas[i]);
                            break;
                        }
                    }
                    return item;
                }
            }       
        }
        Debug.Log("Can't Find");
        return null;
    }

    public void CalculateCredit(int amount)
    {
        Credit += amount;
        UpdateCredit();
    }

    private void UpdateCredit()
    {
        credit.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Credit.ToString();
        NPCFunctionManager.Instance.UpdateCredit();
    }

    public void MoveConsumableInventorySpace()
    {
        //Debug.Log("C");
        GameObject setter = Craft.Instance.InventorySetter;
        //아직 옮겨진 인벤토리가 없다면
        if (EmptyInventory == null)
        {
            for (int i = 0; i < consumable.transform.childCount; i++)
            {
                //컨슈머블의 인벤토리를 옮기고                
                consumable.transform.GetChild(i).SetParent(setter.transform);
            }
            //컨슈머블이 옮겨졌음을 알림
            EmptyInventory = consumable;
        }
        else//옮겨졌던 인벤토리가 있다면
        {
            for (int i = 0; i < setter.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                setter.transform.GetChild(i).SetParent(EmptyInventory.transform);
            }
            
            for (int i = 0; i < consumable.transform.childCount; i++)
            {
                //컨슈머블의 인벤토리를 옮기고                
                consumable.transform.GetChild(i).SetParent(setter.transform);
            }
            //컨슈머블이 옮겨졌음을 알림
            EmptyInventory = consumable;
        }
    }
    
    public void MoveItemsInventorySpace()
    {
        //Debug.Log("I");
        GameObject setter = Craft.Instance.InventorySetter;
        if (EmptyInventory == null)
        {
            for (int i = 0; i < items.transform.childCount; i++)
            {
                items.transform.GetChild(i).SetParent(setter.transform);
            }
            EmptyInventory = items;
        }
        else//옮겨졌던 인벤토리가 있다면
        {
            for (int i = 0; i < setter.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                setter.transform.GetChild(i).SetParent(EmptyInventory.transform);
            }
            
            for (int i = 0; i < items.transform.childCount; i++)
            {
                //인벤토리를 옮기고                
                items.transform.GetChild(i).SetParent(setter.transform);
            }
            //옮겨졌음을 알림
            EmptyInventory = items;
        }
    }
    
    public void MoveKeyItemsInventorySpace()
    {
        //Debug.Log("K");
        GameObject setter = Craft.Instance.InventorySetter;
        if (EmptyInventory == null)
        {
            for (int i = 0; i < keyItems.transform.childCount; i++)
            {
                keyItems.transform.GetChild(i).SetParent(setter.transform);
            }
            EmptyInventory = keyItems;
        }
        else//옮겨졌던 인벤토리가 있다면
        {
            for (int i = 0; i < setter.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                setter.transform.GetChild(i).SetParent(EmptyInventory.transform);
            }
            
            for (int i = 0; i < keyItems.transform.childCount; i++)
            {
                //인벤토리를 옮기고                
                keyItems.transform.GetChild(i).SetParent(setter.transform);
            }
            //옮겨졌음을 알림
            EmptyInventory = keyItems;
        }
    }
    
    public void MoveConsumableInventorySpaceToRepair()
    {
        //Debug.Log("C");
        GameObject setter = Repair.Instance.SelectedInventory;
        //아직 옮겨진 인벤토리가 없다면
        if (EmptyInventory == null)
        {
            for (int i = 0; i < consumable.transform.childCount; i++)
            {
                //컨슈머블의 인벤토리를 옮기고                
                consumable.transform.GetChild(i).SetParent(setter.transform);
            }
            //컨슈머블이 옮겨졌음을 알림
            EmptyInventory = consumable;
        }
        else//옮겨졌던 인벤토리가 있다면
        {
            for (int i = 0; i < setter.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                setter.transform.GetChild(i).SetParent(EmptyInventory.transform);
            }
            
            for (int i = 0; i < consumable.transform.childCount; i++)
            {
                //컨슈머블의 인벤토리를 옮기고                
                consumable.transform.GetChild(i).SetParent(setter.transform);
            }
            //컨슈머블이 옮겨졌음을 알림
            EmptyInventory = consumable;
        }
    }
    
    public void MoveItemsInventorySpaceToRepair()
    {
        //Debug.Log("I");
        GameObject setter = Repair.Instance.SelectedInventory;
        if (EmptyInventory == null)
        {
            for (int i = 0; i < items.transform.childCount; i++)
            {
                items.transform.GetChild(i).SetParent(setter.transform);
            }
            EmptyInventory = items;
        }
        else//옮겨졌던 인벤토리가 있다면
        {
            for (int i = 0; i < setter.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                setter.transform.GetChild(i).SetParent(EmptyInventory.transform);
            }
            
            for (int i = 0; i < items.transform.childCount; i++)
            {
                //인벤토리를 옮기고                
                items.transform.GetChild(i).SetParent(setter.transform);
            }
            //옮겨졌음을 알림
            EmptyInventory = items;
        }
    }
    
    public void MoveKeyItemsInventorySpaceToRepair()
    {
        //Debug.Log("K");
        GameObject setter = Repair.Instance.SelectedInventory;
        if (EmptyInventory == null)
        {
            for (int i = 0; i < keyItems.transform.childCount; i++)
            {
                keyItems.transform.GetChild(i).SetParent(setter.transform);
            }
            EmptyInventory = keyItems;
        }
        else//옮겨졌던 인벤토리가 있다면
        {
            for (int i = 0; i < setter.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                setter.transform.GetChild(i).SetParent(EmptyInventory.transform);
            }
            
            for (int i = 0; i < keyItems.transform.childCount; i++)
            {
                //인벤토리를 옮기고                
                keyItems.transform.GetChild(i).SetParent(setter.transform);
            }
            //옮겨졌음을 알림
            EmptyInventory = keyItems;
        }
    }
    
    public void ResetInventorySpace(GameObject space)
    {
        space.transform.SetParent(EmptyInventory.transform);
    }

    public void Reload()
    {
        /*for (int i = 0; i < ItemDataGroup.Count; i++)
        {
            for (int j = 0; j < ItemDataGroup[i].Count; j++)
            {
                while (ItemDataGroup[i][j].Peek().data.ItemCategory > 2)
                {
                    
                }
            }
        }*/
    }
}
