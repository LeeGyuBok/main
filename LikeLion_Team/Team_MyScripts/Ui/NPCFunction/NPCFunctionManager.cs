using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCFunctionManager : MonoBehaviour
{
    public static NPCFunctionManager Instance { get; private set; }

    public static Craft CraftWindow;
    public static Repair RepairWindow;
    public static Upgrade UpgradeWindow;
    public static SkillFunction SkillFunctionWindow;

    /// <summary>
    /// 0 == Craft, 1 == Repair, 2 == Upgrade, 3 == skill, 4 == exit
    /// </summary>
    [SerializeField] private List<Button> functionTab;

    private Dictionary<Button, GameObject> tabTowindow;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            functionTab[4].onClick.AddListener(ExitFunctionWindow);
            
            

            tabTowindow = new Dictionary<Button, GameObject>
            {
                //얘는 탭버튼
                [functionTab[0]] = gameObject.transform.GetChild(0).gameObject,
                //얘는 컨텐츠
                [functionTab[1]] = gameObject.transform.GetChild(1).gameObject,
            };
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(CraftWindow.gameObject.name);
        /*for (int i = 0; i < gameObject.transform.GetChild(0).childCount; i++)
        {
            Debug.Log(gameObject.transform.GetChild(0).GetChild(i).name);    
        }*/
        if (CraftWindow.gameObject.activeInHierarchy)
        {
            CraftWindow.gameObject.SetActive(false);
        }
        if (RepairWindow.gameObject.activeInHierarchy)
        {
            RepairWindow.gameObject.SetActive(false);
        }
        if (UpgradeWindow.gameObject.activeInHierarchy)
        {
            UpgradeWindow.gameObject.SetActive(false);
        }
        if (SkillFunctionWindow.gameObject.activeInHierarchy)
        {
            SkillFunctionWindow.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
        UpdateCredit();
    }

    public void UpdateCredit()
    {
        gameObject.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            UiManager.Inventory.Credit.ToString();
    }

    private void OnNPCFunction()
    {
        if (!Instance.gameObject.activeInHierarchy)
        {
            //Debug.Log("OnFunc");
            Instance.gameObject.SetActive(true);
            if (UiManager.Instance.DialogBox.activeInHierarchy)
            {
                UiManager.Instance.DialogBox.SetActive(false);
            }
        }
    }
    
    public void OffNpcFunction()
    {
        if (Instance.gameObject.activeInHierarchy)
        {
            AudioManager.Instance.ClickButtonOffWindow();
            UiManager.Instance.Player.gameObject.GetComponent<CursorState>().SetCursor(true, false);
            Instance.gameObject.SetActive(false);
        }
        
        if (!UiManager.Instance.UiDictionary[0].activeInHierarchy && UiManager.Instance.PlayerContact)
        {
            UiManager.Instance.UiDictionary[0].SetActive(true);
        }
    }

    public void OnCraftWindow()
    {
        AudioManager.Instance.ClickButtonOnWindow();
        CraftWindow.gameObject.SetActive(true);
        RepairWindow.gameObject.SetActive(false);
        UpgradeWindow.gameObject.SetActive(false);
        SkillFunctionWindow.gameObject.SetActive(false);
        OnNPCFunction();
    }

    public void OnRepairWindow()
    {
        AudioManager.Instance.ClickButtonOnWindow();
        CraftWindow.gameObject.SetActive(false);
        RepairWindow.gameObject.SetActive(true);
        UpgradeWindow.gameObject.SetActive(false);
        SkillFunctionWindow.gameObject.SetActive(false);
        OnNPCFunction();
    }

    public void OnUpgradeWindow()
    {
        AudioManager.Instance.ClickButtonOnWindow();
        CraftWindow.gameObject.SetActive(false);
        RepairWindow.gameObject.SetActive(false);
        UpgradeWindow.gameObject.SetActive(true);
        SkillFunctionWindow.gameObject.SetActive(false);
        OnNPCFunction();
    }

    public void OnSkillWindow()
    {
        if (SkillFunction.Instance.CheckLiminex())
        {
            AudioManager.Instance.ClickButtonOnWindow();
            SkillFunction.Instance.ShowMainSkills();
            
            CraftWindow.gameObject.SetActive(false);
            RepairWindow.gameObject.SetActive(false);
            UpgradeWindow.gameObject.SetActive(false);
            SkillFunctionWindow.gameObject.SetActive(true);
            OnNPCFunction();
        }
    }

    private void ExitFunctionWindow()
    {
        //Debug.Log("exit");
    }
}
