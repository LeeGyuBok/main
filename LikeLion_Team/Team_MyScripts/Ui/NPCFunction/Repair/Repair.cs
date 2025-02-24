using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum EnumRepairMode
{
    Selected,
    Equipped,
    All
}

public class Repair : MonoBehaviour
{
    public static Repair Instance;

    [SerializeField] private Button repairSelected;
    [SerializeField] private Button repairEquipped;
    [SerializeField] private Button repairAll;
    [SerializeField] private TextMeshProUGUI repairCost;

    [SerializeField] private GameObject repairSpaceTable;
    [SerializeField] private GameObject repairSpacePrefab;

    [SerializeField] private GameObject selectedInventory;
    [SerializeField] private GameObject inventorySelector;
    public GameObject SelectedInventory => selectedInventory;

    private Button clickedButton;
    private List<Button> repairTableButtons;
    private List<IRepairable> repairItemList;

    private Dictionary<Button, IRepairable> repairButtonToRepairables;
    private int totalCostToRepair;

    private EnumRepairMode repairMode = EnumRepairMode.Selected;

    private List<UnityAction> buttonListeners;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            repairItemList = new List<IRepairable>();
            repairTableButtons = new List<Button>();
            repairButtonToRepairables = new Dictionary<Button, IRepairable>();
            buttonListeners = new List<UnityAction>();

