using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DictionaryUiManager : MortalManager<DictionaryUiManager>, IGoMain
{
    private List<Chimera> _wholeChimeras;
    private List<Gene> _wholeGenes;
    
    #region geneDictionary

    [SerializeField] private Button geneDictionaryButtonPrefab;
    [SerializeField] private Button enterGeneDictionaryButton;
    [SerializeField] private GameObject geneDictionaryPanel;
    [SerializeField] private Transform geneDictionaryContent;

    [SerializeField] private GameObject geneInfoPanel;
    [SerializeField] private GameObject geneGradePanel;
    
    [SerializeField] private TextMeshProUGUI geneTypeInfo;
    [SerializeField] private TextMeshProUGUI[] geneFeaturesInfo = new TextMeshProUGUI[3];
    
    [SerializeField] private Image mainSkillImage;
    [SerializeField] private TextMeshProUGUI mainSkillName;
    [SerializeField] private TextMeshProUGUI mainSkillDetail;
    
    [SerializeField] private Image subSkillImage;
    [SerializeField] private TextMeshProUGUI subSkillName;
    [SerializeField] private TextMeshProUGUI subSkillDetail;
    
    private Dictionary<Button, Gene> geneByButton;
    private Dictionary<Button, UnityAction> geneButtonActions;

    private GameObject _spawnedChimera;
    private readonly Vector3 _spawnedPoint = new (3.2f, 0.3f, -1.8f);
    private readonly Vector3 _spawnedRotation = new (0f, -135f, 0f);

    #endregion
    
    #region chimeraDictionary

    [SerializeField] private Button chimeraDictionaryButtonPrefab;
    [SerializeField] private Button enterChimeraDictionaryButton;
    [SerializeField] private GameObject chimeraDictionaryPanel;
    [SerializeField] private Transform chimeraDictionaryContent;

    [SerializeField] private GameObject chimeraStatusInfo;
    
    [SerializeField] private TextMeshProUGUI chimeraGeneTypeInfo;
    [SerializeField] private TextMeshProUGUI[] chimeraFeaturesInfo = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI isChimeraMutant;
    [SerializeField] private TextMeshProUGUI[] baseStatusInfo = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI chimeraDetail;
    
    [SerializeField] private Image chimeraSkillImage;
    [SerializeField] private TextMeshProUGUI chimeraSkillName;
    [SerializeField] private TextMeshProUGUI chimeraSkillDetail;
    
    private Dictionary<Button, Chimera> chimeraByButton;
    private Dictionary<Button, UnityAction> chimeraButtonActions;

    #endregion

    #region CommonButton

    [SerializeField] private Button acquisitionPathButton;
    [SerializeField] private Button closeInfoPanelButton;
    [SerializeField] private GameObject acquisitionPathPanel;
    [SerializeField] private TextMeshProUGUI acquisitionPathInfo;
    [SerializeField] private GameObject closeCurrentDictionaryPanelButtonObject;

    private readonly string geneAcquisitionInfo = "1. 유전자 보급권\n2. 성과 달성\n3. 공식 테스트 - 매 승리 시\n4. 비공식 테스트 - 승리 시";
    private readonly string chimeraAcquisitionInfo = "돌연변이 전문 연구원과의 상호 테스트에서 승리(공식/비공식)";
    
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _wholeChimeras = new List<Chimera>();
        _wholeGenes = new List<Gene>();
        
        geneByButton = new Dictionary<Button, Gene>();
        geneButtonActions = new Dictionary<Button, UnityAction>();
        
        chimeraByButton = new Dictionary<Button, Chimera>();
        chimeraButtonActions = new Dictionary<Button, UnityAction>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeCurrentDictionaryPanelButtonObject.SetActive(false);
        
        _wholeChimeras = new List<Chimera>(GameImmortalManager.Instance.AllChimeras);
        for (int i = 0; i < _wholeChimeras.Count; i++)
        {
            Button button = Instantiate(chimeraDictionaryButtonPrefab, chimeraDictionaryContent);
            UnityAction action = () => ShowSelectedChimeraInfo(button);
            if (_wholeChimeras[i].IsMutant)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"Mutant\n{_wholeChimeras[i].GeneType}";    
            }
            else
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"{_wholeChimeras[i].GeneType}";
            }
            chimeraButtonActions[button] = action;
            button.onClick.AddListener(action);
            chimeraByButton[button] = _wholeChimeras[i];
        }
        
        _wholeGenes = new List<Gene>(GameImmortalManager.Instance.AllGenes);
        for (int i = 0; i < _wholeGenes.Count; i++)
        {
            Button button = Instantiate(geneDictionaryButtonPrefab, geneDictionaryContent);
            UnityAction action = () => ShowSelectedGeneInfo(button);
            
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{_wholeGenes[i].GeneType}";
            
            geneButtonActions[button] = action;
            button.onClick.AddListener(action);
            geneByButton[button] = _wholeGenes[i];
        }

        acquisitionPathPanel.SetActive(false);
    }

    #region geneDictionary

    public void TurnOnGeneDictionaryPanel()
    {
        geneDictionaryPanel.SetActive(true);
        closeCurrentDictionaryPanelButtonObject.SetActive(true);
        enterGeneDictionaryButton.gameObject.SetActive(false);
        enterChimeraDictionaryButton.gameObject.SetActive(false);
        UiSoundManager.Instance.InDigitalSound();
    }
    
    private void ShowSelectedGeneInfo(Button button)
    {
        geneInfoPanel.SetActive(true);
        geneGradePanel.SetActive(true);
        acquisitionPathButton.gameObject.SetActive(true);
        closeInfoPanelButton.gameObject.SetActive(true);

        Gene gene = geneByButton[button];
        //Debug.Log(gene.GeneType);
        geneTypeInfo.text = gene.GeneType.ToString();

        for (int i = 0; i < geneFeaturesInfo.Length; i++)
        {
            geneFeaturesInfo[i].text = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene.GeneType]
                .Features[i].ToString();
        }

        
        if (ImmortalScriptableObjectManager.Instance.MainDnaByGeneType.TryGetValue(gene.GeneType, out MainDna main))
        {
            mainSkillImage.gameObject.SetActive(true);
            mainSkillName.gameObject.SetActive(true);
            mainSkillDetail.gameObject.SetActive(true);
            mainSkillImage.sprite = main.Sprite;
            mainSkillName.text = main.DnaMainSkill.SkillName;
            mainSkillDetail.text = main.DnaMainSkill.SkillDescription;    
        }
        else
        {
            mainSkillImage.gameObject.SetActive(false);
            mainSkillName.gameObject.SetActive(false);
            mainSkillDetail.gameObject.SetActive(false);
        }
        
        subSkillImage.sprite = gene.Sprite;
        subSkillName.text = gene.DnaSubSkill.SkillName;
        subSkillDetail.text = gene.DnaSubSkill.SkillDescription;

        acquisitionPathInfo.text = geneAcquisitionInfo;
        geneDictionaryPanel.SetActive(false);
        UiSoundManager.Instance.InDigitalSound();
    }

    #endregion
    
    #region chimeraDictionary
    
    public void TurnOnChimeraDictionaryPanel()
    {
        chimeraDictionaryPanel.SetActive(true);
        closeCurrentDictionaryPanelButtonObject.SetActive(true);
        enterGeneDictionaryButton.gameObject.SetActive(false);
        enterChimeraDictionaryButton.gameObject.SetActive(false);
        UiSoundManager.Instance.InDigitalSound();
    }

    private void ShowSelectedChimeraInfo(Button button)
    {
        chimeraStatusInfo.SetActive(true);
        acquisitionPathButton.gameObject.SetActive(true);
        closeInfoPanelButton.gameObject.SetActive(true);

        Chimera chimera = chimeraByButton[button];
        GeneType geneType = chimera.GeneType;
        chimeraGeneTypeInfo.text = geneType.ToString();
        for (int i = 0; i < geneFeaturesInfo.Length; i++)
        {
            chimeraFeaturesInfo[i].text = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[geneType]
                .Features[i].ToString();
        }
        isChimeraMutant.text = chimera.IsMutant.ToString();
        chimeraDetail.text = ImmortalScriptableObjectManager.Instance.MainDnaByGeneType[geneType].MainDnaDescription;

        if (chimera.IsMutant)
        {
            baseStatusInfo[0].text = ImmortalScriptableObjectManager.Instance.MutantStatusesByGeneType[geneType].MaxHealthPoint
                .ToString();
            baseStatusInfo[1].text = ImmortalScriptableObjectManager.Instance.MutantStatusesByGeneType[geneType].AttackPoint
                .ToString();
            baseStatusInfo[2].text = ImmortalScriptableObjectManager.Instance.MutantStatusesByGeneType[geneType].DefencePoint
                .ToString();
            baseStatusInfo[3].text = ImmortalScriptableObjectManager.Instance.MutantStatusesByGeneType[geneType].AgilityPoint
                .ToString();
        }
        else
        {
            baseStatusInfo[0].text = ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[geneType].MaxHealthPoint
                .ToString();
            baseStatusInfo[1].text = ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[geneType].AttackPoint
                .ToString();
            baseStatusInfo[2].text = ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[geneType].DefencePoint
                .ToString();
            baseStatusInfo[3].text = ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[geneType].AgilityPoint
                .ToString();
        }

        chimeraSkillImage.sprite = ImmortalScriptableObjectManager.Instance.MainDnaByGeneType[geneType].Sprite;
        chimeraSkillName.text = ImmortalScriptableObjectManager.Instance.MainDnaByGeneType[geneType].DnaMainSkill.SkillName;
        chimeraSkillDetail.text = ImmortalScriptableObjectManager.Instance.MainDnaByGeneType[geneType].DnaMainSkill.SkillDescription;
        
        acquisitionPathInfo.text = chimeraAcquisitionInfo;
        chimeraDictionaryPanel.SetActive(false);

        _spawnedChimera = 
            Instantiate(GameImmortalManager.Instance.DictionaryChimera[geneType].gameObject, _spawnedPoint, Quaternion.Euler(_spawnedRotation));
        UiSoundManager.Instance.InDigitalSound();
    }
    #endregion


    public void CloseCurrentDictionaryPanel()
    {
        if (geneDictionaryPanel.activeInHierarchy)
        {
            geneDictionaryPanel.SetActive(false);
        }
        
        if (chimeraDictionaryPanel.activeInHierarchy)
        {
            chimeraDictionaryPanel.SetActive(false);
        }

        if (geneInfoPanel.activeInHierarchy)
        {
            geneInfoPanel.SetActive(false);
            geneGradePanel.SetActive(false);
            acquisitionPathButton.gameObject.SetActive(false);
            closeInfoPanelButton.gameObject.SetActive(false);
        }

        if (chimeraStatusInfo.activeInHierarchy)
        {
            chimeraStatusInfo.SetActive(false);
            acquisitionPathButton.gameObject.SetActive(false);
            closeInfoPanelButton.gameObject.SetActive(false);
            Destroy(_spawnedChimera.gameObject);
        }
        
        acquisitionPathPanel.SetActive(false);
        enterGeneDictionaryButton.gameObject.SetActive(true);
        enterChimeraDictionaryButton.gameObject.SetActive(true);
        closeCurrentDictionaryPanelButtonObject.SetActive(false);
        UiSoundManager.Instance.InDigitalSound();
    }

    public void CloseInfoPanel()
    {
        UiSoundManager.Instance.InDigitalSound();
        if (geneInfoPanel.activeInHierarchy)
        {
            geneInfoPanel.SetActive(false);
            geneGradePanel.SetActive(false);
            acquisitionPathButton.gameObject.SetActive(false);
            closeInfoPanelButton.gameObject.SetActive(false);
            geneDictionaryPanel.SetActive(true);
            return;
        }

        if (chimeraStatusInfo.activeInHierarchy)
        {
            chimeraStatusInfo.SetActive(false);
            acquisitionPathButton.gameObject.SetActive(false);
            closeInfoPanelButton.gameObject.SetActive(false);
            chimeraDictionaryPanel.SetActive(true);
            Destroy(_spawnedChimera.gameObject);
        }
    }

    public void OpenAcquisitionInfo()
    {
        acquisitionPathPanel.SetActive(true);
        UiSoundManager.Instance.InDigitalSound();
    }

    public void CloseAcquisitionInfo()
    {
        acquisitionPathPanel.SetActive(false);
        UiSoundManager.Instance.InDigitalSound();
    }
    
    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }
}
