using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Craft : MonoBehaviour
{
    public static Craft Instance;

    private bool initialized = false;
    
    //전체 제작창 할당
    private List<GameObject> craftWindows;

    //설계도 정보
    [SerializeField] private List<BlueprintData_SO> armBpDatabase;
    //장비아이템 능력치 정보
    [SerializeField] private List<WeaponStatus_SO> armDatabase;
    
    //설계도 정보
    [SerializeField] private List<BlueprintData_SO> materialBpDatabase;
    //재료아이템 정보
    [SerializeField] private List<ItemData_SO> materialDatabase;
    
    [SerializeField] private GameObject partSpace;
    [SerializeField] private GameObject inventorySetter;
    [SerializeField] private GameObject inventorySelector;

    public GameObject InventorySetter => inventorySetter;
    
    public List<Button> BlueprintButtonList { get; private set; }
    private GameObject bpList;
    private GameObject pageList;

    private Dictionary<int, List<BlueprintData_SO>> pageToDataListDictionary;
    private Dictionary<List<BlueprintData_SO>, int> dataListToPageDictionary;
    
    private Dictionary<Button, BlueprintData_SO> buttonToBpDictionary;
    private Dictionary<Button, Dictionary<Button, BlueprintData_SO>> buttonToButtonDictonary;

    private Dictionary<BlueprintData_SO, WeaponStatus_SO> blueprintToWeapon;
    private Dictionary<WeaponStatus_SO, EquipItem_Weapon> baseEquipItemDict;

    private List<BlueprintData_SO> currentPage;

    private EquipItem_Weapon CraftTargetWeapon;
    private ItemData_SO CraftTargetItemData;
    private List<Item_SO> craftedItem;

    private List<Item_SO> CraftPartList;
    private Dictionary<string, int> CraftPartCountDictionary;

    private Button SelectedCraftItemButton;
    private Dictionary<Button, EquipItem_Weapon> CraftedWeapon;

    private List<BlueprintData_SO> bluePrintDatabase;

    private Dictionary<BlueprintData_SO, ItemData_SO> blueprintToMaterialItem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            //Debug.Log(gameObject.transform.childCount);
            craftWindows = new List<GameObject>(gameObject.transform.childCount);
            BlueprintButtonList = new List<Button>();
            pageToDataListDictionary = new Dictionary<int, List<BlueprintData_SO>>();
            dataListToPageDictionary = new Dictionary<List<BlueprintData_SO>, int>();
            buttonToBpDictionary = new Dictionary<Button, BlueprintData_SO>();
            buttonToButtonDictonary = new Dictionary<Button, Dictionary<Button, BlueprintData_SO>>();
            blueprintToWeapon = new Dictionary<BlueprintData_SO, WeaponStatus_SO>();
            baseEquipItemDict = new Dictionary<WeaponStatus_SO, EquipItem_Weapon>();
            currentPage = new List<BlueprintData_SO>();
            CraftPartList = new List<Item_SO>();
            CraftPartCountDictionary = new Dictionary<string, int>();
            CraftedWeapon = new Dictionary<Button, EquipItem_Weapon>();
            craftedItem = new List<Item_SO>();

            bluePrintDatabase = new List<BlueprintData_SO>();
            blueprintToMaterialItem = new Dictionary<BlueprintData_SO, ItemData_SO>();

            for (int i = 0; i < armBpDatabase.Count; i++)
            {
                blueprintToWeapon[armBpDatabase[i]] = armDatabase[i];
                bluePrintDatabase.Add(armBpDatabase[i]);
            }

            for (int i = 0; i < materialBpDatabase.Count; i++)
            {
                blueprintToMaterialItem[materialBpDatabase[i]] = materialDatabase[i];
                bluePrintDatabase.Add(materialBpDatabase[i]);
            }
        
            for (int i = 0; i < craftWindows.Capacity; i++)
            {
                craftWindows.Add(gameObject.transform.GetChild(i).gameObject);
                /*
                 * 0. 아이템렌더러(아이템이미지, BP선택버튼, 드롭다운리스트(카테고라이즈, BP리스트, 페이지))
                 * 1. 능력치 프리뷰(EquipItem 쪽에서 가져옴)
                 * 2. 아이템창(보유한 장비아이템 창, 창고)
                 * 3. 제작에 필요한 부품들(블루프린트 클래스에 추가예정)
                 * 4. 제작버튼
                 * 5. 아이템 제작 딜레이용 화살표
                 * 6. 아이템렌더러와 동일(단, 이부분은 아마 등급에 따른 이펙트 추가 예정) 만들어진 아이템의 이미지가 나타남.
                 * 7. 제작이 완료된 아이템의 능력치
                 */
                //Debug.Log(craftWindows[i].gameObject.name);
            }
            //BP리스트(블루프린트버튼만있음)
            bpList = craftWindows[0].transform.GetChild(2).GetChild(1).gameObject;
            //페이지리스트(0은 왼쪽으로 넘기기버튼, 1은 오른쪽으로 넘기기버튼. 2는 현재페이지 위치)
            pageList = craftWindows[0].transform.GetChild(2).GetChild(2).gameObject;
            MakePageDictionary();
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        NPCFunctionManager.CraftWindow = Instance;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!initialized)
        {
            for (int i = 0; i < bluePrintDatabase.Count; i++)
            {
                Button button = bpList.transform.GetChild(i).GetComponent<Button>();
                button.onClick.AddListener(() => ShowTargetItem(button));
                BlueprintButtonList.Add(button);
            }

            for (int i = bluePrintDatabase.Count; i < bpList.transform.childCount; i++)
            {
                GameObject button = bpList.transform.GetChild(i).gameObject;
                button.SetActive(false);
            }
            //materialBpDatabase, materialDatabase
            initialized = true;
        }
    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/
    
    public void InitializeBpButtonList()
    {
        if (!craftWindows[0].transform.GetChild(2).gameObject.activeInHierarchy)
        {
            craftWindows[0].transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            craftWindows[0].transform.GetChild(2).gameObject.SetActive(false);
            return;
        }

        //현재 페이지가 아직 할당되지 않았다면
        if (currentPage.Count == 0)
        {
            //현재 페이지에 1페이지 저장
            currentPage = pageToDataListDictionary[1];
            //Debug.Log(currentPage[0].Code.ToString());
            for (int i = 0; i < BlueprintButtonList.Count; i++)
            {
                if (!BlueprintButtonList[i].gameObject.activeInHierarchy)
                {
                    BlueprintButtonList[i].gameObject.SetActive(true);    
                }
                if (BlueprintButtonList[i].transform.GetChild(0).gameObject.TryGetComponent(out TextMeshProUGUI label))
                {
                    //여기는 이제 설계도의 이름이 들어갈 부분
                    buttonToBpDictionary[BlueprintButtonList[i]] = currentPage[i];
                    buttonToButtonDictonary[BlueprintButtonList[i]] = buttonToBpDictionary;
                    label.text = buttonToBpDictionary[BlueprintButtonList[i]].ItemName;
                }
            }
            if (pageList.transform.GetChild(2).TryGetComponent(out TextMeshProUGUI page))
            {
                //페이지 개수를 동적으로 계산할 수 없을까?
                //BpDataBase의 count가 10이 될 때 마다 리스트로 빼서 저장한다. page넘버와 함께 - <페이지넘버, 블루프린트데이터리스트>
                page.text = dataListToPageDictionary[currentPage].ToString();
            }
        }
        else//만약 currentPage가 할당이 되었다면
        {
            //Debug.Log(currentPage[0].Code.ToString());
            for (int i = 0; i < BlueprintButtonList.Count; i++)
            {
                if (!BlueprintButtonList[i].gameObject.activeInHierarchy)
                {
                    BlueprintButtonList[i].gameObject.SetActive(true);    
                }
                if (BlueprintButtonList[i].transform.GetChild(0).gameObject.TryGetComponent(out TextMeshProUGUI label))
                {
                    if (i >= currentPage.Count)
                    {
                        BlueprintButtonList[i].gameObject.SetActive(false);
                        continue;
                    }
                    //여기는 이제 설계도의 이름이 들어갈 부분
                    buttonToBpDictionary[BlueprintButtonList[i]] = currentPage[i];
                    buttonToButtonDictonary[BlueprintButtonList[i]] = buttonToBpDictionary;
                    label.text = buttonToBpDictionary[BlueprintButtonList[i]].ItemName;
                }
            }
            if (pageList.transform.GetChild(2).TryGetComponent(out TextMeshProUGUI page))
            {
                //페이지 개수를 동적으로 계산할 수 없을까?
                //BpDataBase의 count가 10이 될 때 마다 리스트로 빼서 저장한다. page넘버와 함께 - <페이지넘버, 블루프린트데이터리스트>
                page.text = dataListToPageDictionary[currentPage].ToString();
            }
        }
    }

    //왼쪽으로 넘기는 버튼. 현재페이지 최신화 + 버튼 최신화. -> 어떻게? 딕셔너리 사용
    public void PagingLeft()
    {
        if (pageList.transform.GetChild(2).TryGetComponent(out TextMeshProUGUI page))
        {
            //페이지 개수를 동적으로 계산할 수 없을까?
            //BpDataBase의 count가 10이 될 때 마다 리스트로 빼서 저장한다. page넘버와 함께 - <페이지넘버, 블루프린트데이터리스트>
            if (int.TryParse(page.text, out int pageNumber))
            {
                int beforePageNumber = pageNumber - 1; 
                if (beforePageNumber <= 0)
                {
                    return;
                }
                //현재페이지 최신화
                page.text = beforePageNumber.ToString();
                
                //버튼최신화
                currentPage = pageToDataListDictionary[beforePageNumber];
                for (int i = 0; i < currentPage.Count; i++)
                {
                    if (!BlueprintButtonList[i].gameObject.activeInHierarchy)
                    {
                        BlueprintButtonList[i].gameObject.SetActive(true);    
                    }
                    if (BlueprintButtonList[i].transform.GetChild(0).gameObject.TryGetComponent(out TextMeshProUGUI label))
                    {
                        //여기는 이제 설계도의 이름이 들어갈 부분
                        buttonToBpDictionary[BlueprintButtonList[i]] = currentPage[i];
                        buttonToButtonDictonary[BlueprintButtonList[i]] = buttonToBpDictionary;
                        label.text = buttonToBpDictionary[BlueprintButtonList[i]].ItemName;
                    }
                }
            }
        }
        AudioManager.Instance.ClickButtonOnWindow();
    }

    //오른쪽으로 넘기는 버튼. 현재페이지 최신화 + 버튼 최신화.
    //++ 그... 만약 최신화되지 않았다면 그것은 어떻게 알지?
    public void PagingRight()
    {
        if (pageList.transform.GetChild(2).TryGetComponent(out TextMeshProUGUI page))
        {
            //페이지 개수를 동적으로 계산할 수 없을까?
            //BpDataBase의 count가 10이 될 때 마다 리스트로 빼서 저장한다. page넘버와 함께 - <페이지넘버, 블루프린트데이터리스트>
            if (int.TryParse(page.text, out int pageNumber))
            {
                int nextPage = pageNumber + 1;
                if (nextPage > pageToDataListDictionary.Count)
                {
                    return;
                }
                //현재페이지 최신화
                page.text = (nextPage).ToString();
                
                //버튼최신화
                currentPage = pageToDataListDictionary[nextPage];
                //Debug.Log(list[0].Code.ToString());
                for (int i = 0; i < BlueprintButtonList.Count; i++)
                {
                    if (BlueprintButtonList[i].transform.GetChild(0).gameObject.TryGetComponent(out TextMeshProUGUI label))
                    {
                        if (i >= currentPage.Count)
                        {
                            BlueprintButtonList[i].gameObject.SetActive(false);
                            continue;
                        }
                        //여기는 이제 설계도의 이름이 들어갈 부분
                        buttonToBpDictionary[BlueprintButtonList[i]] = currentPage[i];
                        buttonToButtonDictonary[BlueprintButtonList[i]] = buttonToBpDictionary;
                        label.text = buttonToBpDictionary[BlueprintButtonList[i]].ItemName;
                    }
                }
            }
        }
        AudioManager.Instance.ClickButtonOnWindow();
        //ButtonChecker();
    }
    
    //버튼을 누르면 해당 버튼이 갖고있는 이름의 설계도를 찾아야함.
    private void ShowTargetItem(Button button)
    {
        if (buttonToButtonDictonary.TryGetValue(button, out var targetDict))
        {
            //Debug.Log(craftWindows[0].transform.GetChild(1).gameObject.name);
            if (craftWindows[0].transform.GetChild(1).GetChild(0).gameObject.TryGetComponent(out TextMeshProUGUI label))
            {
                //블루프린트의 코드를 여기서 사용 -> 아이템 이름으로 변경
                label.text = targetDict[button].ItemName;
            }
            if (craftWindows[0].transform.GetChild(0).gameObject.TryGetComponent(out Image image))
            {
                //여기서 이미지 뽑아오기
                image.sprite = targetDict[button].BpImage; 
            }
            
        }

        StatusPreview(button);
        ShowPartsAndPartsCount(button);
        
        if (craftWindows[0].transform.GetChild(2).gameObject.activeInHierarchy)
        {
            craftWindows[0].transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            craftWindows[0].transform.GetChild(2).gameObject.SetActive(true);
        }

        SelectedCraftItemButton = button;
        AudioManager.Instance.ClickButtonOnWindow();
    } 

    //맨 마지막에 비어있는 칸들 정리. 나중에 켜야함. 잠깐만 이거 뭐더라.
    private void ButtonChecker()
    {
        for (int i = 0; i < BlueprintButtonList.Count; i++)
        {
            if (BlueprintButtonList[i].transform.GetChild(0).gameObject.TryGetComponent(out TextMeshProUGUI label))
            {
                //혹은 enum화 하기?
                if (label.text.Equals("Not acquired"))
                {
                    BlueprintButtonList[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
    private void MakePageDictionary()
    {
        //페이지번호
        int page = 0;
        //임시 리스트
        List<BlueprintData_SO> list = new List<BlueprintData_SO>();
        //DB크기만큼 돌면서
        for (int i = 0; i < bluePrintDatabase.Count; i++)
        {
            list.Add(bluePrintDatabase[i]);
            if (list.Count == 10)
            {
                page++;
                pageToDataListDictionary[page] = list;
                dataListToPageDictionary[list] = page;
                //Debug.Log($"{page}: {pageToDataListDictionary[page].Count}");
                //Debug.Log(list[0].Code.ToString());
                list = new List<BlueprintData_SO>();
                //list.clear(); 하면안되요
                /*코드에서 list.Clear()를 호출하는 부분에 문제가 있을 수 있습니다.
                list.Clear()는 리스트의 모든 요소를 제거하는데,
                이것을 사용하면 pageToDataListDictionary[page]와 dataListToPageDictionary[list]에 비어 있는 리스트가 저장될 수 있습니다.
                즉, 데이터가 없는 리스트가 딕셔너리에 저장되기 때문에 나중에 값을 불러올 때 문제가 발생할 수 있습니다.*/
            }
            if (i == bluePrintDatabase.Count-1)
            {
                page++;
                pageToDataListDictionary[page] = list;
                dataListToPageDictionary[list] = page;
                //Debug.Log($"{page}: {pageToDataListDictionary[page].Count}");
            }
        }
    }

    //여기가 무기의 경우 스텟을, 일반 아이템의 경우 설명을, 소모품 아이템의 경우 제작되는 개수와 효과 보여주기
    /*각 설계도의 코드는 아이템 카테고리를 따라감.
    Blank,
    Equipment,
    ConsumableItem,//2
    CommonItem,
    KeyItem,//4
    BluePrint*/
    private void StatusPreview(Button button)
    {
        switch (buttonToBpDictionary[button].Code)
        {
            
            case 1://장비아이템의 경우
                CraftingEquipItemInfo(button);
                break;
            case 2://소모품아이템의 경우
                CraftingConsumableItemInfo(button);
                break;
            case 3://일반 == 재료아이템의 경우
                CraftingMaterialItemInfo(button);
                break;
            case 4://중요아이템의 경우
                CraftingKeyItemInfo(button);
                break;
            default://의도치않게 빈칸, 혹은 블루프린트가 들어온 경우
                Debug.Log($"{buttonToBpDictionary[button].Code} is not defined bpCode");
                break;
        }
        //Debug.Log($"{button.gameObject.name}: {blueprintToWeapon[buttonToBpDictionary[button]].MaxDurability}");
        //Debug.Log(craftWindows[1].transform.GetChild(0).GetChild(0).childCount);
        //재료아이템의 경우 여기서 에러남 ㅋㅋ
        
    }

    private void CraftingEquipItemInfo(Button button)
    {
        WeaponStatus_SO selectedWeapon = blueprintToWeapon[buttonToBpDictionary[button]];
        
        baseEquipItemDict[selectedWeapon] = 
            new EquipItem_Weapon(ItemPool_SO.Instance.ItemDataBase[selectedWeapon.ItemCode], selectedWeapon, EnumItemRarity.Legendary, false);
        
        CraftTargetWeapon = baseEquipItemDict[blueprintToWeapon[buttonToBpDictionary[button]]];

        Transform parentContent = craftWindows[1].transform.GetChild(0).GetChild(0);

        //Debug.Log(craftWindows[1].transform.GetChild(0).GetChild(0).gameObject.name);
        
        TextMeshProUGUI durability = parentContent.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI attackRange = parentContent.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI rarity = parentContent.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI guardEfficiency = parentContent.GetChild(3).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI attackPower = parentContent.GetChild(4).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI attackSpeed = parentContent.GetChild(5).GetComponent<TextMeshProUGUI>();
    
        CraftTargetWeapon.ShowStatusByRarity(CraftTargetWeapon.MinRarity);
        
        durability.text = $"MaxDurability: {CraftTargetWeapon.MaxDurability} ~ ";
        attackRange.text = $"AttackRange: {CraftTargetWeapon.AttackRange} ~ ";
        guardEfficiency.text = $"GuardEfficiency: {CraftTargetWeapon.GuardEfficiency} ~ ";
        attackPower.text = $"AttackPower: {CraftTargetWeapon.AttackPower} ~ ";
        attackSpeed.text = $"AttackSpeed: {CraftTargetWeapon.AttackSpeed} ~ ";
        
        CraftTargetWeapon.ShowStatusByRarity(CraftTargetWeapon.MaxRarity);
        
        durability.text += $"{CraftTargetWeapon.MaxDurability}";
        attackRange.text += $"{CraftTargetWeapon.AttackRange}";
        guardEfficiency.text += $"{CraftTargetWeapon.GuardEfficiency}";
        attackPower.text += $"{CraftTargetWeapon.AttackPower}";
        attackSpeed.text += $"{CraftTargetWeapon.AttackSpeed}";
        
        rarity.text = $"Rarity: {CraftTargetWeapon.MinRarity.ToString()} ~ " + $"{CraftTargetWeapon.MaxRarity.ToString()}";
        
        for (int i = 0; i < parentContent.childCount; i++)
        {
            parentContent.GetChild(i).gameObject.SetActive(true);
        }
        
    }

    private void CraftingMaterialItemInfo(Button button)
    {
        CraftTargetItemData = blueprintToMaterialItem[buttonToBpDictionary[button]];
        
        Transform parentContent = craftWindows[1].transform.GetChild(0).GetChild(0);
        
        TextMeshProUGUI createCount = parentContent.GetChild(0).GetComponent<TextMeshProUGUI>();
        createCount.text = $"Create Count: {CraftTargetItemData.DropQuantity}";
        
        for (int i = 1; i < parentContent.childCount; i++)
        {
            parentContent.GetChild(i).gameObject.SetActive(false);
        }
    }
    private void CraftingConsumableItemInfo(Button button)
    {
        
    }
    private void CraftingKeyItemInfo(Button button)
    {
        
    }

    private void ShowPartsAndPartsCount(Button button)
    {
        //파츠-뷰포트-컨텐트
        GameObject parentContent = craftWindows[3].transform.GetChild(1).GetChild(0).gameObject;
        
        //생성된 part오브젝트가 있다면
        if (parentContent.transform.childCount > 0)
        {
            int childCount = parentContent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(parentContent.transform.GetChild(0).gameObject);
            }
        }
        
        for (int i = 0; i < buttonToBpDictionary[button].PartsCount.Count; i++)
        {
            GameObject space = Instantiate(partSpace, parentContent.transform);
            TextMeshProUGUI part = space.GetComponent<TextMeshProUGUI>();
            part.text = $"{buttonToBpDictionary[button].Parts[i].ItemName}";
            part.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{buttonToBpDictionary[button].PartsCount[i]}";
                
        }
        TextMeshProUGUI cost = Instantiate(partSpace, parentContent.transform).GetComponent<TextMeshProUGUI>();
        cost.text = "$";
        cost.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{buttonToBpDictionary[button].Cost}"; 

        /*if (parentContent.transform.childCount > 0)
        {
            //생성된 텍스트오브젝트의 숫자와 블루프린트가 요구하는 아이템종류의 숫자와 비교한다.
            if (parentContent.transform.childCount-1 > buttonToBpDictionary[button].Parts.Count)//크면
            {
                //텍스트오브젝트의 숫자만큼 반복한다.
                for (int i = 0; i < parentContent.transform.childCount-1; i++)
                {
                    //i가 아이템종류 숫자의 개수보다 적은동안.
                    if (i <= buttonToBpDictionary[button].Parts.Count)
                    {
                        //업데이트한다.
                        TextMeshProUGUI part = parentContent.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
                        part.text = $"{buttonToBpDictionary[button].Parts[i].ItemName}";
                        part.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{buttonToBpDictionary[button].PartsCount[i]}";
                        
                    }//만약 커지면
                    else
                    {
                        //텍스트를 끈다.
                        parentContent.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else//작으면
            {
                //그 차이만큼 텍스트공간 만들고
                for (int i = 0; i < buttonToBpDictionary[button].Parts.Count - parentContent.transform.childCount-1; i++)
                {
                    Instantiate(partSpace, parentContent.transform);
                }
                //만들어진 공간만큼을 추가로 확인해서 업데이트한다.
                for (int i = 0; i < parentContent.transform.childCount-1; i++)
                {
                    TextMeshProUGUI part = parentContent.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
                    part.text = $"{buttonToBpDictionary[button].Parts[i].ItemName}";
                    part.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{buttonToBpDictionary[button].PartsCount[i]}";
                }
            }
        }
        else//0이하이다. == 최초생성이다.
        {
           
        }*/
        //Content
        //Debug.Log(craftWindows[3].transform.GetChild(1).GetChild(0).gameObject.name);
        //partSpace는 그냥 TextMeshProUGUI
        /*if (parentContent.transform.childCount < buttonToBpDictionary[button].PartsCount.Count)
        {
            TextMeshProUGUI cost = Instantiate(partSpace, parentContent.transform).GetComponent<TextMeshProUGUI>();
            cost.text = "$";
            cost.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{buttonToBpDictionary[button].Cost}";
        }
        else
        {
            TextMeshProUGUI cost = parentContent.transform.GetChild(parentContent.transform.childCount - 1)
                .GetComponent<TextMeshProUGUI>();
            cost.text = "$";
            cost.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{buttonToBpDictionary[button].Cost}";
        }*/
    }
    
    //크래프튼 버튼을 눌러요
    //화살표가 차올라요
    public void StartCraft()
    {
        if (CraftTargetWeapon != null)
        {
            if (IsCraftingPartLack()) return;
            
            //크래프팅애로우마스크
            /*Debug.Log(craftWindows[5].gameObject.name);*/
            //여기는 애로우 채우면서 템만들기
            Image fill = craftWindows[5].transform.GetChild(0).gameObject.GetComponent<Image>();
            StartCoroutine(FillCraftArrowImageWithCraftingWeapon(fill, 0.02f));
        }

        if (CraftTargetItemData != null)
        {
            if (IsCraftingPartLack()) return;
            
            Image fill = craftWindows[5].transform.GetChild(0).gameObject.GetComponent<Image>();
            StartCoroutine(FillCraftArrowImageWithCraftingItem(fill, 0.02f));
        }
    }

    private bool IsCraftingPartLack()
    {
        Transform parentContent = craftWindows[3].transform.GetChild(1).GetChild(0);
             
        int craftCost = 0;
            
        for (int i = 0; i < parentContent.childCount; i++)//재화는 따로 체크
        {
            TextMeshProUGUI targetItemPartsInfo = parentContent.GetChild(i).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI targetItemPartsCountInfo = targetItemPartsInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                
            if (i < parentContent.childCount-1 )
            {
                //이 아이템들이 필요하네요.
                //Debug.Log($"{targetItemPartsInfo.text}: {targetItemPartsCountInfo.text}");
                if (int.TryParse(targetItemPartsCountInfo.text, out int partsCount))
                {
                    //있나 확인좀
                    if (UiManager.Inventory.CheckItem(targetItemPartsInfo.text, partsCount))
                    {
                        CraftPartCountDictionary[targetItemPartsInfo.text] = partsCount;
                        //있어요
                    }
                    else
                    {
                        //없는아이템이 존재해요
                        CraftPartCountDictionary.Clear();
                        Debug.Log("Lack item");
                        return true;
                    }
                }
            }
            else
            {
                //아이템 다 있으면 재화도 좀 체크해주세요.
                if (int.TryParse(targetItemPartsCountInfo.text, out int count))
                {
                    bool enough = UiManager.Inventory.Credit >= count;
                    if (!enough)
                    {
                        CraftPartCountDictionary.Clear();
                        //Debug.Log("Lack Credit");
                        return true;
                    }
                    craftCost = count;
                }
                else
                {
                    Debug.Log("char input");
                    return true;
                }
            }
        }
        
        //여기까지 오면 이제 그거지. 필요한 아이템들이 있고 수량도 충분하다. 재화도 충분하다.
        //딕셔너리에 넣은 정보를 토대로
        foreach (KeyValuePair<string, int> pair in CraftPartCountDictionary)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                CraftPartList.Add(UiManager.Inventory.FindAndReturnItem(pair.Key));
            }
        }

        int listContentCount = CraftPartList.Count;
        for (int i = 0; i < listContentCount; i++)
        {
            ItemManager_SO.Instance.PickUpItem(CraftPartList[i]);
            //Debug.Log(CraftPartList[i].data.ItemName);
        }
        UiManager.Inventory.CalculateCredit(-craftCost);
        CraftPartList.Clear();
        CraftPartCountDictionary.Clear();
        //디버그용코드
        Debug.Log(CraftPartList.Count);
        return false;
    }
    
    /// <summary>
    /// Image Fill
    /// </summary>
    /// <param name="fillImage">Image you want to fill. must FilledType.</param>
    /// <param name="fillDegree">degree how fast you want</param>
    /// <returns></returns>
    private IEnumerator FillCraftArrowImageWithCraftingWeapon(Image fillImage, float fillDegree)
    {
        AudioManager.Instance.ClickCraft();
        while (fillImage.fillAmount < 1f)
        {
            fillImage.fillAmount += fillDegree;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.5f);
        if (fillImage.fillAmount >= 1f)
        {
            fillImage.fillAmount = 0.0f;
        }
        
        if (craftWindows[6].transform.GetChild(0).TryGetComponent(out Image image2))
        {
            WeaponStatus_SO selectedWeapon = blueprintToWeapon[buttonToBpDictionary[SelectedCraftItemButton]];
            int randInt = Random.Range(0, (int)EnumItemRarity.Legendary + 1);
            Button button = craftWindows[6].transform.GetChild(0).gameObject.GetComponent<Button>();
            button.onClick.AddListener(() => GetCraftedWeapon(button));
            
            CraftedWeapon[button] = 
                new EquipItem_Weapon(ItemPool_SO.Instance.ItemDataBase[selectedWeapon.ItemCode], selectedWeapon, (EnumItemRarity)randInt, true);
            
            image2.sprite = CraftedWeapon[button].Icon;
            
            //Debug.Log(craftWindows[7].transform.GetChild(0).gameObject.name);
            Transform parentContent = craftWindows[7].transform.GetChild(0).GetChild(0);
            
            TextMeshProUGUI durability = parentContent.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI attackRange = parentContent.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI guardEfficiency = parentContent.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI attackPower = parentContent.GetChild(3).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI attackSpeed = parentContent.GetChild(4).GetComponent<TextMeshProUGUI>();
            
            List<int> statusList = new List<int>(parentContent.childCount)
            {
                CraftedWeapon[button].MaxDurability,
                CraftedWeapon[button].AttackRange,
                CraftedWeapon[button].GuardEfficiency,
                CraftedWeapon[button].AttackPower,
                CraftedWeapon[button].AttackSpeed
            };

            durability.text = $"MaxDurability: {statusList[0]}";
            attackRange.text = $"AttackRange: {statusList[1]}";
            guardEfficiency.text = $"GuardEfficiency: {statusList[2]}";
            attackPower.text = $"AttackPower: {statusList[3]}";
            attackSpeed.text = $"AttackSpeed: {statusList[4]}";
        }
    }

    private IEnumerator FillCraftArrowImageWithCraftingItem(Image fillImage, float fillDegree)
    {
        while (fillImage.fillAmount < 1f)
        {
            fillImage.fillAmount += fillDegree;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.5f);
        if (fillImage.fillAmount >= 1f)
        {
            fillImage.fillAmount = 0.0f;
        }

        if (craftWindows[6].transform.GetChild(0).TryGetComponent(out Image image2))
        {
            Button button = craftWindows[6].transform.GetChild(0).gameObject.GetComponent<Button>();
            button.onClick.AddListener(() => GetCraftedItem(button));
            int.TryParse(CraftTargetItemData.ItemCode, out int itemCode);

            for (int i = 0; i < CraftTargetItemData.DropQuantity; i++)
            {
                craftedItem.Add(ItemManager_SO.Instance.GetItem(itemCode));    
            }
            
            image2.sprite = craftedItem[0].Icon;
            
            Transform parentContent = craftWindows[7].transform.GetChild(0).GetChild(0);
        
            TextMeshProUGUI createCount = parentContent.GetChild(0).GetComponent<TextMeshProUGUI>();
            createCount.text = $"Create Count: {CraftTargetItemData.DropQuantity}";
        
            for (int i = 1; i < parentContent.childCount; i++)
            {
                parentContent.GetChild(i).gameObject.SetActive(false);
            }
        }


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

    public void ShowConsumableInventoryOnCraft()
    {
        UiManager.Inventory.MoveConsumableInventorySpace();
    }
    public void ShowItemInventoryOnCraft()
    {
        UiManager.Inventory.MoveItemsInventorySpace();
    }
    public void ShowKeyItemInventoryOnCraft()
    {
        UiManager.Inventory.MoveKeyItemsInventorySpace();
    }
    public void ShowStorageOnCraft()
    {
        Debug.Log("not yet");
    }

    private void GetCraftedWeapon(Button button)
    {
        if (CraftedWeapon.Count <= 0)
        {
            //Debug.Log("Not Crafted");
            return;
        }
        else
        {
            CraftedWeapon[button].ConsumesDurability(20);
            UiManager.Inventory.PublicItemGotoInventory(CraftedWeapon[button]);
            CraftedWeapon.Clear();
            button.onClick.RemoveAllListeners();
        }

        if (craftWindows[6].transform.GetChild(0).TryGetComponent(out Image image2))
        {
            image2.sprite = null;
            
            Transform parentContent = craftWindows[7].transform.GetChild(0).GetChild(0);

            for (int i = 0; i < parentContent.childCount; i++)
            {
                parentContent.GetChild(i).gameObject.SetActive(true);
            }
            
            TextMeshProUGUI durability = parentContent.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI attackRange = parentContent.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI guardEfficiency = parentContent.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI attackPower = parentContent.GetChild(3).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI attackSpeed = parentContent.GetChild(4).GetComponent<TextMeshProUGUI>();
            
            durability.text = "";
            attackRange.text = "";
            guardEfficiency.text = "";
            attackPower.text = "";
            attackSpeed.text = "";
        }
    }

    private void GetCraftedItem(Button button)
    {
        if (craftedItem == null)
        {
            return;
        }

        for (int i = 0; i < craftedItem.Count; i++)
        {
            UiManager.Inventory.PublicItemGotoInventory(craftedItem[0]);    
        }
        craftedItem.Clear();
        button.onClick.RemoveAllListeners();
        
        if (craftWindows[6].transform.GetChild(0).TryGetComponent(out Image image2))
        {
            image2.sprite = null;
            
            Transform parentContent = craftWindows[7].transform.GetChild(0).GetChild(0);
        
            for (int i = 0; i < parentContent.childCount; i++)
            {
                parentContent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        if (InventorySetter.transform.childCount > 0)
        {
            for (int i = 0; i < InventorySetter.transform.childCount; i++)
            {
                //옮겨졌던 인벤토리들을 현재 비어있는 곳에 넣고 
                UiManager.Inventory.ResetInventorySpace(InventorySetter.transform.GetChild(i).gameObject);
            }
        }
    }

    /*public void OnCraft()
    {
        if (!Instance.gameObject.activeInHierarchy)
        {
            Instance.gameObject.SetActive(true);    
        }
    }

    public void OffCraft()
    {
        if (Instance.gameObject.activeInHierarchy)
        {
            Instance.gameObject.SetActive(false);    
        }
    }*/
    
    //여기는 혹시모를 페이드인과 페이드아웃
    /*/// <summary>
    /// yield return value is 0.02f.
    /// </summary>
    /// <param name="image">Gameobject having image component what u want to control Alpha</param>
    /// <param name="maxAlphaValue">0~1, float</param>
    /// <param name="coefficient">increase Or decrease amount</param>
    /// <returns></returns>
    private IEnumerator FadeInAndFadeOut(GameObject image, float maxAlphaValue, float coefficient)
    {
        image.SetActive(true);
        if (image.TryGetComponent(out Image targetImage))
        {
            float maxAlpha = 0;
            while (maxAlpha <= maxAlphaValue)
            {
                maxAlpha += coefficient;
                Color colorAlpha = targetImage.color;
                colorAlpha.a = maxAlpha;
                targetImage.color = colorAlpha;//알파값을 변경한 컬러를 다시 할당해줘야합니다.
                yield return new WaitForSeconds(0.02f);
            }
            while (maxAlpha > 0)
            {
                maxAlpha -= coefficient;
                Color colorAlpha = targetImage.color;
                colorAlpha.a = maxAlpha;
                targetImage.color = colorAlpha;
                yield return new WaitForSeconds(0.02f);
            } 
        }
        image.SetActive(false);
    }

    private IEnumerator FadeIn(Image image, float maxAlphaValue, float coefficient)
    {
        float maxAlpha = 0;
        while (maxAlpha <= maxAlphaValue)
        {
            maxAlpha += coefficient;
            Color colorAlpha = image.color;
            colorAlpha.a = maxAlpha;
            image.color = colorAlpha;//알파값을 변경한 컬러를 다시 할당해줘야합니다.
            yield return new WaitForSeconds(0.02f);
        }
    }
*/
}
