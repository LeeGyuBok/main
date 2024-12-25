using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OperatorDetailUiManager : MortalManager<OperatorDetailUiManager>
{
    [SerializeField] private Button levelButton;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterSex;
    [SerializeField] private TextMeshProUGUI characterAge;
    [SerializeField] private Button trustButton;
    [SerializeField] private TextMeshProUGUI healthPoint;
    [SerializeField] private TextMeshProUGUI attackPower;
    [SerializeField] private TextMeshProUGUI defensePower;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private Button roleButton;
    [SerializeField] private Button dnaInfoButton;
    [SerializeField] private Button cycleSkillButton;
    [SerializeField] private Button overExSkillButton;
    
    [SerializeField] private Transform popupPanelParent;
    [SerializeField] private GameObject lvlUpPopupPanel;
    [SerializeField] private GameObject trustPopupPanel;
    [SerializeField] private GameObject rolePopupPanel;
    [SerializeField] private GameObject dnaInfoPopupPanel;
    [SerializeField] private GameObject cycleSkillPopupPanel;
    [SerializeField] private GameObject overExSkillPopupPanel;
    
    [SerializeField] private Button geneListButton;
    [SerializeField] private Transform geneListImageParent;
    [SerializeField] private GameObject geneListPopupPanel;
    [SerializeField] private List<Image> geneStickImagePrefabs;

    [SerializeField] private Button geneArchiveInfoButtonPrefab;
    [SerializeField] private GameObject geneArchiveListContent;
    [SerializeField] private Transform geneListImagePopupParent;
    [SerializeField] private List<TextMeshProUGUI> geneListCoefficientPanelText;

    [SerializeField] private GameObject geneSimpleInfoPanel; 
    [SerializeField] private TextMeshProUGUI simpleInfoGeneGrade;
    [SerializeField] private Image simpleInfoHpStatusImage;
    [SerializeField] private TextMeshProUGUI simpleInfoHpStatus;
    [SerializeField] private Image simpleInfoAtkStatusImage;
    [SerializeField] private TextMeshProUGUI simpleInfoAtkStatus;
    [SerializeField] private Image simpleInfoDfeStatusImage;
    [SerializeField] private TextMeshProUGUI simpleInfoDfeStatus;
    [SerializeField] private Image simpleInfoSpdStatusImage;
    [SerializeField] private TextMeshProUGUI simpleInfoSpdStatus;
    [SerializeField] private Sprite baseGeneImage;
    
    private Dictionary<Button, GameObject> popupPanelDict = new Dictionary<Button, GameObject>();
    private Dictionary<Button, Gene> geneByButton = new Dictionary<Button, Gene>();
    
    private Dictionary<Gene, List<Button>> stickButtonListByGene = new Dictionary<Gene, List<Button>>(); 
    private List<List<Button>> operatorsStickGeneButtonList = new List<List<Button>>();
    
    private Dictionary<Gene, List<Image>> stickImageListByGene = new Dictionary<Gene, List<Image>>(); 
    private List<List<Image>> operatorsStickGeneImageList = new List<List<Image>>();
    
    
    public FriendlyOperator selectedOperator { get; private set; }
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    //게임매니저로부터 선택된 오퍼레이터 데이터 받아오기
    void Start()
    {
        selectedOperator = GameImmortalManager.Instance.GetOperatorData();
        
        //버튼과 해당 팝업패널을 딕셔너리로 연결 -> 오픈과 클로즈팝업으로 이어짐
        popupPanelDict.Add(levelButton, lvlUpPopupPanel);
        popupPanelDict.Add(trustButton, trustPopupPanel);
        popupPanelDict.Add(roleButton, rolePopupPanel);
        popupPanelDict.Add(dnaInfoButton, dnaInfoPopupPanel);
        popupPanelDict.Add(cycleSkillButton, cycleSkillPopupPanel);
        popupPanelDict.Add(overExSkillButton, overExSkillPopupPanel);
        popupPanelDict.Add(geneListButton, geneListPopupPanel);
        
        for (int i = 0; i < popupPanelParent.childCount; i++)
        {
            popupPanelParent.GetChild(i).gameObject.SetActive(false);
        }
        
        /*if (selectedOperator == null)
        {
            Debug.Log("No Operator Selected");
            return;
        }
        //Debug.Log("Selected Operator : " + selectedOperator.name);*/
        
        //캐릭터의 능력치정보 셋팅
        SetCharacterStatusInfo();
    }

    #region PopUp

    //팝업창을 연다. 단, 플레이어가 보유한 Gene이 없다면 이전 화면으로 되돌아감. -> 이부분은 알림창을 띄워주고 되돌아가는게 나을듯
    public void OpenPopup(Button button)
    {
        if (GameImmortalManager.Instance.Genes == null)
        {
            ButtonToOperatorScene();
        }
        
        //팝업패널 딕셔너리에따라서 버튼에 연결된 팝업패널을 열고 끔.
        if (popupPanelDict[button].activeInHierarchy)
        {
            popupPanelDict[button].SetActive(false);    
            return;
        }
        popupPanelDict[button].SetActive(true);
        
        // 젠리스트팝업패널의 경우,
        // 오퍼레이터의 현재 Dna 상태를 반영해야함. <- 이것도 해야하고 <- 상태반영은 인풋으로 바로바로한다. 그러면 여기서 상태를 다시 반영할 필요가 있나? 없다.
        // 진을 삽입하고 추출하면서 변해야함. <- 이것도 해야함. 
        // 오퍼레이터들에게 삽입된 Gene을 보여주지 않아야함. <- 이건 해야함. 패널업데이트
        if (popupPanelDict[button].Equals(geneListPopupPanel))
        {
            SetArchiveListPanel();
        }
    }

    //팝업패널에 붙은 클로즈버튼
    public void ClosePopup(Button button)
    {
        button.transform.parent.gameObject.SetActive(false);
    }

    #endregion

    #region InitializeCharaterInfo

    private void SetCharacterStatusInfo()
    {
        if (selectedOperator != null)
        {
            //텍스트만 설정
            SetStatusTextUi();

            
            //팝업창과 하단 배터리모양 Dna의 상태세팅(셀릭티드오퍼레이터의)
            SetOperatorDnaState();
        }
        else
        {
            Debug.Log("No Operator Selected");
        }
    }

    private void SetStatusTextUi()
    {
        characterName.text = selectedOperator.TrustData.CharacterName;
        characterSex.text = selectedOperator.TrustData.Sex.ToString();
        characterAge.text = selectedOperator.TrustData.Age.ToString();

        trustButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedOperator.CurrentTrust.ToString();
            
        healthPoint.text = selectedOperator.Health.CurrentPoint.ToString();
        attackPower.text = selectedOperator.Attack.CurrentPoint.ToString();
        defensePower.text = selectedOperator.Defense.CurrentPoint.ToString();
        speed.text = selectedOperator.Speed.CurrentPoint.ToString();
            
        roleButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedOperator.Role.ToString();

        dnaInfoButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedOperator.BattleStatusData.Gene.ToString();
        cycleSkillButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedOperator.Dna.CycleSkill.SkillName;
        overExSkillButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedOperator.Dna.OverExpressionSkill.SkillName;
    }

    private void SetOperatorDnaState()
    {
        List<Gene> list = selectedOperator.Dna.GeneList;
        int geneCount = list.Count;
        if (list.Count < 1)
        {
            RemoveAndRecreateDummyImages(geneListImagePopupParent);
            RemoveAndRecreateDummyImages(geneListImageParent);
            return;
        }
        //오퍼레이터에 삽입된 Gene에 대해서
        for (int i = 0; i < geneCount; i++)//그 갯수만큼 돌건데
        {
            Gene gene = list[i];
            string stringGeneType = gene.GeneType.ToString();
            
            //여기는 노말버전
            foreach (var prefab in geneStickImagePrefabs)
            {
                if (prefab.sprite.name.Equals(stringGeneType/*.ToString()*/ + "StickGene"))
                {
                    switch (gene.GeneGrade)
                    {
                        case GeneGrade.Pure:
                            SetStickGeneButtons(prefab, geneListImagePopupParent, gene, 1); 
                            RemoveAndRecreateDummyImages(geneListImagePopupParent);
                            
                            SetStickGeneImages(prefab, geneListImageParent, gene, 1);
                            RemoveAndRecreateDummyImages(geneListImageParent);
                            break;
                        case GeneGrade.Advanced:
                            SetStickGeneButtons(prefab, geneListImagePopupParent, gene, 2);
                            RemoveAndRecreateDummyImages(geneListImagePopupParent);
                            
                            SetStickGeneImages(prefab, geneListImageParent, gene, 2);
                            RemoveAndRecreateDummyImages(geneListImageParent);
                            break;
                        case GeneGrade.Processed:
                            SetStickGeneButtons(prefab, geneListImagePopupParent, gene, 3);
                            RemoveAndRecreateDummyImages(geneListImagePopupParent);
                            
                            SetStickGeneImages(prefab, geneListImageParent, gene, 3);
                            RemoveAndRecreateDummyImages(geneListImageParent);
                            break;
                        case GeneGrade.Crude:
                            SetStickGeneButtons(prefab, geneListImagePopupParent, gene, 4);
                            RemoveAndRecreateDummyImages(geneListImagePopupParent);
                            
                            SetStickGeneImages(prefab, geneListImageParent, gene, 4);
                            RemoveAndRecreateDummyImages(geneListImageParent);
                            break;
                        case GeneGrade.None:
                            Debug.Log("not defined: Grade is None");
                            break;
                        default:
                            Debug.Log("default Grade is None");
                            break;
                    }
                }
            }
        }
    }

    #endregion

    #region InitializeGeneInputInfo

    private void SetArchiveListPanel()
    {
        //아카이브버튼 리스트 셋팅- 최초
        if (geneArchiveListContent.transform.childCount < 1)
        {
            List<Gene> genes = GameImmortalManager.Instance.Genes;
            if (genes == null)
            {
                return;
            }
            for (int i = 0; i < genes.Count; i++)
            {
                //해당 오퍼레이터가 사용중인 dna 찾으면 보여주지 않음.
                if (selectedOperator.Dna.GeneList.Contains(genes[i]))
                {
                    continue;
                }
                
                Button button = Instantiate(geneArchiveInfoButtonPrefab, geneArchiveListContent.transform);
                button.GetComponent<Image>().sprite = genes[i].Sprite;
                button.onClick.AddListener(() => InputSelectedGene(button));
                geneByButton.Add(button, genes[i]);
            }
        
            //능력치계수 보여주기
            geneListCoefficientPanelText[0].text = selectedOperator.Dna.TotalHealthCoefficient.ToString();
            geneListCoefficientPanelText[1].text = selectedOperator.Dna.TotalAttackCoefficient.ToString();
            geneListCoefficientPanelText[2].text = selectedOperator.Dna.TotalDefenceCoefficient.ToString();
            geneListCoefficientPanelText[3].text = selectedOperator.Dna.TotalSpeedCoefficient.ToString();
        }
        else//오퍼레이터 진을 넣어서 아카이브리스트 수정 등 아카이브리스트자체에 변동
        {
            List<Gene> genes = GameImmortalManager.Instance.Genes;
            if (genes == null)
            {
                return;
            }
            
            //싹다지우고
            int index = geneArchiveListContent.transform.childCount;
            for (int i = 0; i < index; i++)
            {
                geneArchiveListContent.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                DestroyImmediate(geneArchiveListContent.transform.GetChild(0).gameObject);
            }
            geneByButton.Clear();
            
            
            for (int i = 0; i < genes.Count; i++)
            {
                if (selectedOperator.Dna.GeneList.Contains(genes[i]))
                {
                    continue;
                }
                
                Button button = Instantiate(geneArchiveInfoButtonPrefab, geneArchiveListContent.transform);
                button.GetComponent<Image>().sprite = genes[i].Sprite;
                button.onClick.AddListener(() => InputSelectedGene(button));
                geneByButton.Add(button, genes[i]);
            }
        
            //능력치계수 보여주기
            geneListCoefficientPanelText[0].text = selectedOperator.Dna.TotalHealthCoefficient.ToString();
            geneListCoefficientPanelText[1].text = selectedOperator.Dna.TotalAttackCoefficient.ToString();
            geneListCoefficientPanelText[2].text = selectedOperator.Dna.TotalDefenceCoefficient.ToString();
            geneListCoefficientPanelText[3].text = selectedOperator.Dna.TotalSpeedCoefficient.ToString();
        }
    }

    //아카이브 버튼에 동작연결.
    //아카이브버튼과 연결된 Gene을 리스트에 넣어줌.
    //그리고 이 아카이브버튼을 지움.
    private void InputSelectedGene(Button button)
    {
        //Debug.Log(button.onClick);
        Gene selectedGene = geneByButton[button];
        //selectedGene.SetCharacterName(selectedOperator.TrustData.CharacterName);
        if (selectedOperator.Dna.TryInsertGene(selectedGene))//Dna에 Gene을 넣을 수 있다면 넣고 true반환
        {
            //능력치 최신화
            OperatorBattleStatus selectedOperatorBattleStatus = selectedOperator;
            selectedOperatorBattleStatus.SetCoefficient();
            SetStatusTextUi();
            JSONWriter.Instance.SaveFriendlyOperatorData(selectedOperator);
            JSONWriter.Instance.SaveRenewalGeneData(selectedGene);
            
            //자기 자신을 지우기. 온클릭리스너들 모두제거, 리스트에서제거, 최종적으로 클릭된 버튼 자기자신 제거
            button.onClick.RemoveAllListeners();
            geneByButton.Remove(button);
            DestroyImmediate(button.gameObject);
            
            //가끔 사라진 버튼에대한 인포창이남음
            HideInfo();
            
            //이후에 해야하는 것. 진리스티이미지팝업패런트에 막대'버튼' 생성하기
            //막대버튼 생성하기도 여기서해야겠네.
            //막대버튼들을 앞으로 이동시키려면(현재 레이어그룹으로 자동 정렬됨)
            //막대버튼생성하고, 더미들을 지우고, 다시 만들어준다(리무브앤리크리에이트더미)
            
            //진짜 딱 지 행동만함 매우만족
            //Part of loop's body can be converted into LINQ-expression but another 'GetEnumerator' method will be used
            //여기는 Linq버전
            foreach (var prefab in geneStickImagePrefabs.Where(prefab => prefab.sprite.name.Equals(selectedGene.GeneType/*.ToString()*/ + "StickGene")))
            {
                switch (selectedGene.GeneGrade)
                {
                    case GeneGrade.Pure:
                        SetStickGeneButtons(prefab, geneListImagePopupParent, selectedGene, 1); 
                        RemoveAndRecreateDummyImages(geneListImagePopupParent);
                            
                        SetStickGeneImages(prefab, geneListImageParent, selectedGene, 1);
                        RemoveAndRecreateDummyImages(geneListImageParent);
                        break;
                    case GeneGrade.Advanced:
                        SetStickGeneButtons(prefab, geneListImagePopupParent, selectedGene, 2);
                        RemoveAndRecreateDummyImages(geneListImagePopupParent);
                            
                        SetStickGeneImages(prefab, geneListImageParent, selectedGene, 2);
                        RemoveAndRecreateDummyImages(geneListImageParent);
                        break;
                    case GeneGrade.Processed:
                        SetStickGeneButtons(prefab, geneListImagePopupParent, selectedGene, 3);
                        RemoveAndRecreateDummyImages(geneListImagePopupParent);
                            
                        SetStickGeneImages(prefab, geneListImageParent, selectedGene, 3);
                        RemoveAndRecreateDummyImages(geneListImageParent);
                        break;
                    case GeneGrade.Crude:
                        SetStickGeneButtons(prefab, geneListImagePopupParent, selectedGene, 4);
                        RemoveAndRecreateDummyImages(geneListImagePopupParent);
                            
                        SetStickGeneImages(prefab, geneListImageParent, selectedGene, 4);
                        RemoveAndRecreateDummyImages(geneListImageParent);
                        break;
                    case GeneGrade.None:
                        Debug.Log("not defined: Grade is None");
                        break;
                    default:
                        Debug.Log("default Grade is None");
                        break;
                }
            }
            SetArchiveListPanel();
        }
    }
    
    //팝업패널에 버튼 생성하는 아이
    private void SetStickGeneButtons(Image prefab, Transform parentTransform, Gene gene, int cycle)
    {
        List<Button> buttonList = new List<Button>();
        for (int j = 0; j < cycle; j++)
        {
            Image image = Instantiate(prefab, parentTransform);
            image.raycastTarget = true;
            image.gameObject.AddComponent<Button>();//버튼은 들어가. 근데 버튼에 온클릭이 할당이안돼.
            Button button = image.gameObject.GetComponent<Button>();
            button.onClick.AddListener(() => RemoveSelectedGene(button)); 
            buttonList.Add(button);
        }
        stickButtonListByGene[gene] = buttonList;
        operatorsStickGeneButtonList.Add(buttonList);
    }
    
        //하단 이미지에 막대 이미지 생성하는 아이
        private void SetStickGeneImages(Image prefab, Transform parentTransform, Gene gene, int cycle)
        {
            List<Image> imageList = new List<Image>();
            for (int j = 0; j < cycle; j++)
            {
                Image image = Instantiate(prefab, parentTransform);
                imageList.Add(image);
            }
            stickImageListByGene[gene] = imageList;
            operatorsStickGeneImageList.Add(imageList);
        }

    //클릭된 버튼과 그 그룹을 먼저지운다.
    private void RemoveSelectedGene(Button button)
    {
        //안되던 이유 - 이미지 자체에 레이캐스트타겟이 꺼져있었다 ㅋㅋ;
        for (int i = 0; i < operatorsStickGeneButtonList.Count; i++)
        {
            //버튼리스트 선정
            List<Button> buttonList = operatorsStickGeneButtonList[i];
            List<Image> imageList = operatorsStickGeneImageList[i];
            //찾은버튼 = 버튼리스트에서 찾기(버튼리스트의 요소   중에서   버튼리스트의 요소와 같음(이 버튼이))
            //아.. 이거 .. 말고 다른 더 쉬운방법이 있었네
            //Button foundButton = buttonList.Find(matchedButton => matchedButton.Equals(button));
            //선정된 버튼리스트에 인자로 받은 버튼(클릭한 버튼)이 있으면
            if (buttonList.Contains(button))
            {
                //buttonList로 gene을 찾는 딕셔너리를 만들고
                Dictionary<List<Button>, Gene> swapButton = new Dictionary<List<Button>, Gene>();
                foreach (var kvpB in stickButtonListByGene)
                {
                    swapButton[kvpB.Value] = kvpB.Key;
                }

                /*Dictionary<List<Image>, Gene> swapImage = new Dictionary<List<Image>, Gene>();
                foreach (var kvpI in stickImageListByGene)
                {
                    swapImage[kvpI.Value] = kvpI.Key;
                }*/
                //buttonList가 가르키는 Gene을 뺀다.
                selectedOperator.Dna.RemoveGene(swapButton[buttonList]);
                OperatorBattleStatus selectedOperatorBattleStatus = selectedOperator;
                selectedOperatorBattleStatus.SetCoefficient();
                SetStatusTextUi();
                JSONWriter.Instance.SaveFriendlyOperatorData(selectedOperator);
                JSONWriter.Instance.SaveRenewalGeneData(swapButton[buttonList]);
                
                for (int j = 0; j < buttonList.Count; j++)
                {
                    //버튼리스트의 모든 리스너들을 지우고
                    buttonList[j].onClick.RemoveAllListeners();
                    //버튼을 지운뒤(이때, 버튼만 지우면 버튼 컴포넌트만 지워짐.)
                    DestroyImmediate(buttonList[j].gameObject);
                }

                for (int j = 0; j < imageList.Count; j++)
                {
                    DestroyImmediate(imageList[j].gameObject);
                }
                operatorsStickGeneButtonList.Remove(buttonList);
                operatorsStickGeneImageList.Remove(imageList);
                break;
            }
        }
        //operatorsStickGeneButtonList로 변경된 정보를 토대로 최신화한다.
        //제거하는 로직은 따로 만들어야겠다.
        RemoveAndRecreateDummyImages(geneListImagePopupParent);
        RemoveAndRecreateDummyImages(geneListImageParent);
        SetArchiveListPanel();
    }

    //인자로받은 트랜스폼에서 더미찾아서 지우고 재생성하면서 맨 뒤로 재정렬해줌
    private void RemoveAndRecreateDummyImages(Transform parent)
    {
        //부모에 있는 더미유전자이미지 찾기
        int childCount = parent.childCount;
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i <childCount; i++)
        {
            if (parent.GetChild(i).GetComponent<Image>().sprite.name.Contains("Dummy"))
            {
                children.Add(parent.GetChild(i).gameObject);
            }
        }
        
        //싹다 지우기
        childCount = children.Count;
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(children[i]);
        }
        
        //더미재생성. 필요한 더미는 최대수용량에서 현재 수용된 진을 의미하는 버튼의 개수만큼 뺀 수
        Image dummyGeneStickImage = geneStickImagePrefabs[0];
        int totalDummyCount = Dna.MaxGeneCapacity - parent.childCount;
        for (int i = 0; i < totalDummyCount; i++)
        {
            Instantiate(dummyGeneStickImage, parent);
        }
    }
    #endregion
    
    #region InfoPanel

    public void MoveInfo()
    {
        if (geneSimpleInfoPanel.activeInHierarchy)
        {
            geneSimpleInfoPanel.transform.position = Input.mousePosition;
        }
    }

    public void ShowSimpleInfo(Button button)
    {
        Gene gene = geneByButton[button];
        
        //근데 이미지는 어차피 고정되있을거아닌가? 맞네
        /*simpleInfoHpStatusImage.sprite = gene.Sprite;
        simpleInfoAtkStatusImage.sprite = gene.Sprite;
        simpleInfoDfeStatusImage.sprite = gene.Sprite;
        simpleInfoSpdStatusImage.sprite = gene.Sprite;*/
        
        Debug.Log(gene.GeneType.ToString());
        switch (gene.GeneGrade)
        {
            case GeneGrade.Crude:
                simpleInfoGeneGrade.text = $"{gene.GeneGrade.ToString()}: need 4";
                break;
            case GeneGrade.Processed:
                simpleInfoGeneGrade.text = $"{gene.GeneGrade.ToString()}: need 3";
                break;
            case GeneGrade.Advanced:
                simpleInfoGeneGrade.text = $"{gene.GeneGrade.ToString()}: need 2";
                break;
            case GeneGrade.Pure:
                simpleInfoGeneGrade.text = $"{gene.GeneGrade.ToString()}: need 1";
                break;
        }
        
        simpleInfoHpStatus.text = gene.RandomStatusCoefficient[0].ToString();
        simpleInfoAtkStatus.text = gene.RandomStatusCoefficient[1].ToString();
        simpleInfoDfeStatus.text = gene.RandomStatusCoefficient[2].ToString();
        simpleInfoSpdStatus.text = gene.RandomStatusCoefficient[3].ToString();
        
        
        
        geneSimpleInfoPanel.SetActive(true);
        geneSimpleInfoPanel.transform.position = Input.mousePosition;
    }
    
    public void HideInfo()
    {
        if (geneSimpleInfoPanel.activeInHierarchy)
        {
            geneSimpleInfoPanel.SetActive(false);
        }
    }

    #endregion
    
   

    public void ButtonToOperatorScene()
    {
        SceneImmortalManager.Instance.LoadOperatorScene();
    } 
}

