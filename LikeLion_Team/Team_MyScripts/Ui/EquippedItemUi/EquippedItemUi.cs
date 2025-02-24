using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class EquippedItemUi : MonoBehaviour
{
    public static EquippedItemUi Instance { get; private set; }

    [SerializeField] private RWContoller RWController;
    [SerializeField] private MWController MWController;
    [SerializeField] private GameObject ConsumableHand;

    /// <summary>
    /// 0~1은 원거리무기 - 라이플, 샷건, 2~4는 근접무기 - 칼, 도끼, 배트?, 5~7은 소모품 - 체력, 감염도, 스태미나
    /// </summary>
    [SerializeField] private List<GameObject> equippedItems;
    
    [SerializeField] private GameObject equippedItemsHand;
    
    //로직용
    private List<Stack<Item_SO>> equippedItemList;
    private Dictionary<Button, Stack<Item_SO>> stackByButton;
    private Dictionary<GameObject, Stack<Item_SO>> dataByObject;
    private Animator handAnimator;

    public Item_SO BeforeWeaponItem { get; private set; }
    public Item_SO CurrentWeaponItem { get; private set; }
    public int CurrentWeaponItemCode { get; private set; }
    public Item_SO SelectedConsumableItem { get; private set; }
    public bool UseConsumable { get; private set; }
    private GameObject currentWeaponGameObejct;
    
    private static readonly int Health = Animator.StringToHash("Health");
    private static readonly int Infection = Animator.StringToHash("Infection");
    private static readonly int Stamina = Animator.StringToHash("Stamina");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            equippedItemList = new List<Stack<Item_SO>>();
            stackByButton = new Dictionary<Button, Stack<Item_SO>>();
            dataByObject = new Dictionary<GameObject, Stack<Item_SO>>();
            handAnimator = equippedItemsHand.GetComponent<Animator>();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateEquippedItemUI();
        for (int i = 0; i < equippedItems.Count; i++)
        {
            equippedItems[0].gameObject.SetActive(false);
        }
        ConsumableHand.SetActive(false);
        Color color = Instance.gameObject.GetComponent<Image>().color;

        if (color.a != 0) //투명도가 0이 아니면 == 꺼야함
        {
            color.a = 0;
            Instance.gameObject.GetComponent<Image>().color =
                new Color(color.r, color.g, color.b, color.a);
            for (int i = 0; i < Instance.gameObject.transform.childCount; i++)
            {
                Instance.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }

            Instance.gameObject.GetComponent<Image>().raycastTarget = false;
        }
    }

    public void UpdateEquippedItemUI()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (equippedItemList.Count > 0)
        {
            ClearButton();
            equippedItemList.Clear();
        }
        
        Dictionary<Button, Stack<Item_SO>> equipments = Inventory.Instance.inventory_Equipments;
        
        foreach (KeyValuePair<Button, Stack<Item_SO>> pair in equipments)
        {
            int index = 0;
            Item_SO item = pair.Value.Peek();
            if (!item.data.ItemName.Equals(EnumItemCode.Blank.ToString()) && !item.data.ItemName.Equals(EnumItemCode.LastBlank.ToString()))//빈칸
            {
                //스택이 들어간다. 왜냐하면 소모품의 경우 그 수량을 소진시켜야하기 때문
                equippedItemList.Add(pair.Value);

                if (item is EquipItem_Weapon weapon)
                {
                    if (item.data.ItemCode.Equals(((int)EnumItemCode.AssaultRifle).ToString()))
                    {
                        equippedItems[0].gameObject.GetComponent<RangedWeapon>().SettingWeaponData(weapon);
                    }
                    else if (item.data.ItemCode.Equals(((int)EnumItemCode.ShotGun).ToString()))
                    {
                        equippedItems[1].gameObject.GetComponent<RangedWeapon>().SettingWeaponData(weapon);
                    }
                    else if (item.data.ItemCode.Equals(((int)EnumItemCode.Knife).ToString()))
                    {
                        equippedItems[2].gameObject.GetComponent<MeleeWeapon>().SettingWeaponData(weapon);
                    }
                    else if (item.data.ItemCode.Equals(((int)EnumItemCode.Axe).ToString()))
                    {
                        equippedItems[3].gameObject.GetComponent<MeleeWeapon>().SettingWeaponData(weapon);
                    }
                    else if (item.data.ItemCode.Equals(((int)EnumItemCode.Bat).ToString()))
                    {
                        equippedItems[4].gameObject.GetComponent<MeleeWeapon>().SettingWeaponData(weapon);
                    }
                    else
                    {
                        Debug.Log("Not defined Weapon or is not Weapon");
                    }
                }
                else if (item is EquipItem_Consumable consumable)
                {
                    if (item.data.ItemCode.Equals(((int)EnumItemCode.HealthRestoreSyringe).ToString()))
                    {
                        equippedItems[5].gameObject.GetComponent<Consumable>().SettingConsumableData(consumable);
                    }
                    else if (item.data.ItemCode.Equals(((int)EnumItemCode.InfectionRestoreSyringe).ToString()))
                    {
                        equippedItems[6].gameObject.GetComponent<Consumable>().SettingConsumableData(consumable);
                    }
                    else if (item.data.ItemCode.Equals(((int)EnumItemCode.EnergyDrink).ToString()))
                    {
                        equippedItems[7].gameObject.GetComponent<Consumable>().SettingConsumableData(consumable);
                    }
                    else
                    {
                        Debug.Log("Not defined Consumable or is not Consumable");
                    }
                }
            }

            index++;
            if (index > gameObject.transform.childCount)
            {
                Debug.Log(index);
                break;
            }
        }
        
        if (equippedItemList.Count < 1)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                ClearButton();
            }
            Debug.Log(equippedItemList.Count);
            Debug.Log("Update Complete");
            return;
        }
        
        for (int i = 0; i < equippedItemList.Count; i++)
        {
            //마찬가지로 경준님이 설정한 이미지에서
            Transform buttonBackGroundImage = gameObject.transform.GetChild(i);
            //첫번째 자식오브젝트가 버튼이고
            if (buttonBackGroundImage.GetChild(1).TryGetComponent(out Button button))
            {
                //버튼의 이미지를 리스트에 들어있는 스택에서 peek해서(item이에요) Icon을 할당해요 -> 아마 어설트라이플이면 경준님이 설정한 아이템이 나오겠죠
                //근데 문제는 우리가 버튼을 껐잖아요 그러면 이제 켜줘야죠
                //그쳐 근데 해제하려면 장착아이템창을 꺼야되잖아요 그러면 자동으로 클리어버튼 불러서 꺼요 근데 아마 지금 아이템창 켜놓은 상태에서 장착아이템창이 켜질거라 그것도 셋팅하긴 해야겠네요
                button.image.sprite = equippedItemList[i].Peek().Icon;
                if (button.transform.GetChild(0).gameObject.TryGetComponent(out TextMeshProUGUI amount))
                {
                    amount.text = (equippedItemList[i].Count - 1).ToString();
                }
                else
                {
                    Debug.Log("not text");
                    return;
                }
                button.gameObject.SetActive(true);
                stackByButton[button] = equippedItemList[i];
            }
            else
            {
                Debug.Log("Need to add button component");
                return;
            }
        }
        
        Debug.Log(equippedItemList.Count);
        Debug.Log("Update Complete");
    }

    private void ClearButton()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            //이제 얘는 경준님이 할당한 이미지에요
            Transform buttonBackGroundImage = gameObject.transform.GetChild(i);
            if (buttonBackGroundImage.GetChild(1).TryGetComponent(out Button button))
            {
                button.gameObject.SetActive(false);
                /*button.image.sprite = Resources.Load<Sprite>("Icons/Blank");
                if (buttonBackGroundImage.transform.GetChild(0).gameObject.TryGetComponent(out TextMeshProUGUI amount))
                {
                    amount.text = "";
                }
                else
                {
                    Debug.Log("not text");
                    return;
                }*/
            }
        }
        stackByButton.Clear();
    }

    public void UseEquippedItem(Button button)
    {
        if (stackByButton.Count < 1)
        {
            Debug.Log("list is empty");
            return;
        }
        if (!stackByButton.ContainsKey(button))
        {
            Debug.Log("not assigned");
            return;
        }
        Item_SO selectedItem = stackByButton[button].Peek();
        switch (selectedItem.data.ItemCategory)
        {
            case 0://빈칸
                Debug.Log($"{selectedItem.data.ItemCategory}");
                break;
            case 1://장비아이템
                if (UseConsumable || selectedItem.Equals(CurrentWeaponItem))
                {
                    return;
                }
                Debug.Log($"{selectedItem.data.ItemCategory}");
                if (selectedItem is EquipItem_Weapon)
                {
                    EquipWeaponItem(selectedItem);
                }
                break;
            case 2://소모품아이템
                if (UseConsumable)
                {
                    return;
                }
                Debug.Log(selectedItem.data.ItemCode);
                EquipItem_Consumable consumable = selectedItem as EquipItem_Consumable;
                UsingConsumableItem(consumable);
                
                //팔이 겹치지 않게 딜레이 코루틴 필요할듯
                StartCoroutine(HandDelay(consumable));
                /*//Debug.Log($"{selectedItem.data.ItemCategory}");
                //소모품아이템을 손에 드는 로직 - 수량 감소 o
                //손에 아이템을 들고있는가?
                //손이 비어있다면
                //selecteditem을 든다. 그 아이템의 수량을 1 감소시킨다.
                //장비아이템을 들고있다면
                //장비아이템을 넣고 selectedItem을 든다. 그 아이템의 수량을 1 감소시킨다.
                //소모품아이템을 들고있다면
                //들고있는 소모품을 넣고 그 아이템의 수량을 1 증가시킨다. selecteditem을 들고 그 아이템의 수량을 1 증가시킨다.*/
                //애니메이션이 재생되면, 각 아이템에 맞는 효과를 발동시킨다.
                
                break;
            default:
                Debug.Log("not defined category");
                break;
        }
    }

    public void EquipWeaponItem()
    {
        EquipWeaponItem(BeforeWeaponItem);
    }
    
    private void EquipWeaponItem(Item_SO selectedItem)
    {
        if (selectedItem == null)
        {
            return;
        }
        if (ConsumableHand.activeInHierarchy)
        {
            ConsumableHand.SetActive(false);    
        }
        if (selectedItem.data.ItemCode.Equals(((int)EnumItemCode.AssaultRifle).ToString()))
        {
            BeforeWeaponItem = CurrentWeaponItem;
            RWController.currentRWeapon = equippedItems[0].GetComponent<RangedWeapon>();
            SetWeaponFalseInScreen();
            Debug.Log(equippedItems[0]);
            equippedItems[0].SetActive(true);
            currentWeaponGameObejct = equippedItems[0];
            CurrentWeaponItem = selectedItem;
            CurrentWeaponItemCode = (int)EnumItemCode.AssaultRifle;
            UiManager.Instance.CurrentAmo.text = RWController.currentRWeapon.curMagAmmo.ToString();
            UiManager.Instance.MaxAmo.text = RWController.currentRWeapon.haveAmmo.ToString();
            UiManager.Instance.Amo.SetActive(true);
            if (RWController.currentRWeapon.curMagAmmo > 0)
            {
                RWController.IsEmpty(false);
            }
            else
            {
                RWController.IsEmpty(true);
            }
        }
        else if (selectedItem.data.ItemCode.Equals(((int)EnumItemCode.ShotGun).ToString()))
        {
            BeforeWeaponItem = CurrentWeaponItem;
            RWController.currentRWeapon = equippedItems[1].GetComponent<RangedWeapon>();
            SetWeaponFalseInScreen();
            equippedItems[1].SetActive(true);
            currentWeaponGameObejct = equippedItems[1];
            CurrentWeaponItem = selectedItem;
            CurrentWeaponItemCode = (int)EnumItemCode.ShotGun;
            UiManager.Instance.CurrentAmo.text = RWController.currentRWeapon.curMagAmmo.ToString();
            UiManager.Instance.MaxAmo.text = RWController.currentRWeapon.haveAmmo.ToString();
            UiManager.Instance.Amo.SetActive(true);
            if (RWController.currentRWeapon.curMagAmmo > 0)
            {
                RWController.IsEmpty(false);
            }
            else
            {
                RWController.IsEmpty(true);
            }
        }
        else if (selectedItem.data.ItemCode.Equals(((int)EnumItemCode.Knife).ToString()))
        {
            BeforeWeaponItem = CurrentWeaponItem;
            MWController.CurrentMWeapon = equippedItems[2].GetComponent<MeleeWeapon>();
            SetWeaponFalseInScreen();
            equippedItems[2].SetActive(true);
            currentWeaponGameObejct = equippedItems[2];
            CurrentWeaponItem = selectedItem;
            CurrentWeaponItemCode = (int)EnumItemCode.Knife;
            UiManager.Instance.Amo.SetActive(false);
        }
        else if (selectedItem.data.ItemCode.Equals(((int)EnumItemCode.Axe).ToString()))
        {
            BeforeWeaponItem = CurrentWeaponItem;
            MWController.CurrentMWeapon = equippedItems[3].GetComponent<MeleeWeapon>();
            SetWeaponFalseInScreen();
            equippedItems[3].SetActive(true);
            currentWeaponGameObejct = equippedItems[3];
            CurrentWeaponItem = selectedItem;
            CurrentWeaponItemCode = (int)EnumItemCode.Axe;
            UiManager.Instance.Amo.SetActive(false);
        }
        else if (selectedItem.data.ItemCode.Equals(((int)EnumItemCode.Bat).ToString()))
        {
            BeforeWeaponItem = CurrentWeaponItem;
            MWController.CurrentMWeapon = equippedItems[4].GetComponent<MeleeWeapon>();
            SetWeaponFalseInScreen();
            equippedItems[4].SetActive(true);
            currentWeaponGameObejct = equippedItems[4];
            CurrentWeaponItem = selectedItem;
            CurrentWeaponItemCode = (int)EnumItemCode.Bat;
            UiManager.Instance.Amo.SetActive(false);
        }
        else
        {
            Debug.Log("No Weapon");
        }
    }
    
    private void UsingConsumableItem(Item_SO selectedItem)
    {
        if (!ConsumableHand.activeInHierarchy)
        {
            ConsumableHand.SetActive(true);    
        }
        if (currentWeaponGameObejct != null)
        {
            currentWeaponGameObejct.SetActive(false);    
        }
        if (selectedItem is EquipItem_Consumable consumable)
        {
            if (selectedItem.data.ItemCode.Equals(((int)EnumItemCode.HealthRestoreSyringe).ToString()))
            {
                handAnimator.SetTrigger(Health);
                SelectedConsumableItem = consumable;
                UseConsumable = true;
            }
            else if (selectedItem.data.ItemCode.Equals(((int)EnumItemCode.InfectionRestoreSyringe).ToString()))
            {
                handAnimator.SetTrigger(Infection);
                SelectedConsumableItem = consumable;
                UseConsumable = true;
            }
            else if (selectedItem.data.ItemCode.Equals(((int)EnumItemCode.EnergyDrink).ToString()))
            {
                handAnimator.SetTrigger(Stamina);
                SelectedConsumableItem = consumable;
                UseConsumable = true;
            }
            else
            {
                Debug.Log("No Consumable");
            }
        }
    }
    
    private void SetWeaponFalseInScreen()
    {
        for (int i = 0; i < equippedItems.Count; i++)
        {
            if (equippedItems[i].activeInHierarchy)
            {
                equippedItems[i].SetActive(false);
            }
        }
    }

    private IEnumerator HandDelay(EquipItem_Consumable consumable)
    {
        Debug.Log("delay");
        if (SelectedConsumableItem.data.ItemCode.Equals(((int)EnumItemCode.EnergyDrink).ToString()))
        {
            yield return new WaitForSeconds(4.5f);
            consumable.Consume();
        }
        else
        {
            yield return new WaitForSeconds(2.5f);
            consumable.Consume();
        }

        SelectedConsumableItem = null;
        UseConsumable = false;
        ConsumableHand.SetActive(false);
        EquipWeaponItem(CurrentWeaponItem);
        Debug.Log("weapon turn on");
        //currentWeaponGameObejct.SetActive(true);
    }

    public void Reload()
    {
        Awake();
        Start();
    }
}
