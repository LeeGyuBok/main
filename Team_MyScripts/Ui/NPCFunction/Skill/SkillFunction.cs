using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillFunction : MonoBehaviour
{
    public static SkillFunction Instance;

    [SerializeField] private Button skill1;
    [SerializeField] private Button skill2;
    [SerializeField] private Button skill3;
    [SerializeField] private Button skill4;
    [SerializeField] private TextMeshProUGUI skillPointAmount;

    [SerializeField] private Image selectedSkillImage;
    [SerializeField] private TextMeshProUGUI selectedSkillLevel;

    [SerializeField] private GameObject selectedSkillTree;

    [SerializeField] private TextMeshProUGUI selectedSkillName;
    [SerializeField] private Image selectedSkillDetailImage;
    [SerializeField] private TextMeshProUGUI selectedSkillDetail;

    [SerializeField] private Button selectedSkillSubOption;
    [SerializeField] private Button selectedSkillNextLevel;
    [SerializeField] private TextMeshProUGUI totalSubOptionDetail;
    [SerializeField] private TextMeshProUGUI mainSkillNextLevelDetail;

    [SerializeField] private Button subOptionButtonObjectPrefab;
    
    //로직용
    public Liminex liminex { get; private set; }
    private Dictionary<Button, IamSkill> mainSkillByButton;
    private Sprite baseImage;
    private Dictionary<Button, SkillSubOption> subOptionByButton;
    private Dictionary<SkillSubOption, SkillNode> skillNodeBySubOption;
    private List<UnityAction> onClickEventList;

    private IamSkill selectedSkill;

    
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            mainSkillByButton = new Dictionary<Button, IamSkill>();
            subOptionByButton = new Dictionary<Button, SkillSubOption>();
            skillNodeBySubOption = new Dictionary<SkillSubOption, SkillNode>();
            onClickEventList = new List<UnityAction>();
        }
        NPCFunctionManager.SkillFunctionWindow = Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //리미넥스가 활성화 되었는지 여부 확인
    //리미넥스를 어떻게 할당하지? 
    //여기에 리미넥스 변수를 선언하고, null이면 키아이템창을 순회시켜서 찾고, 찾으면 할당해놓는다.
    public bool CheckLiminex()
    {
        //리미넥스가 이미 할당되어있으면 트루
        if (liminex != null) return true;
        //키아이템의 데이터를 순회
        foreach (KeyValuePair<Button, Stack<Item_SO>> pair in Inventory.Instance.inventory_KeyItem)
        {
            //각 키와 밸류쌍에 대해서, 밸류의 스택에있는 아이템을 선언
            Item_SO item = pair.Value.Peek();
            //Debug.Log(item.data.ItemName.Equals(EnumItemCode.Liminex.ToString()));//여기는 트루
            //리미넥스 아이템이 있으면
            if (item.data.ItemName.Equals(EnumItemCode.Liminex.ToString()))
            {
                //리미넥스아이템이 있으면
                Debug.Log(item.GetType());
                if (item is Liminex liminexItem)
                {
                    //이미 등록된 리미넥스가 있는지 확인(최초로 획득한 리미넥스만 사용)
                    if (liminex == null)
                    {
                        //없으면 그 리미넥스를 할당한다.
                        liminex = liminexItem;
                        SkillManager.Instance.Liminex = liminex;
                    }
                    else
                    {
                        //만약 등록된 리미넥스가 있으면, 이후 발견되는 리미넥스들은 반납
                        ItemPool_SO.Instance.GoPool_Item(item);
                    }
                }
            }
        }
        Debug.Log(liminex);
        if (liminex != null) return true;
        return false;
        
    }

    public void ShowMainSkills()
    {
        if (!CheckLiminex()) return;
        if (!liminex.IsActivate)return;
        
        /*나머지스킬들에 대한 할당 - 하드코딩. + 미해금 스킬에대한 덮어씌울이미지? -- Lock*/
        skill1.GetComponent<Image>().sprite = liminex.Blink.SkillImage;
        skill2.GetComponent<Image>().sprite = liminex.Barrier.SkillImage;
        mainSkillByButton[skill1] = liminex.Blink;
        mainSkillByButton[skill2] = liminex.Barrier;
        
        //리미넥스의 스킬포인트(모든 스킬들이 공유하는 포인트)
        skillPointAmount.text = liminex.SkillPoint.ToString();
        
        //디테일지우기. string 부분은 추후에 바뀌는 이미지의 이름으로 변경예정
        if (!selectedSkillImage.sprite.name.Equals("GUI_14"))
        {
            selectedSkillImage.sprite = baseImage;
            selectedSkillDetailImage.sprite = baseImage;
            selectedSkillLevel.text = "0";
            selectedSkillName.text = "";
            selectedSkillDetail.text = "";
            totalSubOptionDetail.text = "";

            totalSubOptionDetail.gameObject.SetActive(false);
            mainSkillNextLevelDetail.gameObject.SetActive(false);
            
            //8번돈다
            for (int i = 0; i < selectedSkillTree.transform.childCount; i++)
            {
                int index = selectedSkillTree.transform.GetChild(i).childCount;
                for (int j = 0; j < index; j++)
                {
                    selectedSkillTree.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                    DestroyImmediate(selectedSkillTree.transform.GetChild(i).GetChild(0).gameObject);
                }
            }
            //버튼 삭제하고 리스트에 등록된 모든 이벤트리스너들을 삭제한다.
            onClickEventList.Clear();
        }
    }

    public void ShowSkillDetail(Button button)
    {
        //이미지와 투자된 스킬포인트셋팅 셀렉티드스킬레벨 == 투자된 스킬포인트
        if (baseImage == null)
        {
            //최초선택일 때, 최초선택당시의 기본 이미지를 저장한다.
            baseImage = selectedSkillImage.sprite;
        }
        
        //버튼을 클릭해서 스킬이 있을 때.
        if (mainSkillByButton.TryGetValue(button, out selectedSkill))
        {
            //최초선택일 때
            if (selectedSkillImage.sprite.name.Equals("GUI_14"))
            {
                SettingSkillTree(button);
            }
            else//최초선택이 아닐 때.. 위에 부분을 안들어가고..
            {
                //먼저 스킬트리 삭제
                for (int i = 0; i < selectedSkillTree.transform.childCount; i++)
                {
                    int index = selectedSkillTree.transform.GetChild(i).childCount;
                    for (int j = 0; j < index; j++)
                    {
                        selectedSkillTree.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                        DestroyImmediate(selectedSkillTree.transform.GetChild(i).GetChild(0).gameObject);
                    }
                }
                //버튼 삭제하고 리스트에 등록된 모든 이벤트리스너들을 삭제한다.
                onClickEventList.Clear();
                
                SettingSkillTree(button);
            }
            //셀렉트되면 셀렉트된 스킬의 이미지 및 정보 출력
            selectedSkillImage.sprite = selectedSkill.SkillImage;
            selectedSkillLevel.text = selectedSkill.SkillTreeValue.Count.ToString();
        
            //디테일창
            selectedSkillName.text = selectedSkill.SkillName;
            selectedSkillDetailImage.sprite = selectedSkill.SkillImage;
            
            //스위치나 if문으로 수정해야함.
            if (button.Equals(skill1))
            {
                selectedSkillDetail.text = $"Blink {mainSkillByButton[button].TotalStatus}m toward you move";
            }
            else if (button.Equals(skill2))
            {
                selectedSkillDetail.text = $"Create Barrier of {mainSkillByButton[button].TotalStatus}health around you";
            }
            
        
            //서브스킬옵션 디테일
            if (mainSkillByButton[button].SkillTreeValue.Count == 0)
            {
                totalSubOptionDetail.text = "SubOption: available";    
            }
            else
            {
                string detail = "";
                for (int i = 0; i < mainSkillByButton[button].SkillTreeValue.Count; i++)
                {
                    detail += mainSkillByButton[button].SkillTreeValue[i].SelectedOption.TestInt + "\n";//여기서 에러가 나는데?
                }
                totalSubOptionDetail.text += $"SubOption: {detail}";    
            }
            //스위치나 if문으로 수정해야함.
            if (button.Equals(skill1))
            {
                mainSkillNextLevelDetail.text =
                    $"Blink {mainSkillByButton[button].TotalStatus + mainSkillByButton[button].BaseEnforceStatus}m toward you move";    
            }
            else if (button.Equals(skill2))
            {
                mainSkillNextLevelDetail.text =
                    $"Create Barrier of {mainSkillByButton[button].TotalStatus + mainSkillByButton[button].BaseEnforceStatus}health around you";
            }
            
            ShowSubOptionDetail();
        }
        else//없을 때
        {
            Debug.Log("unlocked Skill");
            return;
        }
    }

    private void SettingSkillTree(Button button)
    {
        List<SkillNode> eachNodes = mainSkillByButton[button].SkillTree.CircuitNextNodes(mainSkillByButton[button].SkillTree);

        for (int i = 0; i < selectedSkillTree.transform.childCount; i++)
        {
            //Debug.Log($"{eachNodes[i].name}");
            for (int j = 0; j < eachNodes[i].PossibleSubOptions.Count; j++)
            {
                Button subOptionButton = Instantiate(subOptionButtonObjectPrefab, selectedSkillTree.transform.GetChild(i).transform);
                //서브옵션의 내용 또는 이미지 넣기
                subOptionButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    eachNodes[i].PossibleSubOptions[j].TestInt.ToString();
                /*subOptionButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                            eachNodes[i].name.ToString();*/
                subOptionByButton[subOptionButton] = eachNodes[i].PossibleSubOptions[j];
                skillNodeBySubOption[eachNodes[i].PossibleSubOptions[j]] = eachNodes[i];
                UnityAction clickEvent = () => OnClickSkillSubOption(subOptionButton);
                onClickEventList.Add(clickEvent);
                subOptionButton.onClick.AddListener(clickEvent);
            }
        }
    }

    //최초에는 토탈서브옵션만나옴. 하지만 다음레벨의 정보도 여기서 세팅함.
    public void ShowBlinkDetail()
    {
        //이미지와 투자된 스킬포인트셋팅 셀렉티드스킬레벨 == 투자된 스킬포인트
        if (baseImage == null)
        {
            baseImage = selectedSkillImage.sprite;    
        }
        
        //아직 셀렉트되지 않은 상태에서만 버튼들 추가.
        //string 부분은 추후에 바뀌는 이미지의 이름으로 변경예정
        if (selectedSkillImage.sprite.name.Equals("GUI_14"))
        {
            //각 노드의 PossibleSubOptions의 카운트가필요하다.
            List<SkillNode> eachNodes = mainSkillByButton[skill1].SkillTree.CircuitNextNodes(mainSkillByButton[skill1].SkillTree);
            
            for (int i = 0; i < selectedSkillTree.transform.childCount; i++)
            {
                //Debug.Log($"{eachNodes[i].name}");
                for (int j = 0; j < eachNodes[i].PossibleSubOptions.Count; j++)
                {
                    Button subOptionButton = Instantiate(subOptionButtonObjectPrefab, selectedSkillTree.transform.GetChild(i).transform);
                    //서브옵션의 내용 또는 이미지 넣기
                    subOptionButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                        eachNodes[i].PossibleSubOptions[j].TestInt.ToString();
                    /*subOptionButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                        eachNodes[i].name.ToString();*/
                    subOptionByButton[subOptionButton] = eachNodes[i].PossibleSubOptions[j];
                    skillNodeBySubOption[eachNodes[i].PossibleSubOptions[j]] = eachNodes[i];
                    UnityAction clickEvent = () => OnClickSkillSubOption(subOptionButton);
                    onClickEventList.Add(clickEvent);
                    subOptionButton.onClick.AddListener(clickEvent);
                }
                /*mainSkillByButton[skill1].SkillTree.*/
            }
        }
        
        //셀렉트되면 셀렉트된 스킬의 이미지 및 정보 출력
        selectedSkillImage.sprite = skill1.GetComponent<Image>().sprite;
        selectedSkillLevel.text = mainSkillByButton[skill1].SkillTreeValue.Count.ToString();
        
        //디테일창
        selectedSkillName.text = mainSkillByButton[skill1].SkillName;
        selectedSkillDetailImage.sprite = selectedSkillImage.sprite;
        selectedSkillDetail.text = $"Blink {mainSkillByButton[skill1].TotalStatus}m toward you move";
        
        //서브스킬옵션 디테일
        if (mainSkillByButton[skill1].SkillTreeValue.Count == 0)
        {
            totalSubOptionDetail.text = "SubOption: available";    
        }
        else
        {
            string detail = "";
            for (int i = 0; i < mainSkillByButton[skill1].SkillTreeValue.Count; i++)
            {
                detail += mainSkillByButton[skill1].SkillTreeValue[i].SelectedOption.TestInt + "\n";
            }
            totalSubOptionDetail.text += $"SubOption: {detail}";    
        }
        mainSkillNextLevelDetail.text =
            $"Blink {mainSkillByButton[skill1].TotalStatus + mainSkillByButton[skill1].BaseEnforceStatus}m toward you move";
        ShowSubOptionDetail();
    }
    
    private void OnClickSkillSubOption(Button button)
    {
        if (subOptionByButton.TryGetValue(button, out SkillSubOption subOption))
        {
            SkillNode targetNode = skillNodeBySubOption[subOption];
            Debug.Log(targetNode.IsInvested);
            //스킬이 찍혀있는가?
            if (targetNode.IsInvested)//네
            {
                //스킬이 Lv1스킬인가?
                if (targetNode.FrontNode.Equals(targetNode)) //예
                {
                    //다음 스킬이 찍혀있는가?
                    if (targetNode.NextNode.IsInvested)//예
                    {
                        //그 노드의, 모든 다음 노드를 가지고온다.
                        List<SkillNode> alreadyInvestedNode = targetNode.CircuitNextNodes(targetNode.NextNode);
                        for (int i = 0; i < alreadyInvestedNode.Count; i++)
                        {
                            SkillNode investedNode = alreadyInvestedNode[i];
                            
                            if (investedNode.IsInvested)
                            {
                                
                                WithdrawSkill(investedNode);
                                
                                
                                /*investedNode.WithdrawSkillPoint();
                                investedNode.RemoveSubOption();
                                
                                liminex.Blink.Withdraw(investedNode);
                                liminex.WithdrawSkillPoint();
                            
                                SkillPointChecker();
                                
                                selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    //아니오. 또는 모든 다음 스킬들을 초기화 함.
                    //현재 선택한 노드의 스킬을 안찍은 상태로 되돌린다.
                    WithdrawSkill(targetNode);
                    /*targetNode.WithdrawSkillPoint();
                    targetNode.RemoveSubOption();
                    
                    liminex.Blink.Withdraw(targetNode);
                    liminex.WithdrawSkillPoint();
                    
                    SkillPointChecker();
                    
                    selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                    SkillManager.Instance.RequestSetSkillUi();
                    return;
                }

                //스킬이 분기스킬인가?
                if (targetNode.PossibleSubOptions.Count >= 2)//예
                {
                    //분기스킬의 다음 스킬이 찍혀있는가?
                    if (targetNode.NextNode.IsInvested)//네
                    {
                        //그 노드의, 모든 다음 노드를 가지고온다.
                        List<SkillNode> alreadyInvestedNode = targetNode.CircuitNextNodes(targetNode.NextNode);
                        for (int i = 0; i < alreadyInvestedNode.Count; i++)
                        {
                            SkillNode investedNode = alreadyInvestedNode[i];
                            
                            if (investedNode.IsInvested)
                            {
                                WithdrawSkill(investedNode);
                                /*investedNode.WithdrawSkillPoint();
                                investedNode.RemoveSubOption();
                                
                                liminex.Blink.Withdraw(investedNode);
                                liminex.WithdrawSkillPoint();
                            
                                SkillPointChecker();
                                
                                selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                            }
                            else
                            {
                                break;
                            }
                        }
                        //아니오. 또는 모든 다음 스킬들을 초기화 함.
                        //현재 선택한 노드의 스킬을 안찍은 상태로 되돌린다.
                        WithdrawSkill(targetNode);
                        /*targetNode.WithdrawSkillPoint();
                        targetNode.RemoveSubOption();
                    
                        liminex.Blink.Withdraw(targetNode);
                        liminex.WithdrawSkillPoint();
                    
                        SkillPointChecker();
                    
                        selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                        return;
                    }
                    else//아니오
                    {
                        //선택한 옵션과 이미 찍혀있는 옵션이 같은가?
                        if (targetNode.SelectedOption.Equals(subOption))//예
                        {
                            //찍혀있는 옵션을 해제한다.
                            WithdrawSkill(targetNode);
                            /*targetNode.WithdrawSkillPoint();
                            targetNode.RemoveSubOption();
                    
                            liminex.Blink.Withdraw(targetNode);
                            liminex.WithdrawSkillPoint();
                    
                            SkillPointChecker();
                    
                            selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                            return;
                        }
                        else//아니오
                        {
                            //기존 등록된 노드(옵션)을 제거한다. 새로운 옵션을 할당한다.
                            liminex.Blink.Withdraw(targetNode);
                            targetNode.SetSubOption(subOption);
                            liminex.Blink.Enforce(targetNode);
                            
                            selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();
                            return;
                        }
                    }
                }
                
                //다음 스킬이 찍혀있는가?
                if (targetNode.NextNode.IsInvested)//예
                {
                    //그 스킬을 시작으로 모든 다음 스킬을 가지고온다.
                    List<SkillNode> alreadyInvestedNode = targetNode.CircuitNextNodes(targetNode.NextNode);
                    for (int i = 0; i < alreadyInvestedNode.Count; i++)
                    {
                        SkillNode autoWithdrawNode = alreadyInvestedNode[i];
                        if (autoWithdrawNode.IsInvested)
                        {
                            WithdrawSkill(autoWithdrawNode);
                            /*autoWithdrawNode.WithdrawSkillPoint();
                            autoWithdrawNode.RemoveSubOption();
                    
                            liminex.Blink.Withdraw(autoWithdrawNode);
                            liminex.WithdrawSkillPoint();
                    
                            SkillPointChecker();
                    
                            selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                        }
                        else
                        {
                            //처음노드부터 넣고, 순차적으로 돌기때문에 가능할듯?
                            break;
                        }
                    }
                }
                //아니오.
                //찍혀있는 모든 스킬들을 초기화했거나, 다음 스킬이 찍혀져있지 않다면 현재 선택한 스킬을 초기화한다.
                WithdrawSkill(targetNode);
                return;
            }
            else//스킬이 찍혀있지 않음 == 새로 찍음
            {
                //1레벨 스킬인가?
                if (targetNode.FrontNode.Equals(targetNode))//네
                {
                    if (liminex.SkillPoint > 0)
                    {
                        //그럼 찍는다.
                        InvestSkill(targetNode, subOption);
                        /*liminex.InvestSkillPoint();
                        targetNode.InvestSkillPoint();

                        targetNode.SetSubOption(subOption);

                        SkillPointChecker();

                        liminex.Blink.Enforce(targetNode);
                        selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/    
                        return;
                    }
                }
                //분기스킬인가?
                if (targetNode.PossibleSubOptions.Count >= 2)//네
                {
                    //이전 스킬이 찍혀있나?
                    if (targetNode.FrontNode.IsInvested)//네
                    {
                        if (liminex.SkillPoint > 0)
                        {
                            Debug.Log("Front is invested");
                            InvestSkill(targetNode, subOption);
                            /*liminex.InvestSkillPoint();
                            targetNode.InvestSkillPoint();

                            targetNode.SetSubOption(subOption);

                            SkillPointChecker();

                            liminex.Blink.Enforce(targetNode);


                            selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                            return;
                        }
                        Debug.Log("not enough skill point");
                        return;
                    }
                    else//아니오
                    {
                        List<int> frontNodeSubOptionCounts = targetNode.GetFrontPossibleSubOptionsCount(targetNode.FrontNode);
                        
                        if (frontNodeSubOptionCounts.Contains(2) || frontNodeSubOptionCounts.Contains(3))
                        {
                            Debug.Log("Select Divided Node First");
                            return;
                        }

                        List<SkillNode> autoInvestNodes = targetNode.CircuitFrontNodes(targetNode.FrontNode);
                        
                        for (int i = 0; i < autoInvestNodes.Count; i++)
                        {
                            if (!autoInvestNodes[i].IsInvested && liminex.SkillPoint > 0)
                            {
                                InvestSkill(autoInvestNodes[i], autoInvestNodes[i].PossibleSubOptions[0]);
                                /*liminex.InvestSkillPoint();
                                autoInvestNodes[i].InvestSkillPoint();
                        
                                autoInvestNodes[i].SetSubOption(autoInvestNodes[i].PossibleSubOptions[0]);
                        
                                SkillPointChecker();

                                liminex.Blink.Enforce(autoInvestNodes[i]);
                                selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                            }
                            else
                            {
                                if (autoInvestNodes[i].IsInvested)
                                {
                                    Debug.Log("invest all");
                                }

                                if (liminex.SkillPoint == 0)
                                {
                                    Debug.Log("not enough skill point");
                                    return;
                                }
                                break;
                            }
                        }
                        if (liminex.SkillPoint > 0)
                        {
                            InvestSkill(targetNode, subOption);
                            /*liminex.InvestSkillPoint();
                            targetNode.InvestSkillPoint();

                            targetNode.SetSubOption(subOption);

                            SkillPointChecker();

                            liminex.Blink.Enforce(targetNode);

                            selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                            return;
                        }
                        Debug.Log("not enough skill point");
                        return;
                       
                    }
                }
                //이전 스킬이 찍혀있는가?
                if (targetNode.FrontNode.IsInvested)
                {
                    if (liminex.SkillPoint > 0)
                    {
                        InvestSkill(targetNode, subOption);
                        /*liminex.InvestSkillPoint();
                        targetNode.InvestSkillPoint();

                        targetNode.SetSubOption(subOption);

                        SkillPointChecker();

                        liminex.Blink.Enforce(targetNode);

                        selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                        return;
                    }
                    Debug.Log("not enough skill point");
                    return;
                }
                else
                {
                    List<int> frontNodeSubOptionCounts = targetNode.GetFrontPossibleSubOptionsCount(targetNode.FrontNode);
                    
                    if (frontNodeSubOptionCounts.Contains(2) || frontNodeSubOptionCounts.Contains(3))
                    {
                        Debug.Log("Select Divided Node First");
                        return;
                    }
               
                    List<SkillNode> autoInvestNodes = targetNode.CircuitFrontNodes(targetNode.FrontNode);
                    for (int i = 0; i < autoInvestNodes.Count; i++)
                    {
                        if (!autoInvestNodes[i].IsInvested && liminex.SkillPoint > 0)
                        {
                            InvestSkill(autoInvestNodes[i], autoInvestNodes[i].PossibleSubOptions[0]);
                            /*liminex.InvestSkillPoint();
                            autoInvestNodes[i].InvestSkillPoint();
                        
                            autoInvestNodes[i].SetSubOption(autoInvestNodes[i].PossibleSubOptions[0]);
                        
                            SkillPointChecker();

                            liminex.Blink.Enforce(autoInvestNodes[i]);
                            selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();*/
                        }
                        else
                        {
                            if (autoInvestNodes[i].IsInvested)
                            {
                                Debug.Log("invest all");
                            }

                            if (liminex.SkillPoint == 0)
                            {
                                Debug.Log("not enough skill point");
                                return;
                            }
                            break;
                        }
                    }

                    if (liminex.SkillPoint > 0)
                    {
                        InvestSkill(targetNode, subOption);
                        return;
                    }
                    Debug.Log("not enough skill point");
                    return;
                }
            }
        }
    }

    private void InvestSkill(SkillNode targetNode, SkillSubOption subOption)
    {
        //리미넥스의 통합 스킬포인트 감소
        liminex.InvestSkillPoint();
        //노드에 투자
        targetNode.InvestSkillPoint();
        //서브옵션 활성화
        targetNode.SetSubOption(subOption);
                    
        SkillPointChecker();

        //스킬강화
        selectedSkill.Enforce(targetNode); // 이새기가 문제였네
                    
        selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();
        selectedSkillDetail.text = $"Blink {mainSkillByButton[skill1].TotalStatus}m toward you move";
        SkillManager.Instance.RequestSetSkillUi();
    }
    
    //이 부분 왜 안들어감?
    private void WithdrawSkill(SkillNode targetNode)
    {
        //노드 철회
        targetNode.WithdrawSkillPoint();
        //서브옵션 비활성화
        targetNode.RemoveSubOption();
                    
        SkillPointChecker();
        //리미넥스 통합스킬포인트 증가.
        liminex.WithdrawSkillPoint();
        
        //스킬 약화..?
        selectedSkill.Withdraw(targetNode); // 이새기가 문제였네
        
        selectedSkillLevel.text = liminex.Blink.SkillTreeValue.Count.ToString();
        selectedSkillDetail.text = $"Blink {mainSkillByButton[skill1].TotalStatus}m toward you move";
        SkillManager.Instance.RequestSetSkillUi();
    }

    public void ShowNextLevelInfo()
    {
        if (!selectedSkillImage.sprite.name.Equals("GUI_14"))
        {
            totalSubOptionDetail.gameObject.SetActive(false);
            mainSkillNextLevelDetail.gameObject.SetActive(true);
        }
    }

    public void ShowSubOptionDetail()
    {
        if (!selectedSkillImage.sprite.name.Equals("GUI_14"))
        {
            totalSubOptionDetail.gameObject.SetActive(true);
            mainSkillNextLevelDetail.gameObject.SetActive(false);
        }
    }

    //일단은 블링크만
    private bool SkillPointChecker()
    {
        if (liminex.Blink.SkillTreeValue.Count == liminex.RemainSkillPoint())
        {
            return true;
        }
        return false;
    }
}
