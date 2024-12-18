using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Upgrade : MonoBehaviour
{
    public static Upgrade Instance;
    
    //자주 사용하는 오브젝트
    [SerializeField] private GameObject itemRenderer;
    [SerializeField] private GameObject materialsSpace;
    [SerializeField] private GameObject upgradeState;
    [SerializeField] private GameObject showUpgradeableSpace;
    [SerializeField] private Button upgradeButton;

    [SerializeField] private GameObject materialShowPrefab;
    [SerializeField] private GameObject statusChangeShowPrefab;
    [SerializeField] private GameObject statusNonChangeShowPrefab;
    [SerializeField] private GameObject upgradeableShowPrefab;
    //자주 사용하는 오브젝트
    
    //로직용
    private Item_SO selectedItemToUpgrade;
    
    private List<Item_SO> showedUpgradeableItemList;
    private Dictionary<Button, Item_SO> buttonToItem;
    private List<UnityAction> buttonListeners;
    private Dictionary<ItemData_SO, int> upgradeableItemMaterials;
    
    private Item_SO selectedItem;
    
    private List<Item_SO> requestedItemToUpgrade;

    private int costToUpgrade;
    private Dictionary<Item_SO, Button> revertToButton;
    private Dictionary<Button, UnityAction> buttonByAction;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            showedUpgradeableItemList = new List<Item_SO>();
            buttonToItem = new Dictionary<Button, Item_SO>();
            buttonListeners = new List<UnityAction>();
            upgradeableItemMaterials = new Dictionary<ItemData_SO, int>();
            requestedItemToUpgrade = new List<Item_SO>();
            revertToButton = new Dictionary<Item_SO, Button>();
            buttonByAction = new Dictionary<Button, UnityAction>();
        }
        NPCFunctionManager.UpgradeWindow = Instance;
        Debug.Log("Awake");
    }

    //업그레이드 창을 켜면, 업그레이드 가능한 아이템들을 찾아 리스트로 만든다.
    public void ShowUpgradeable()
    {
        //업그레이드창을 누르면 바로갱신. 어떻게?
        //그냥 다 지우고 다시 생성한다.
        int objectCount = showUpgradeableSpace.transform.childCount;
        for (int i = 0; i < objectCount; i++)
        {
            //여기서 버튼을 지운다. 
            //지우기전에 리스너를 지운다.
            showUpgradeableSpace.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            DestroyImmediate(showUpgradeableSpace.transform.GetChild(0).gameObject);
            showedUpgradeableItemList.Clear();
        }

        
        //아이템창에 있는 모든 아이템들을 검사한다. 
        foreach (List<Stack<Item_SO>> inventoryList in UiManager.Inventory.ItemDataGroup)
        {
            foreach (Stack<Item_SO> inventorySpace in inventoryList)
            {
                //아이템을 꺼내서
                Item_SO item = inventorySpace.Peek();
                Debug.Log(item.data.ItemName);
                //무기인지?
                if (item is IWeaponUpgradeable upgradeableWeaponItem)
                {
                    //아이템이 강화가 가능한지 그러니까.. 강화가능 회 수가 남았는지 확인한다. 강화가 가능하지 않다면
                    if (!upgradeableWeaponItem.IsUpgradeable)
                    {
                        Debug.Log("remove process1");//여긴들어옴
                        //위에서 버튼(값)을 지워서 널이다. 즉 키는 남았다.
                        //또는 해당하는 키가 존재하는지 확인한다.
                        if (revertToButton.TryGetValue(item, out Button button))
                        {
                            Debug.Log("remove process2");
                            //위에서 리스너를 지웠기때문에 딕셔너리에서 키-널 을 지운다.
                            buttonByAction.Remove(revertToButton[item]);
                            buttonToItem.Remove(revertToButton[item]);
                            revertToButton.Remove(item);
                            
                            
                            if (selectedItem == item)
                            {
                                selectedItem = null;    
                            }
                            
                            itemRenderer.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
                            
                            int destroyObjectCount = materialsSpace.transform.childCount;
                            
                            if (destroyObjectCount != 0)
                            {
                                for (int i = 0; i < destroyObjectCount; i++)
                                {
                                    DestroyImmediate(materialsSpace.transform.GetChild(0).gameObject);
                                }
                
                                destroyObjectCount = upgradeState.transform.GetChild(1).childCount;
                                
                                if (destroyObjectCount != 0)
                                {
                                    for (int i = 0; i < destroyObjectCount; i++)
                                    {
                                        DestroyImmediate(upgradeState.transform.GetChild(1).GetChild(0).gameObject);
                                    }
                                }
                                upgradeState.transform.GetChild(0).gameObject.SetActive(false);
                            }
                        }
                        continue;
                    }
                    //업캐스팅이가능하면 리스트에 추가된 아이템인지 확인한다.
                    if (showedUpgradeableItemList.Contains(item))
                    {
                        Debug.Log("already exist");
                        continue;
                    }
                    else //신규아이템이면 아이템을 리스트에추가하고
                    {
                        showedUpgradeableItemList.Add(item);
                        //해당 아이템의 정보를 보여줄 버튼 프리팹을 그 버튼이 위치할 공간에 생성한다. 
                        GameObject upgradeableItemInfo = Instantiate(upgradeableShowPrefab, showUpgradeableSpace.transform);
                    
                        //이 코드는 이 아이템의 카테고리용 이미지를 할당하는 것으로 바꿔야함
                        upgradeableItemInfo.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = item.Icon;
                    
                        upgradeableItemInfo.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = item.data.ItemName;
                        if (upgradeableWeaponItem is EquipItem_Weapon)
                        {
                            upgradeableItemInfo.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                                "Enforce Durability, AttackPower";
                        }
                        /*else if (item is EquipItem_Armor)
                        {
                            upgradeableItemInfo.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                                "Enforce Durability, Defence";
                        }
                        else if (item is KeyItem)
                        {
                            upgradeableItemInfo.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                                "Add SkillPoint";
                        }*/

                        upgradeableItemInfo.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text =
                            upgradeableWeaponItem.UpgradeCount.Count.ToString();
                        
                        Button button = upgradeableItemInfo.GetComponent<Button>();
                        UnityAction action = () => UpgradeableOnclick(button);
                        buttonListeners.Add(action);
                        button.onClick.AddListener(action);
                        buttonByAction[button] = action;
                        buttonToItem[button] = item;
                        revertToButton[item] = button;
                    }
                }
                //리미넥스인지?
                if (item is Liminex liminex)
                {
                    if (liminex.IsUpgradeable)
                    {
                        showedUpgradeableItemList.Add(item);
                        //해당 아이템의 정보를 보여줄 버튼 프리팹을 그 버튼이 위치할 공간에 생성한다. 
                        GameObject upgradeableItemInfo = Instantiate(upgradeableShowPrefab, showUpgradeableSpace.transform);
                        upgradeableItemInfo.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = item.data.ItemName;
                        upgradeableItemInfo.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                            "Add SkillPoint +1";
                        
                        upgradeableItemInfo.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text =
                            liminex.CurrentUpgradeCount.ToString();
                        
                        Button button = upgradeableItemInfo.GetComponent<Button>();
                        UnityAction action = () => UpgradeableOnclick(button);
                        buttonListeners.Add(action);
                        button.onClick.AddListener(action);
                        buttonByAction[button] = action;
                        buttonToItem[button] = item;
                        revertToButton[item] = button;
                        
                    }
                }
            }
        }
    }

    //생성된 업그레이트 아이템 리스트에서 업그레이드할 아이템을 선택하기
    public void UpgradeableOnclick(Button button)
    {
        selectedItem = buttonToItem[button];
        revertToButton[selectedItem] = button;    
        
        //선택한 아이템 이미지 보여주기
        itemRenderer.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = selectedItem.Icon;
        
        if (selectedItem is IWeaponUpgradeable upgradeableSelectedItem)
        {
            //여기가 재료 셋팅인데..
            //만약 이미 텍스트공간이 생성되어있으면
            int destroyObjectCount = materialsSpace.transform.childCount;
            if (destroyObjectCount != 0)
            {
                for (int i = 0; i < destroyObjectCount; i++)
                {
                    DestroyImmediate(materialsSpace.transform.GetChild(0).gameObject);
                }
            }
            
            //필요한 재료 아이템의 데이터 셋팅(개수, 이름 등)
            foreach (ItemData_SO materials in upgradeableSelectedItem.UpgradeMaterialsAndNeedCount.Keys)
            {
                upgradeableItemMaterials[materials] = upgradeableSelectedItem.UpgradeMaterialsAndNeedCount[materials];
                GameObject materialShow = Instantiate(materialShowPrefab, materialsSpace.transform);
                materialShow.GetComponent<TextMeshProUGUI>().text = materials.ItemName;
                materialShow.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    upgradeableItemMaterials[materials].ToString();
            }
            //증가하는 능력치보여주는 셋팅
            if (selectedItem is EquipItem_Weapon weaponItem)
            {
                Transform upgradeDegreeInfoWindow = upgradeState.transform.GetChild(0);
                //업그레이드 정보(아이템이름, 차수)
                upgradeDegreeInfoWindow.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    selectedItem.data.ItemName;
                int currentUpdateDegree = upgradeableSelectedItem.UpgradeCount.Count;
                upgradeDegreeInfoWindow.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text =
                    currentUpdateDegree.ToString();
                upgradeDegreeInfoWindow.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                    (currentUpdateDegree + 1).ToString();

                if (!upgradeState.transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    upgradeState.transform.GetChild(0).gameObject.SetActive(true);
                }
                //3번째 자식오브젝트는 단순 화살표

                List<string> statusLabel = new List<string>()
                {
                    "Durability", "AttackPower", "AttackRange", "AttackSpeed", "GuardEfficiency"
                };
                
                destroyObjectCount = upgradeState.transform.GetChild(1).childCount;
                if (destroyObjectCount != 0)
                {
                    for (int i = 0; i < destroyObjectCount; i++)
                    {
                        DestroyImmediate(upgradeState.transform.GetChild(1).GetChild(0).gameObject);
                    }
                }
                GameObject durabilityDifference = Instantiate(statusChangeShowPrefab, upgradeState.transform.GetChild(1));
                durabilityDifference.GetComponent<TextMeshProUGUI>().text = statusLabel[0];
                durabilityDifference.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    weaponItem.MaxDurability.ToString();
                durabilityDifference.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text =
                    (weaponItem.MaxDurability + upgradeableSelectedItem.AdditionalStatusByUpgrade[currentUpdateDegree + 1].Durability).ToString();
                
                GameObject attackPowerDifference = Instantiate(statusChangeShowPrefab, upgradeState.transform.GetChild(1));
                attackPowerDifference.GetComponent<TextMeshProUGUI>().text = statusLabel[1];
                attackPowerDifference.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    weaponItem.AttackPower.ToString();
                attackPowerDifference.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text =
                    (weaponItem.AttackPower + upgradeableSelectedItem.AdditionalStatusByUpgrade[currentUpdateDegree + 1].AttackPower).ToString();
                
                GameObject attackRange = Instantiate(statusNonChangeShowPrefab, upgradeState.transform.GetChild(1));
                attackRange.GetComponent<TextMeshProUGUI>().text = statusLabel[2];
                attackRange.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    weaponItem.AttackRange.ToString();
                
                GameObject attackSpeed = Instantiate(statusNonChangeShowPrefab, upgradeState.transform.GetChild(1));
                attackSpeed.GetComponent<TextMeshProUGUI>().text = statusLabel[3];
                attackSpeed.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    weaponItem.AttackSpeed.ToString();
                
                GameObject guardEfficiency = Instantiate(statusNonChangeShowPrefab, upgradeState.transform.GetChild(1));
                guardEfficiency.GetComponent<TextMeshProUGUI>().text = statusLabel[4];
                guardEfficiency.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    weaponItem.GuardEfficiency.ToString();
                
                //업그레이드를 진행하는 버튼 셋팅. 업그레이드 가격
                //대충 공식 == (해당아이템의 희귀도 + 1) * 300 * (현재업그레이드차수+1)
                //아이템의 희귀도와 현재 업그레이드차수가 0일 가능성이 있어서.
                //ex)common, 0강 아이템강화시, (0 + 1) * 300 * (0 + 1) = 300
                TextMeshProUGUI amount = upgradeButton.transform.GetChild(0).GetChild(0).gameObject
                    .GetComponent<TextMeshProUGUI>();
                amount.text = (((int)weaponItem.Rarity+1) * 300 * (currentUpdateDegree + 1)).ToString();
            }
            /*else if (item is EquipItem_Armor)
            {
                upgradeableItemInfo.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                    "Enforce Durability, Defence";
            }
            else if (item is KeyItem)
            {
                upgradeableItemInfo.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                    "Add SkillPoint";
            }*/
        }
        else if (selectedItem is Liminex liminex)
        {
            //여기가 재료 셋팅인데..
            //만약 이미 텍스트공간이 생성되어있으면
            int destroyObjectCount = materialsSpace.transform.childCount;
            if (destroyObjectCount != 0)
            {
                for (int i = 0; i < destroyObjectCount; i++)
                {
                    DestroyImmediate(materialsSpace.transform.GetChild(0).gameObject);
                }
            }
            
            //필요한 재료 아이템의 데이터 셋팅(개수, 이름 등)
            foreach (ItemData_SO materials in liminex.UpgradeMaterialsAndNeedCount.Keys)
            {
                upgradeableItemMaterials[materials] = liminex.UpgradeMaterialsAndNeedCount[materials];
                GameObject materialShow = Instantiate(materialShowPrefab, materialsSpace.transform);
                materialShow.GetComponent<TextMeshProUGUI>().text = materials.ItemName;
                materialShow.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    upgradeableItemMaterials[materials].ToString();
            }
            
            Transform upgradeDegreeInfoWindow = upgradeState.transform.GetChild(0);
            //업그레이드 정보(아이템이름, 차수)
            upgradeDegreeInfoWindow.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                selectedItem.data.ItemName;
            int currentUpdateDegree = liminex.CurrentUpgradeCount;
            upgradeDegreeInfoWindow.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text =
                currentUpdateDegree.ToString();
            upgradeDegreeInfoWindow.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                (currentUpdateDegree + 1).ToString();
            
            if (!upgradeState.transform.GetChild(0).gameObject.activeInHierarchy)
            {
                upgradeState.transform.GetChild(0).gameObject.SetActive(true);
            }

            string statusLabel = "SkillPoint";
            
            destroyObjectCount = upgradeState.transform.GetChild(1).childCount;
            if (destroyObjectCount != 0)
            {
                for (int i = 0; i < destroyObjectCount; i++)
                {
                    DestroyImmediate(upgradeState.transform.GetChild(1).GetChild(0).gameObject);
                }
            }
            //3번째 자식오브젝트는 단순 화살표
            
            GameObject skillPointDifference = Instantiate(statusChangeShowPrefab, upgradeState.transform.GetChild(1));
            skillPointDifference.GetComponent<TextMeshProUGUI>().text = statusLabel;
            skillPointDifference.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                liminex.CurrentUpgradeCount.ToString();
            skillPointDifference.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text =
                (liminex.CurrentUpgradeCount + 1).ToString();
            
            TextMeshProUGUI amount = upgradeButton.transform.GetChild(0).GetChild(0).gameObject
                .GetComponent<TextMeshProUGUI>();
            amount.text = (800 * (currentUpdateDegree + 1)).ToString();
        }
    }

    //누르면 업그레이드함
    public void UpgradeButtonOnclick()
    {
        foreach (ItemData_SO itemData in upgradeableItemMaterials.Keys)
        {
            //체크아이템(아이템이 존재하고, 수량이 충분하면) 결과 트루이면
            if (UiManager.Inventory.CheckItem(itemData.ItemName, upgradeableItemMaterials[itemData]))
            {
                //필요한 개수만큼
                for (int i = 0; i < upgradeableItemMaterials[itemData]; i++)
                {
                    //꺼내서
                    Item_SO material = UiManager.Inventory.FindAndReturnItem(itemData.ItemName);
                    //넣는다.
                    requestedItemToUpgrade.Add(material);
                }
            }
            else
            {
                if (requestedItemToUpgrade.Count > 0)
                {
                    foreach (Item_SO item in requestedItemToUpgrade)
                    {
                        UiManager.Inventory.PublicItemGotoInventory(item);
                    }
                    requestedItemToUpgrade.Clear();
                }
                Debug.Log($"Not Enough Item:{itemData.ItemName}, {upgradeableItemMaterials[itemData]}");
                return;
            }
            
            //아이템 다 있으면 재화도 좀 체크해주세요.
            TextMeshProUGUI amount = upgradeButton.transform.GetChild(0).GetChild(0).gameObject
                .GetComponent<TextMeshProUGUI>();
            if (int.TryParse(amount.text, out int count))
            {
                bool enough = UiManager.Inventory.Credit >= count;
                if (!enough)
                {
                    requestedItemToUpgrade.Clear();
                    Debug.Log("Not Enough Credit");
                    return;
                }
                costToUpgrade = count;
            }
            else
            {
                Debug.Log("char input");
                return;
            }
        }

        //여기까지오면 업그레이드하기
        if (selectedItem is EquipItem_Weapon weapon)
        {
            upgradeableItemMaterials.Clear();
            //스택내의 스테이터스변화량을 토대로 능력치를 조정한다. 강화차수, 능력치셋팅 등
            weapon.SetUpgradeState();
            for (int i = 0; i < requestedItemToUpgrade.Count; i++)
            {
                ItemManager_SO.Instance.PickUpItem(requestedItemToUpgrade[0]);
                requestedItemToUpgrade.Remove(requestedItemToUpgrade[0]);
            }
            UiManager.Inventory.CalculateCredit(-costToUpgrade);
            costToUpgrade = 0;

            //강화한 아이템의 강화차수와 재료아이템 다시 갱신하기
            showedUpgradeableItemList.Clear();
            ShowUpgradeable();
            if (selectedItem != null)
            {
                UpgradeableOnclick(revertToButton[selectedItem]);    
            }
        }
        else if (selectedItem is Liminex liminex)
        {
            upgradeableItemMaterials.Clear();
            //스택내의 스테이터스변화량을 토대로 능력치를 조정한다. 강화차수, 능력치셋팅 등
            liminex.SetUpgradeState();
            for (int i = 0; i < requestedItemToUpgrade.Count; i++)
            {
                ItemManager_SO.Instance.PickUpItem(requestedItemToUpgrade[0]);
                requestedItemToUpgrade.Remove(requestedItemToUpgrade[0]);
            }
            UiManager.Inventory.CalculateCredit(-costToUpgrade);
            costToUpgrade = 0;

            //강화한 아이템의 강화차수와 재료아이템 다시 갱신하기
            showedUpgradeableItemList.Clear();
            ShowUpgradeable();
            if (selectedItem != null)
            {
                UpgradeableOnclick(revertToButton[selectedItem]);    
            }
        }
    }
}