            /*Debug.Log(repairSelected.name);
            Debug.Log(repairEquipped.name);
            Debug.Log(repairAll.name);
            Debug.Log(repairCost.name);

            Debug.Log(repairSpacePrefab.name);*/
        }
        NPCFunctionManager.RepairWindow = Instance;
    }

    //셀렉티드리페어 모드시 아이템창을 누르면 해당 함수 실행
    private void SelectRepairItem(Button button)
    {
        //수리할 아이템을 좌클릭한다.    
        
        //작업대에 생성된 공간이 없다면
        if (repairSpaceTable.transform.childCount <= 0)
        {
            //만들고
            CreateRepairSpace();
        }
        //이제 공간이 있으니.
        
        if (repairSpaceTable.transform.childCount > 0)
        {
            //클릭한 인벤토리버튼이 갖고있는 아이템을 Peek해서
            Item_SO selectedItem = UiManager.Inventory.buttonToButtonDict[button][button].Peek();
            //테스트코드 시작
            if (selectedItem is Liminex liminex)
            {
                liminex.RepairLiminex();
                Debug.Log(liminex.IsActivate); //트루래요~
                return;
            }
            //테스트코드 끝
            //내구도가 있는 아이템인지 확인한다.
            CategorizeRepairable(selectedItem);
        }
        else
        {
            Debug.Log("No space");
            return;
        }
    }
    
    private void CostCalculate()
    {
        totalCostToRepair = 0;
        if (repairItemList.Count <= 0)
        {
            //Debug.Log("no repair item");
            return;
        }
        //스페이스에 들어온 아이템들만 수리한다.
        foreach (IRepairable repairableItem in repairItemList)
        {
            int currentDurability = repairableItem.CurrentDurability;
            int maxDurability = repairableItem.MaxDurability;

            totalCostToRepair += 100 - (int)(currentDurability/maxDurability * 100);
            //Debug.Log(totalCostToRepair);
        }
    }

    //IRepairable 만을 선택하여 수리하는 모드로 전환
    public void RepairSelected()
    {
        //처음 눌렀을 때
        if (!repairMode.Equals(EnumRepairMode.Selected))
        {
            repairMode = EnumRepairMode.Selected;
            for (int i = 0; i < repairSpaceTable.transform.childCount; i++)
            {
                RemoveListenerToSelectedInventoryButton();
                DestroyImmediate(repairSpaceTable.transform.GetChild(i).gameObject);
            }
            return;
        }
        
        Debug.Log(repairMode);
        
        //두번째 눌렀을 때 실제로 아이템의 내구도를 수리
        StartCoroutine(RepairDelay(repairItemList.Count));
    }

    //장착하고 있는 아이템 중 IRepairable만 수리하는 모드로 전환
    public void RepairEquipped()
    {
        //처음 눌렀을 때
        if (!repairMode.Equals(EnumRepairMode.Equipped))
        {
            repairMode = EnumRepairMode.Equipped;    
            for (int i = 0; i < repairSpaceTable.transform.childCount; i++)
            {
                RemoveListenerToSelectedInventoryButton();
                DestroyImmediate(repairSpaceTable.transform.GetChild(i).gameObject);
            }
            if (repairSpaceTable.transform.childCount <= 0)
            {
                //만들고
                CreateRepairSpace();
                //잠깐 기다렸다가 아이템을 채우기
                StartCoroutine(FillImageDelay_Equipped(0.2f));
            }
            return;
        }
        
        //장착하고있는 아이템들을 수리아이템리스트에 넣기
        
        
        //실제로 아이템의 내구도를 수리
        StartCoroutine(RepairDelay(repairItemList.Count));
        
        Debug.Log(repairMode);
    }

    //인벤토리 전체를 순회하며 IRepairable만 수리하는 모드로 전환
    public void RepairAll()
    {
        //처음누른다.
        if (!repairMode.Equals(EnumRepairMode.All))
        {
            repairMode = EnumRepairMode.All;    
            for (int i = 0; i < repairSpaceTable.transform.childCount; i++)
            {
                RemoveListenerToSelectedInventoryButton();
                DestroyImmediate(repairSpaceTable.transform.GetChild(i).gameObject);
            }
            if (repairSpaceTable.transform.childCount <= 0)
            {
                //만들고
                CreateRepairSpace();
                //잠깐 기다렸다가 아이템을 채우기
                StartCoroutine(FillImageDelay_All(0.2f));
            }
            //인벤토리에 있는 모든 영역에서, 수리가능한 아이템들을 수리아이템리스트에 넣기
            //장착하고있는 아이템들을 수리아이템리스트에 넣기
            return;
        }
        
        //실제로 아이템의 내구도를 수리
        StartCoroutine(RepairDelay(repairItemList.Count));
        
        Debug.Log(repairMode);
    }
    
     private void CategorizeRepairable(Item_SO selectedItem)
    {
        //내구도가 있는 아이템이면
        if (selectedItem is IRepairable repairableItem)
        {
            //내구도가 손상되었는지 확인한다.
            if (repairableItem.PossibleRepair)
            {
                //내구도가 손상되었다면 아이템정보를 리스트에 추가하고 작업한다.
                repairItemList.Add(repairableItem);
                
                for (int i = 0; i < repairSpaceTable.transform.childCount; i++)
                {
                    Transform spacePrefab = repairSpaceTable.transform.GetChild(i);
                    for (int j = 0; j < spacePrefab.childCount; j++)
                    {
                        Button spaceButton = spacePrefab.GetChild(j).GetComponent<Button>();
                        //Debug.Log(spaceButton.gameObject.name);
                        //버튼을 갖고와서, 버튼의 이미지가 블랭크 이면 == 버튼에 할당된 아이템이 없으면 넣는다.
                        if (spaceButton.image.sprite.name.Equals("Blank"))
                        {
                            //수리창의 버튼과 내구도를 수리할 아이템을 딕셔너리로 연결
                            repairButtonToRepairables[spaceButton] = repairItemList[i];
                            //수리창의 버튼 이미지를 내구도를 수리할 아이템의 이미지로 변경
                            spaceButton.image.sprite = selectedItem.Icon;

                            //새로운 아이템을 넣을 때 마다 다시 계산
                            CostCalculate();
                                
                            break;
                        }
                        else//블랭크가 아니고 아이템이면
                        {
                            //이미 등록되어있는지 확인 
                            //버그수정기록 - j가 아닌 i로 하니, 기존에 맨 앞에있던아이템과 계속 비교하여 새로운 아이템으로 계속 인식했음.
                            if (selectedItem.Equals(repairItemList[j]))
                            {
                                repairItemList.Remove(repairableItem);
                                Debug.Log("duplicationItem");
                                return;
                            }
                        }
                        //중복아이템이 아니면 순회한다.
                        Debug.Log("newItem");
                    }
                }
                Debug.Log(repairItemList.Count);
            }
        }
        else
        {
            Debug.Log("Not repairable");
            return;
        }
    }


    private void RepairItems()
    {
        foreach (IRepairable items in repairItemList)
        {
            Debug.Log($"before: {items.CurrentDurability}");
            /*items.RepairDurability(items.MaxDurability);*/
            //테스트용코드 위에게 진짜 코드
            items.RepairDurability(5);
            Debug.Log($"after: {items.CurrentDurability}");
        }
    }

    //테이블을 생성한다.
    private void CreateRepairSpace()
    { 
        Instantiate(repairSpacePrefab, repairSpaceTable.transform);    
    }
    
    public void ShowConsumableInventoryOnRepair()
    {
        if (SelectedInventory.transform.childCount > 0)
        {
            //들어오긴하는데?
            Debug.Log("Remove");
            RemoveListenerToSelectedInventoryButton();
        }
        UiManager.Inventory.MoveConsumableInventorySpaceToRepair();
        AddListenerToSelectedInventoryButton();
    }
    public void ShowItemInventoryOnRepair()
    {
        if (SelectedInventory.transform.childCount > 0)
        {
            RemoveListenerToSelectedInventoryButton();
        }
        UiManager.Inventory.MoveItemsInventorySpaceToRepair();
        AddListenerToSelectedInventoryButton();
    }
    public void ShowKeyItemInventoryOnRepair()
    {
        if (SelectedInventory.transform.childCount > 0)
        {
            RemoveListenerToSelectedInventoryButton();
        }
        UiManager.Inventory.MoveKeyItemsInventorySpaceToRepair();
        AddListenerToSelectedInventoryButton();
    }
    public void ShowStorageOnRepair()
    {
        Debug.Log("not yet");
        /*if (SelectedInventory.transform.childCount > 0)
        {
            RemoveListenerToSelectedInventoryButton();
        }
        
        AddListenerToSelectedInventoryButton();*/
    }
    
    public void InventorySelect()
    {
        if (!inventorySelector.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            inventorySelector.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            inventorySelector.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    
    private void OnDisable()
    {
        if (SelectedInventory.transform.childCount > 0)
        {
            for (int i = 0; i < SelectedInventory.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                UiManager.Inventory.ResetInventorySpace(SelectedInventory.transform.GetChild(i).gameObject);
            }
        }
    }

    //람다식을 사용하면 서로다른 메모리주소에 할당되어 작동하지 않음.
    //일단은 리무브 올 리스너로 진행
    //리스너리스트를 활용해서 할당한 리스너를 저장하고 저장된 리스널를 다시 제거하는 식으로 변경
    //인벤토리에서 버튼에 대한 호버링을 할당중임...
    private void AddListenerToSelectedInventoryButton()
    {
        if (buttonListeners.Count > 0)
        {
            buttonListeners.Clear();
        }
        
        for (int i = 0; i < SelectedInventory.transform.childCount; i++)
        {
            Transform selectedChild = SelectedInventory.transform.GetChild(i);
            for (int j = 0; j < selectedChild.childCount; j++)
            {
                Button button = selectedChild.GetChild(j).GetComponent<Button>();
                UnityAction listener = () => SelectRepairItem(button);
                buttonListeners.Add(listener);
                button.onClick.AddListener(listener);
            }
        }
    }

    private void RemoveListenerToSelectedInventoryButton()
    {
        for (int i = 0; i < SelectedInventory.transform.childCount; i++)
        {
            Transform selectedChild = SelectedInventory.transform.GetChild(i);
            for (int j = 0; j < selectedChild.childCount; j++)
            {
                Button button = selectedChild.GetChild(j).GetComponent<Button>();
                if (buttonListeners.Count > 0)
                {
                    button.onClick.RemoveListener(buttonListeners[j]);    
                }
                
            }
        }

        if (buttonListeners.Count > 0)
        {
            buttonListeners.Clear();
        }
    }

    /// <summary>
    /// RepairItemCount/2 is delayTime
    /// </summary>
    /// <param name="repairItemCount"></param>
    /// <returns></returns>
    private IEnumerator RepairDelay(int repairItemCount)
    {
        //사용자의 모든 인풋을 막는 방법필요. 1번 - 투명한 창을 하나 올렸다가 내린다. 쓸데없는 짓 못하게.
        float delay = (float)repairItemCount / 2;
        if (repairItemCount > 0)
        {
            yield return new WaitForSeconds(delay);
            RepairItems();
            repairItemList.Clear();
            for (int i = 0; i < repairSpaceTable.transform.childCount; i++)
            {
                Transform spacePrefab = repairSpaceTable.transform.GetChild(i);
                for (int j = 0; j < spacePrefab.childCount; j++)
                {
                    Button spaceButton = spacePrefab.GetChild(j).GetComponent<Button>();
                    spaceButton.image.sprite = Resources.Load<Sprite>("Icons/Blank");
                }
            }

        }
        else
        {
            Debug.Log("List empty");
        }
        
    }

    private IEnumerator FillImageDelay_Equipped(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (KeyValuePair<Button, Stack<Item_SO>> pair in UiManager.Inventory.inventory_Equipments)
        {
            Item_SO selectedItem = pair.Value.Peek();
            CategorizeRepairable(selectedItem);
        }
    }
    
    private IEnumerator FillImageDelay_All(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (List<Stack<Item_SO>> inventoryList in UiManager.Inventory.ItemDataGroup)
        {
            foreach (Stack<Item_SO> inventorySpace in inventoryList)
            {
                CategorizeRepairable(inventorySpace.Peek());
            }
        }
    }
}
