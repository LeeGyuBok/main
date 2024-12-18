using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LaboratoryUiManager : MortalManager<LaboratoryUiManager>
{
    [SerializeField] private GameObject geneArchiveList;
    [SerializeField] private Button archiveButton;
    [SerializeField] private GameObject geneArchiveListContent;
    [SerializeField] private Button geneArchiveInfoButtonPrefab;
    
    [SerializeField] private GameObject geneStabilizationPanel;
    [SerializeField] private Button stabilizeButton;
    [SerializeField] private Button geneStabilizationSelectedGeneButton;
    [SerializeField] private Button startStabilizationButton;
    [SerializeField] private Slider stabilizationDegreeSlider;
    
    [SerializeField] private GameObject geneCombinePanel;
    [SerializeField] private Button combineButton;
    [SerializeField] private List<Button> selectedGenesInfoButtons;
    [SerializeField] private Button combinePreviewButton;
    
    [SerializeField] private GameObject combinePanel;
    [SerializeField] private Button combineStartButton;
    [SerializeField] private Button combineCancelButton;
    [SerializeField] private Button combineGeneTypeSelectButton;
    [SerializeField] private GameObject combineGeneSelectBlockPanel;
    [SerializeField] private TextMeshProUGUI combineStatusBonus;

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
    
    /*[SerializeField] */private GameObject geneDetailInfoPanel;
    /*[SerializeField] */private TextMeshProUGUI detailInfoGeneGrade;
    /*[SerializeField] */private Image detailInfoStatusImage1;
    /*[SerializeField] */private TextMeshProUGUI detailInfoStatus1;
    /*[SerializeField] */private Image detailInfoStatusImage2;
    /*[SerializeField] */private TextMeshProUGUI detailInfoStatus2;

    private Dictionary<Button, GameObject> panelByButton = new Dictionary<Button, GameObject>();
    private Dictionary<Button, Gene> geneByButton = new Dictionary<Button, Gene>();

    private Dictionary<Button, Gene> stabilizationGeneByButton;
    private Dictionary<Button, Gene> combineGeneByButton;
    private List<Gene> combineSelectedGenes;
    private List<Button> activeFalseButtons = new List<Button>();

    private int geneGradeSum;
    private int geneTypeSelector = -1;
    private GeneType selectedGeneType;
    
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        Initialize();
        SetArchiveList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        panelByButton[archiveButton] = geneArchiveList;
        panelByButton[stabilizeButton] = geneStabilizationPanel;
        panelByButton[combineButton] = geneCombinePanel;

        stabilizationGeneByButton = new Dictionary<Button, Gene>();
        
        combineGeneByButton = new Dictionary<Button, Gene>();
        for (int i = 0; i < selectedGenesInfoButtons.Count; i++)
        {
            combineGeneByButton.Add(selectedGenesInfoButtons[i], null);
        }

        combineSelectedGenes = new List<Gene>();
    }

    /*public override void ButtonToMainScene()
    {
        GameImmortalManager.Instance.RenewGenes();
        base.ButtonToMainScene();
    }*/


    #region ArchiveListSetting

    private void SetArchiveList()
    {
        List<Gene> genes = GameImmortalManager.Instance.Genes;
        if (genes == null)
        {
            StartCoroutine(ReturnMain());
            return;
        }
        for (int i = 0; i < genes.Count; i++)
        {
            Button button = Instantiate(geneArchiveInfoButtonPrefab, geneArchiveListContent.transform);
            geneByButton.Add(button, genes[i]);
            button.GetComponent<Image>().sprite = genes[i].Sprite;
        }
    }

    private void DeleteArchiveListButtons()
    {
        Transform geneArchiveListTransform = geneArchiveListContent.transform;
        int index = geneArchiveListTransform.childCount;
        for (int i = 0; i < index; i++)
        {
            Button button = geneArchiveListTransform.GetChild(0).gameObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            DestroyImmediate(button.gameObject);
        }
    }

    #endregion
    
    #region PanelSetting

    public void SetPanel(Button button)
    {
        if (panelByButton[button].Equals(panelByButton[archiveButton]))
        {
            SetPanelStates(true, false, false);
            foreach (var pair in geneByButton)
            {
                Button keyButton = pair.Key;
                keyButton.onClick.RemoveAllListeners();
            }
            return;
        }
        if (panelByButton[button].Equals(panelByButton[stabilizeButton]))
        {
            SetPanelStates(true, true, false);
            foreach (var pair in geneByButton)
            {
                Button keyButton = pair.Key;
                keyButton.onClick.RemoveAllListeners();
                keyButton.onClick.AddListener(() => SelectStabilizationGene(keyButton));
            }
            return;
        }
        if (panelByButton[button].Equals(panelByButton[combineButton]))
        {
            SetPanelStates(true, false, true);
            foreach (var pair in geneByButton)
            {
                Button keyButton = pair.Key;
                keyButton.onClick.RemoveAllListeners();
                keyButton.onClick.AddListener(() => SelectCombineGene(keyButton));
                
            }

            //return;
        }
    }

    private void SetPanelStates(bool archiveActive, bool stabilizationActive, bool combineActive)
    {
        //ex. 켜와 켜는 서로 다른가?     켜와 꺼는 서로 다른가? 
        //아니(false) => 패스           응(true) => 끈다
        if (geneArchiveList.activeInHierarchy != archiveActive)
        {
            geneArchiveList.SetActive(archiveActive);
        }

        if (geneStabilizationPanel.activeInHierarchy != stabilizationActive)
        {
            geneStabilizationPanel.SetActive(stabilizationActive);
        }

        if (geneCombinePanel.activeInHierarchy != combineActive)
        {
            geneCombinePanel.SetActive(combineActive);
        }
    }

    #endregion
    
    #region InfoPanel

    public void MoveInfo()
    {
        if (geneSimpleInfoPanel.activeInHierarchy)
        {
            geneSimpleInfoPanel.transform.position = Input.mousePosition;
            return;
        }

        if (geneDetailInfoPanel.activeInHierarchy)
        {
            geneDetailInfoPanel.transform.position = Input.mousePosition;
        }
    }

    public void ShowSimpleInfo(Button button)
    {
        Gene gene = geneByButton[button];
        Debug.Log(gene.GeneType.ToString());
        simpleInfoGeneGrade.text = gene.GeneGrade.ToString();
        simpleInfoHpStatus.text = gene.RandomStatusCoefficient[0].ToString();
        simpleInfoAtkStatus.text = gene.RandomStatusCoefficient[1].ToString();
        simpleInfoDfeStatus.text = gene.RandomStatusCoefficient[2].ToString();
        simpleInfoSpdStatus.text = gene.RandomStatusCoefficient[3].ToString();
        
        //근데 이미지는 어차피 고정되있을거아닌가? 맞네
        /*simpleInfoHpStatusImage.sprite = gene.Sprite;
        simpleInfoAtkStatusImage.sprite = gene.Sprite;
        simpleInfoDfeStatusImage.sprite = gene.Sprite;
        simpleInfoSpdStatusImage.sprite = gene.Sprite;*/
        
        
        
        geneSimpleInfoPanel.SetActive(true);
        geneSimpleInfoPanel.transform.position = Input.mousePosition;
    }
    
    public void HideInfo()
    {
        if (geneSimpleInfoPanel.activeInHierarchy)
        {
            geneSimpleInfoPanel.SetActive(false);
            return;
        }
        if (geneDetailInfoPanel.activeInHierarchy)
        {
            geneDetailInfoPanel.SetActive(false);
        }
    }

    public void ShowInfoSwitch()
    {
        if (geneSimpleInfoPanel.activeInHierarchy)
        {
            geneSimpleInfoPanel.SetActive(false);
            geneDetailInfoPanel.transform.position = Input.mousePosition;
            geneDetailInfoPanel.SetActive(true);
            return;
        }
        if (geneDetailInfoPanel.activeInHierarchy)
        {
            geneDetailInfoPanel.SetActive(false);
            geneSimpleInfoPanel.transform.position = Input.mousePosition;
            geneSimpleInfoPanel.SetActive(true);
        }
    }
    #endregion

    #region Stabilization

    //아카이브리스트에서 유전자를 선택한다. ->
    //선택한 유전자가 스테빌라이제이션의 geneStabilizationSelectedGeneButton에 등록된다.
    private void SelectStabilizationGene(Button button)
    {
        if (stabilizationGeneByButton.Count == 1)
        {
            Debug.Log("Already, gene selected ");
            return;
        }
        stabilizationGeneByButton[geneStabilizationSelectedGeneButton] = geneByButton[button];
        geneStabilizationSelectedGeneButton.GetComponent<Image>().sprite =
            stabilizationGeneByButton[geneStabilizationSelectedGeneButton].Sprite;
    }
    //만약 해당 버튼을 누르면 등록이 취소된다.
    public void ResetSelectStabilizationGene()
    {
        Debug.Log(stabilizationGeneByButton.Count);
        stabilizationGeneByButton.Clear();
        geneStabilizationSelectedGeneButton.GetComponent<Image>().sprite =
            baseGeneImage;
        Debug.Log(stabilizationGeneByButton.Count);
    }
    
    
    //스타트 버튼을 누르면 아래의 스테빌라이제이션이 시작된다.
    public void Stabilization()
    {
        if (stabilizationGeneByButton.Count == 1)
        {
            //참조타입이므로, 동일한 객체를 참조하게 되있어서 값이 바뀐 상태로 저장.
            Gene targetGene = stabilizationGeneByButton[geneStabilizationSelectedGeneButton];
            targetGene.SetNewStatusCoefficient();
        }
    }

    #endregion

    #region Combine

    //아카이브리스트의 버튼클릭시, 연결된 gene을 selectedGene버튼에 연결하고(딕셔너리),
    //조합예정 리스트에 넣음
    //리스너로 추가할 것
    private void SelectCombineGene(Button button)
    {
        if (combineSelectedGenes.Count > 0)//1개 이상 있으므로 중복검사가 필요
        {
            //조합될 진을 가지고 있는 버튼들에 대해서 전체 검사.
            foreach (var pair in combineGeneByButton)
            {
                //혹시 값이 널? 이라는 뜻은 그 다음 아이가 값이 없다는 뜻
                if (pair.Value == null)
                {
                    continue;
                }
                //조합될 진을 꺼내서 그 값을 확인한다. 
                if (combineGeneByButton.TryGetValue(pair.Key, out var value))//널이라는 밸류가 있어버렷!
                {
                    //값이 있고 그 값이 아카이브버튼이 가지고있는 값과 일치하다면
                    if (value.Equals(geneByButton[button]))
                    {
                        //끝내.
                        return;
                    }   
                }
            }
        }
        //중복인 값이 없다.
        foreach (var pair in combineGeneByButton)
        {
            if (pair.Value == null)
            {
                combineGeneByButton[pair.Key] = geneByButton[button];
                combineSelectedGenes.Add(geneByButton[button]);
                pair.Key.GetComponent<Image>().sprite = geneByButton[button].Sprite;
                activeFalseButtons.Add(button);
                button.gameObject.SetActive(false);
                HideInfo();
                return;
            }
        }
    }

    public void ResetCombineGene(Button button)
    {
        //여기서 이미 널이 되버린다.
        if (!combineGeneByButton.TryGetValue(button, out Gene selectedGene))
        {
            return;
        }

        if (combineSelectedGenes.Contains(selectedGene))
        {
            combineSelectedGenes.Remove(selectedGene); //리스트에서삭제하고
            combineGeneByButton[button] = null; //딕셔너리에서 삭제
            button.GetComponent<Image>().sprite = baseGeneImage;
            
            Dictionary<Gene, Button> swapGBB = new Dictionary<Gene, Button>();
        
            foreach (var kvpB in geneByButton)
            {
                swapGBB[kvpB.Value] = kvpB.Key;
            }

            activeFalseButtons.Remove(swapGBB[selectedGene]);
            swapGBB[selectedGene].gameObject.SetActive(true);
        }
        //딕셔너리와 그 버튼을 재정렬하는 로직 필요. <- 이거 필요해? 해야해? 미치겠네!

        /*List<GeneType> valueListIncludeNull = new List<GeneType>();
        foreach (var pair in combineGeneByButton)
        {
            valueListIncludeNull.Add(pair.Value);
        }

        List<GeneType> sortedValueListIncludeNull = new List<GeneType>();
        int nullCount = 0;
        for (int i = 0; i < valueListIncludeNull.Count; i++)
        {
            if (valueListIncludeNull[i] == null)
            {
                nullCount++;
                continue;
            }
            sortedValueListIncludeNull.Add(valueListIncludeNull[i]);
        }

        for (int i = 0; i < nullCount; i++)
        {
            sortedValueListIncludeNull.Add(null);
        }

        //순회중 요소 변경 ㄷㄷ;
        Dictionary<Button, GeneType> temp = new Dictionary<Button, GeneType>();
        for (int i = 0; i < sortedValueListIncludeNull.Count; i++)
        {
            temp.Add(selectedGenesInfoButtons[i], sortedValueListIncludeNull[i]);
        }

        combineGeneByButton = temp;


        foreach (var pair in combineGeneByButton)
        {
            if (combineGeneByButton.TryGetValue(pair.Key, out var value))
            {
                pair.Key.GetComponentInChildren<TextMeshProUGUI>().text = value.GeneType.ToString();
            }
            pair.Key.GetComponentInChildren<TextMeshProUGUI>().text = "Button";
        }*/
    }

    //진 리스트에 있는 애들 삭제하고(딕셔너리의 밸류값들도 삭제해야함.) 한 개의 진을 생성해줌
    public void ShowCombinePreview()
    {
        if (combineSelectedGenes.Count > 1)
        {
            geneGradeSum = 0;
            for (int i = 0; i < combineSelectedGenes.Count; i++)
            {
                geneGradeSum += (int)combineSelectedGenes[i].GeneGrade;
            }
            
            //여기서 예상결과창 띄우기 , 등급합이 16이상이면 타입선택버튼을 클릭할 수 있게하기.
            //예상결과창 - 보너스여부 == 능력치 계수 최소값: n, 진타입 선택여부, 합성시작, 합성취소. 
            combineGeneTypeSelectButton.image.preserveAspect = false;
            combineGeneTypeSelectButton.image.sprite = Resources.Load<Sprite>($"Image/DNA");
                
            if (geneGradeSum is < 16 and >= 12)
            {
                //진 타입 선택 불가, 계수가 최소 7
                //나중에 이미지로 치환할 것
                
                combineStatusBonus.text = "Minimum: 7";
                SetPreviewPanel(true);
                combineGeneSelectBlockPanel.SetActive(true);
            }
            else if (geneGradeSum is < 20 and >= 16)
            {
                //진 타입 선택, 계수가 최소 7

                combineStatusBonus.text = "Minimum: 7";
                SetPreviewPanel(true);
                combineGeneSelectBlockPanel.SetActive(false);
            }
            else if (geneGradeSum >= 20)
            {
                //진 타입 선택, 계수가 최소 8

                combineStatusBonus.text = "Minimum: 8";
                SetPreviewPanel(true);
                combineGeneSelectBlockPanel.SetActive(false);
            }
            else
            {
                //진 타입 선택 불가, 계수 보너스 없음.

                combineStatusBonus.text = "Minimum: 4";
                SetPreviewPanel(true);
                combineGeneSelectBlockPanel.SetActive(true);
            }
        }
    }

    //진 타입 선택버튼에 할당될 예정
    public void SetGeneTypeSelect(Button button)
    {
        geneTypeSelector++;
        if (geneTypeSelector == Enum.GetValues(typeof(GeneType)).Length-1)
        {
            geneTypeSelector = 0;
        }
        selectedGeneType = (GeneType)geneTypeSelector;
        button.image.preserveAspect = false;
        button.image.sprite = Resources.Load<Sprite>($"Image/{selectedGeneType.ToString()}");
    }

    private void SetPreviewPanel(bool value)
    {
        if (!combinePanel.activeInHierarchy)
        {
            /*if (value)
            {
                
            }*/
            combinePanel.SetActive(true);
          
        }
    }

    //클로즈버튼
    public void HidePreviewPanel()
    {
        combinePanel.SetActive(false);
    }
    
    //스타트버튼
    public void CombineStart()
    {
        StartCoroutine(CombineCoroutine(selectedGeneType));
    }

    private IEnumerator CombineCoroutine(GeneType? geneType)
    {
        //최소 두개
        if (combineSelectedGenes.Count > 1 )
        {
            //딕셔너리 초기화해. 순회오류 왜나는거임
            
            Dictionary<Button, Gene> temp = new Dictionary<Button, Gene>();
            for (int i = 0; i < selectedGenesInfoButtons.Count; i++)
            {
                selectedGenesInfoButtons[i].image.sprite = Resources.Load<Sprite>("Image/DNA");
                temp.Add(selectedGenesInfoButtons[i], null);
            }
            
            combineGeneByButton = new Dictionary<Button, Gene>(temp);
            
            //리스트에 있는 진들을 삭제.
            int index = combineSelectedGenes.Count;
            for (int i = 0; i < index; i++)
            {
                Gene gene = combineSelectedGenes[0];
                combineSelectedGenes.Remove(gene);
                JSONWriter.Instance.DeleteGeneData(gene);
                activeFalseButtons[i].onClick.RemoveAllListeners();
                DestroyImmediate(activeFalseButtons[i].gameObject);
                GameImmortalManager.Instance.RemoveGene(gene); // 최종적으로 게임매니저의 진 리스트에서 삭제 - 오전 4:14 2024-12-11 이부분 수정
            }
            
            activeFalseButtons.Clear();

            if (geneTypeSelector == -1)
            {
                geneType = null;
            }
            
            if (geneGradeSum is < 16 and >= 12)
            {
                Gene newGene = new Gene(7);
                Debug.Log(newGene.GeneID);
            }
            else if (geneGradeSum is < 20 and >= 16)
            {
                Gene newGene = new Gene(7, geneType);
                Debug.Log(newGene.GeneID);
            }
            else if (geneGradeSum >= 20)
            {
                Gene newGene = new Gene(8, geneType);
                Debug.Log(newGene.GeneID);
            }
            else
            {
                Gene newGene = new Gene();
                Debug.Log(newGene.GeneID);
            }
        }
        geneTypeSelector = -1;
        geneGradeSum = 0;

        GameImmortalManager.Instance.RenewGenes();
        DeleteArchiveListButtons();
        Initialize();
        SetArchiveList();
        yield break;
    }
    #endregion

    private IEnumerator ReturnMain()
    {
        yield return new WaitForSeconds(2f);
        ButtonToMainScene();

    }
}
