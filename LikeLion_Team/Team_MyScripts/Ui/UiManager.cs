using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UiManager : MonoBehaviour
{
    
    [SerializeField] private PlayerMoveController player;
    public PlayerMoveController Player => player;
    
    public static UiManager Instance { get; private set; }
    
    [SerializeField] private GameObject dialogBox; 
    public GameObject DialogBox => dialogBox;
    
    [SerializeField] private GameObject dialog; 
    public GameObject Dialog => dialog; 
    public static Inventory Inventory;
     
     
    [SerializeField] private GameObject functionSelectWindow;
    public GameObject FunctionSelectWindow => functionSelectWindow;


    public Dictionary<int, GameObject> UiDictionary { get; private set; }
    public List<GameObject> uiTab { get; private set; }
    public List<GameObject> uiTabContents { get; private set; }

    public bool PlayerContact { get; set; }
    
    //모든 뭐야, NPCFunction 만든 이후.
    [SerializeField] private Slider playerHp;
    [SerializeField] private Slider playerStamina;
    [SerializeField] private Slider playerShield;
    [SerializeField] private Image shieldFillImage;
    [SerializeField] private Slider playerInfection;

    [SerializeField] private GameObject amo;
    [SerializeField] private TextMeshProUGUI currentAmo;
    [SerializeField] private TextMeshProUGUI maxAmo;
    
    public GameObject Amo => amo;
    public TextMeshProUGUI CurrentAmo => currentAmo;
    public TextMeshProUGUI MaxAmo => maxAmo;

    //게임 재시작 끝내기 등
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button ExitButton;
    
    public Slider PlayerInfection
    {
        get => playerInfection;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            UiDictionary = new Dictionary<int, GameObject>();
            
            uiTab = new List<GameObject>();
            uiTabContents = new List<GameObject>();
            
            int index = 0;
            foreach (Transform child in transform)
            {
                UiDictionary[index] = child.gameObject;
                //Debug.Log(UiDictionary[index].name);
                index++;
            }
            /*딕셔너리 정보
             0: 상호작용 키
             1: 캐릭터의 건강상태
             2: 능력치, 소지품, 맵, 퀘스트의 인터페이스 탭. 총 4
             3: NPC를 포함한 캐릭터들 대사창 
             4: 기능NPC의 사용할 기능 선택 창
             5: 장착 및 소모품 아이템 창
             */
            GameObject tab = UiDictionary[2].transform.GetChild(0).gameObject;
            GameObject tabContents = UiDictionary[2].transform.GetChild(1).gameObject;
            //초기 인벤토리 칸을 할당하기
            //인터페이스탭(UiDictionary[2].transform.GetChild(1))의 1번째 자식이 갖고있는 자식오브젝트의 개수 == 4
            for (int i = 0; i < tabContents.transform.childCount; i++)
            {
                uiTab.Add(tab.transform.GetChild(i).gameObject);
                //Debug.Log(tabContents.transform.GetChild(i).gameObject.name);
                //인터페이스탭([2]). 1번째 자식(탭컨텐츠).자식오브젝트(status 등).자식오브젝트(뷰포트).자식오브젝트(컨텐츠)
                uiTabContents.Add(tabContents.transform.GetChild(i).gameObject);
                
                //탭 컨텐츠 디버그. 골든정답
                //Debug.Log(uiTabContents[i].gameObject.name);
            }
            /* 리스트정보
             * UiTab은 버튼을 들고있음. UiTabContents는 각각의 Contents들고있음. 두 리스트의 내부 순서는 아래와 같음
             * 0: WeaponStatus
             * 1: inventory
             * 2: Quest
             * 3: Map
             */
            //Debug.Log($"inventoryTotalSpace: {uiTabContents[1].transform.GetChild(0).childCount}");
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Inventory == null)
        {
            Inventory = Inventory.Instance;
        }
        
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
        gameObject.transform.GetChild(4).gameObject.SetActive(false);
        UiDictionary[2].gameObject.SetActive(false);
        
        
        SetPlayerHp();
        SetPlayerStamina();
        StartCoroutine(BeInfested());
        StartCoroutine(ShowCurrStamina());
        
        Amo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (NPCFunctionManager.Instance.gameObject.activeInHierarchy)
            {
                if (UiDictionary[2].gameObject.activeInHierarchy)
                {
                    //Debug.Log("OFF");
                    UiDictionary[2].gameObject.SetActive(false);
                    player.gameObject.GetComponent<CursorState>().SetCursor(true, false);
                    AudioManager.Instance.ClickButtonOffWindow();
                    return;
                }
                return;
            }
            if (UiDictionary[2].gameObject.activeInHierarchy)
            {
                //Debug.Log("OFF");
                if (Inventory.Instance.ItemSimpleView.activeInHierarchy)
                {
                    Inventory.Instance.ItemSimpleView.SetActive(false);
                }

                if (Inventory.Instance.ItemDetailView.activeInHierarchy)
                {
                    Inventory.Instance.ItemDetailView.SetActive(false);
                }
                UiDictionary[2].gameObject.SetActive(false);
                player.gameObject.GetComponent<CursorState>().SetCursor(true, false);
                AudioManager.Instance.ClickButtonOffWindow();
                return;
            }
            UiDictionary[2].gameObject.SetActive(true);
            player.gameObject.GetComponent<CursorState>().SetCursor(false, true);
            AudioManager.Instance.ClickButtonOnWindow();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Color color = EquippedItemUi.Instance.gameObject.GetComponent<Image>().color;
            
            if (color.a != 0)//투명도가 0이 아니면 == 꺼야함
            {
                color.a = 0;
                EquippedItemUi.Instance.gameObject.GetComponent<Image>().color =
                    new Color(color.r, color.g, color.b, color.a);
                for (int i = 0; i < EquippedItemUi.Instance.gameObject.transform.childCount; i++)
                {
                    EquippedItemUi.Instance.gameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
                EquippedItemUi.Instance.gameObject.GetComponent<Image>().raycastTarget = false;
                /*EquippedItemUi.Instance.gameObject.SetActive(false);*/
                player.gameObject.GetComponent<CursorState>().SetCursor(true, false);
                AudioManager.Instance.ClickButtonOffWindow();
            }
            else//투명도가 0이면 == 켜야함
            {
                if (!EquippedItemUi.Instance.gameObject.activeInHierarchy)
                {
                    EquippedItemUi.Instance.gameObject.SetActive(true);    
                }
                color.a = 255;
                EquippedItemUi.Instance.gameObject.GetComponent<Image>().color =
                    new Color(color.r, color.g, color.b, color.a);
                for (int i = 0; i < EquippedItemUi.Instance.gameObject.transform.childCount; i++)
                {
                    EquippedItemUi.Instance.gameObject.transform.GetChild(i).gameObject.SetActive(true);
                }
                EquippedItemUi.Instance.gameObject.GetComponent<Image>().raycastTarget = true;
                player.gameObject.GetComponent<CursorState>().SetCursor(false, true);
                AudioManager.Instance.ClickButtonOnWindow();
                EquippedItemUi.Instance.UpdateEquippedItemUI();
            }
            
        }
    }

    public void OnFunctionSelectWindow()
    {
        if (!Instance.functionSelectWindow.activeInHierarchy)
        {
            //Debug.Log("functionOn");
            Instance.functionSelectWindow.SetActive(true);
        }
    }

    public void OffFunctionSelectWindow()
    {
        if (Instance.functionSelectWindow.activeInHierarchy)
        {
            Instance.functionSelectWindow.SetActive(false);
            return;
        }
        Instance.functionSelectWindow.SetActive(false);
    }

    public void SetPlayerHp()
    {
        //hp 설정
        if (Player is IHealthpower hp)
        {
            //Debug.Log($"{Player.gameObject.name} has hp");
            playerHp.value = (float)hp.CurrentHealthPower/100;
            //쉴드가 있다면?
            if (Player.IsGetBarrier)
            {
                //Debug.Log($"{Player.gameObject.name} has barrier");
                
                BarrierPrefabScript barrier = FindFirstObjectByType<BarrierPrefabScript>();
                playerShield.gameObject.SetActive(true);

                SetPlayerShield(barrier.CurrentHealthPower);
            }
            else
            {
                playerShield.gameObject.SetActive(false);
            }
        }
    }
    public void SetPlayerStamina()
    {
        playerStamina.value = (float)Player.CurrStamina/10;
        Debug.Log(playerStamina.value);
    }
    
    public void SetPlayerShield(int currentShieldHp)
    {
        if (Player is IHealthpower hp)
        {
            int totalHp = hp.CurrentHealthPower + currentShieldHp;
            if (totalHp >= 100)
            {
                playerHp.value = (float)hp.CurrentHealthPower / totalHp;
                playerShield.value = (float)currentShieldHp / totalHp;    
            }
            else
            {
                playerHp.value = (float)hp.CurrentHealthPower/100;
                playerShield.value = (float)currentShieldHp/100;
            }
        }
    }
    public void SetPlayerInfection()
    {
        
    }

    public void TestInt()
    {
        //Debug.Log(playerHp.value);
    }

    private IEnumerator ShowCurrStamina()
    {
        while (!Player.IsDead)
        {
            playerStamina.value = (float)Player.CurrStamina/10;
            yield return null;
        }
    }
    
    private IEnumerator BeInfested()
    {
        while (playerInfection.value < 1)
        {
            playerInfection.value += 0.1f;
            yield return new WaitForSeconds(10f);
            if (playerInfection.value > 0.99f)
            {
                playerInfection.value -= 0.5f;
            }
        }
    }
    
    public void ReloadScene()
    {
        // 현재 활성화된 씬의 이름을 가져와서 다시 로드합니다.
        Scene currentScene = SceneManager.GetActiveScene();
        //Inventory.Instance.Reload();
        
        Time.timeScale = 1f;
        RestartButton.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(false);
        SceneManager.LoadScene(currentScene.name);
        //EquippedItemUi.Instance.Reload();
    }

    public void EndGame()
    {
        Debug.Log("GameEndClick");
    }

    public void GameOver()
    {
        if (Player.IsDead)
        {
            //화면정지
            Time.timeScale = 0f;
            //점수창과 리스타트, 끝내기버튼 출력
            ExitButton.gameObject.SetActive(true);
            RestartButton.gameObject.SetActive(true);
            Instance.Player.gameObject.GetComponent<CursorState>().SetCursor(false, true);
            //Debug.Log("game over");
        }
    }
}
