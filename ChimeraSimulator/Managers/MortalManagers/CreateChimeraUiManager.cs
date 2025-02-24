using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateChimeraUiManager : MortalManager<CreateChimeraUiManager>, IGoMain
{
    #region SelectChimeraPart
    //프리팹
    [SerializeField] private Button chimeraButton;
    [SerializeField] private Button geneButton;
    
    //버튼을 생성하는 곳
    [SerializeField] private GameObject possibleCreateChimerasPanel;
    [SerializeField] private Transform possibleCreateChimerasContent;
    
    [SerializeField] private TextMeshProUGUI[] selectedChimeraFeatures = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] selectedChimeraBaseStatus = new TextMeshProUGUI[4];
    [SerializeField] private Image selectedChimeraSkillImage;
    [SerializeField] private TextMeshProUGUI selectedChimeraSkillName;
    [SerializeField] private TextMeshProUGUI selectedChimeraSkillDetail;
    [SerializeField] private TextMeshProUGUI selectedChimeraDetail;
    
    //생성된 버튼이 켜주는 패널, 키메라 프리뷰
    [SerializeField] private GameObject selectedChimeraInfoPanel;
    
    [SerializeField] private Button selectCancelButton;
    
    private Chimera _selectedChimera;
    #endregion
    
    #region InsertGenePart
    
    private Gene _selectedGene;
    private Button _selectedGeneButton;
    
    [SerializeField] private GameObject embryoPanel;

    [SerializeField] private GameObject possibleInputGenesPanel;
    [SerializeField] private Transform possibleInputGenesContent;
    
    [SerializeField] private GameObject insertAlertPanel;
    [SerializeField] private GameObject insertedGenesPanel;
    [SerializeField] private Transform insertedGenesContent;
    
    [SerializeField] private GameObject insertedGenesInfoPanel;
    [SerializeField] private TextMeshProUGUI mainDnaGeneTypeByInsertedGenes;
    [SerializeField] private TextMeshProUGUI[] totalStatusByInsertedGenes = new TextMeshProUGUI[4];
    [SerializeField] private Image[] skillsImageByInsertedGenes = new Image[4];
    [SerializeField] private TextMeshProUGUI[] skillsNameByInsertedGenes = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] skillsDetailByInsertedGenes = new TextMeshProUGUI[4];

    [SerializeField] private Button insertAcceptButton;
    
    [SerializeField] private TextMeshProUGUI[] mainDnaFeatures = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] selectedGeneFeatures = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI totalCompatibilityPoint;
    
    [SerializeField] private TextMeshProUGUI selectedGeneName;
    [SerializeField] private TextMeshProUGUI[] selectedGeneStatusInfos = new TextMeshProUGUI[4];
    
    [SerializeField] private Image skillImage;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillActivation;
    #endregion

    #region Development

    [SerializeField] private GameObject goMainButton;
    [SerializeField] private Button developmentButton;

    #endregion
    
    private Dictionary<Button, Chimera> _chimeraByButton;
    private Dictionary<Button, UnityAction> _chimeraButtonActions;
    
    private Dictionary<Button, Gene> _possibleInputGeneByButton;
    private Dictionary<Button, UnityAction> _possibleInputGeneButtonActions;
    
    private Dictionary<Button, Gene> _insertedGeneByButton;
    
    private Dictionary<GeneType, MainDna> _previewDnaByGeneType;
    
    protected override void Awake()
    {
        base.Awake();
        _chimeraByButton = new Dictionary<Button, Chimera>();
        _possibleInputGeneByButton = new Dictionary<Button, Gene>();
        _insertedGeneByButton = new Dictionary<Button, Gene>();

        _chimeraButtonActions = new Dictionary<Button, UnityAction>();
        _possibleInputGeneButtonActions = new Dictionary<Button, UnityAction>();
        _previewDnaByGeneType = new Dictionary<GeneType, MainDna>(Enum.GetValues(typeof(GeneType)).Length-1);
    }

    private void Start()
    {
        for (int i = 0; i < Enum.GetValues(typeof(GeneType)).Length-1; i++)
        {
            _previewDnaByGeneType[(GeneType)i] = new MainDna((GeneType)i);
        }
        
        List<Chimera> tempChimeras = new List<Chimera>(ChimeraManager.Instance.DevelopmentableChimeras);
        for (int i = 0; i < tempChimeras.Count; i++)
        {
            Button button = Instantiate(chimeraButton, possibleCreateChimerasContent);
            if (tempChimeras[i].IsMutant)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"Mutant\n{tempChimeras[i].GeneType}";    
            }
            else
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"{tempChimeras[i].GeneType}";
            }
            
            UnityAction action = () => ShowSelectedChimera(button);
            _chimeraButtonActions[button] = action;
            button.onClick.AddListener(action);
            _chimeraByButton[button] = tempChimeras[i];
        }

        for (int i = 1; i < skillsImageByInsertedGenes.Length; i++)
        {
            skillsImageByInsertedGenes[i].gameObject.SetActive(false);
            skillsDetailByInsertedGenes[i].gameObject.SetActive(false);
            skillsNameByInsertedGenes[i].gameObject.SetActive(false);
        }

        List<Gene> tempGenes = new List<Gene>(GameImmortalManager.Instance.AccountUseAbleGene);
        for (int i = 0; i < tempGenes.Count; i++)
        {
            Button button = Instantiate(geneButton, possibleInputGenesContent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{tempGenes[i].GeneGrade.ToString()}";
            button.gameObject.GetComponent<Image>().sprite = tempGenes[i].Sprite;
            UnityAction action = () => ShowSelectedGene(button);
            _possibleInputGeneButtonActions[button] = action;
            button.onClick.AddListener(action);
            _possibleInputGeneByButton[button] = tempGenes[i];
        }
    }

    #region SelectChimeraPart
    
    private void ShowSelectedChimera(Button button)
    {
        //키메라 프리팹이 정해지는 곳
        selectedChimeraInfoPanel.SetActive(true);
        Chimera selectedChimera = _chimeraByButton[button];
        //키메라 이미지 셋팅 필요
        selectedChimeraFeatures[0].text = selectedChimera.GeneType.ToString();
        for (int i = 1; i < selectedChimeraFeatures.Length; i++)
        {
            selectedChimeraFeatures[i].text = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[selectedChimera.GeneType].Features[i-1].ToString();    
        }

        BaseStatus status = null;
        
        if (selectedChimera.IsMutant)
        {
            status = ImmortalScriptableObjectManager.Instance.MutantStatusesByGeneType[selectedChimera.GeneType];
        }
        else
        {
            status = ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[selectedChimera.GeneType]; 
        }
            
        selectedChimeraBaseStatus[0].text = status.MaxHealthPoint.ToString();
        selectedChimeraBaseStatus[1].text = status.AttackPoint.ToString();
        selectedChimeraBaseStatus[2].text = status.DefencePoint.ToString();
        selectedChimeraBaseStatus[3].text = status.AgilityPoint.ToString();
        
        selectedChimeraSkillImage.sprite = ImmortalScriptableObjectManager.Instance.MainDnaByGeneType[selectedChimera.GeneType].Sprite;
        selectedChimeraSkillName.text = _previewDnaByGeneType[selectedChimera.GeneType].DnaMainSkill.SkillName;
        selectedChimeraSkillDetail.text = _previewDnaByGeneType[selectedChimera.GeneType].DnaMainSkill.SkillDescription;

        selectedChimeraDetail.text = _previewDnaByGeneType[selectedChimera.GeneType].MainDnaDescription;
        
        possibleCreateChimerasPanel.SetActive(false);
        _selectedChimera = selectedChimera;
        UiSoundManager.Instance.InDigitalSound();
    }
    
    public void SetChimera()
    {
        InsertGenePart();
        
        ChimeraCreator.Instance.SetTargetChimeraData(_selectedChimera);
        BaseStatus statusData = ChimeraCreator.Instance.TargetChimeraData.BaseStatus;
        totalStatusByInsertedGenes[0].text = statusData.MaxHealthPoint.ToString(CultureInfo.InvariantCulture);
        totalStatusByInsertedGenes[1].text = statusData.AttackPoint.ToString(CultureInfo.InvariantCulture);
        totalStatusByInsertedGenes[2].text = statusData.DefencePoint.ToString(CultureInfo.InvariantCulture);
        totalStatusByInsertedGenes[3].text = statusData.AgilityPoint.ToString(CultureInfo.InvariantCulture);
        
        skillsImageByInsertedGenes[0].sprite = ImmortalScriptableObjectManager.Instance.MainDnaByGeneType[_selectedChimera.GeneType].Sprite;
        skillsNameByInsertedGenes[0].text = ImmortalScriptableObjectManager.Instance.MainDnaByGeneType[_selectedChimera.GeneType].DnaMainSkill.SkillName;
        skillsDetailByInsertedGenes[0].text = ImmortalScriptableObjectManager.Instance
            .MainDnaByGeneType[_selectedChimera.GeneType].DnaMainSkill.SkillDescription;
        goMainButton.SetActive(false);
        developmentButton.gameObject.SetActive(true);
        mainDnaGeneTypeByInsertedGenes.text = _selectedChimera.GeneType.ToString();
    }

    private void InsertGenePart(bool set = true)
    {
        possibleCreateChimerasPanel.SetActive(!set);
        selectedChimeraInfoPanel.SetActive(!set);
        possibleInputGenesPanel.SetActive(set);
        embryoPanel.SetActive(set);
        insertedGenesPanel.SetActive(set);
        insertedGenesInfoPanel.SetActive(set);
        possibleInputGenesPanel.SetActive(set);
        UiSoundManager.Instance.AcceptSound();
    }
    
    public void CancelChimeraSelect()
    {
        _selectedChimera = null;
        selectedChimeraInfoPanel.SetActive(false);
        possibleCreateChimerasPanel.SetActive(true);
        UiSoundManager.Instance.InDigitalSound();
    }
    
    #endregion
    
    #region InsertGenePart

    private void ShowSelectedGene(Button button)
    {
        if (ChimeraCreator.Instance.TargetChimeraData == null)
        {
            return;
        }
        insertAlertPanel.SetActive(true);
        _selectedGeneButton = button;
        _selectedGene = _possibleInputGeneByButton[_selectedGeneButton];  
        
        //Gene이미지 셋팅?
        int equal = 0;
        for (int i = 0; i < mainDnaFeatures.Length; i++)
        {
            mainDnaFeatures[i].text = ChimeraCreator.Instance.TargetChimeraData.MainDna.DnaFeatureList.Features[i].ToString();
            selectedGeneFeatures[i].text = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[_selectedGene.GeneType].Features[i].ToString();
            if (mainDnaFeatures[i].text.Equals(selectedGeneFeatures[i].text))
            {
                equal++;
            }
        }
        
        totalCompatibilityPoint.text = equal.ToString();
        
        selectedGeneName.text = _selectedGene.GeneType.ToString();
        for (int i = 0; i < selectedGeneStatusInfos.Length; i++)
        {
            selectedGeneStatusInfos[i].text = (_selectedGene.RandomStatusCoefficient[i] + equal).ToString();
        }

        skillImage.sprite = _selectedGene.Sprite;
        skillName.text = _selectedGene.DnaSubSkill.SkillName;
        if (ChimeraCreator.Instance.TargetChimeraData.MainDna.TrySetSubSkill(_selectedGene))
        {
            skillActivation.text = "활성화";
        }
        else
        {
            skillActivation.text = "비활성화";
        }
        
        PossibleGeneInsert(ChimeraCreator.Instance.TargetChimeraData.MainDna.TryInsertGene(_selectedGene));
        UiSoundManager.Instance.InDigitalSound();
    }

    private void PossibleGeneInsert(bool canInsert = true)
    {
        insertAcceptButton.gameObject.SetActive(canInsert);
    }

    public void InsertSelectedGene()
    {
        ChimeraData data = ChimeraCreator.Instance.TargetChimeraData;
        
        if (ChimeraCreator.Instance.TargetChimeraData.MainDna.TrySetSubSkill(_selectedGene))
        {
            int index = data.MainDna.DnaSubSkills.Count + 1;
            skillsImageByInsertedGenes[index].sprite = _selectedGene.Sprite;
            skillsNameByInsertedGenes[index].text = _selectedGene.DnaSubSkill.SkillName;
            skillsDetailByInsertedGenes[index].text = _selectedGene.DnaSubSkill.SkillDescription;    
            skillsImageByInsertedGenes[index].gameObject.SetActive(true);
            skillsNameByInsertedGenes[index].gameObject.SetActive(true);
            skillsDetailByInsertedGenes[index].gameObject.SetActive(true);
        }
        
        ChimeraCreator.Instance.InsertGene(_selectedGene);
        
        _selectedGeneButton.transform.SetParent(insertedGenesContent, false);
        
        _selectedGeneButton.onClick.RemoveListener(_possibleInputGeneButtonActions[_selectedGeneButton]);
        _possibleInputGeneByButton.Remove(_selectedGeneButton);
        _possibleInputGeneButtonActions.Remove(_selectedGeneButton);
        
        totalStatusByInsertedGenes[0].text = (data.BaseStatus.MaxHealthPoint * (100 + data.MainDna.TotalHealthCoefficient)/100).ToString();
        totalStatusByInsertedGenes[1].text = (data.BaseStatus.AttackPoint * (100 + data.MainDna.TotalAttackCoefficient)/100).ToString();
        totalStatusByInsertedGenes[2].text = (data.BaseStatus.DefencePoint * (100 + data.MainDna.TotalDefenceCoefficient)/100).ToString();
        totalStatusByInsertedGenes[3].text = (data.BaseStatus.AgilityPoint * (100 + data.MainDna.TotalAgilityCoefficient)/100).ToString();
        
        _selectedGene = null;
        insertAlertPanel.SetActive(false);
        UiSoundManager.Instance.AcceptSound();
        SetGeneSelectPanel(false);
    }

    public void SetGeneSelectPanel(bool set)
    {
        possibleInputGenesPanel.SetActive(set);
        developmentButton.gameObject.SetActive(set);
    }

    public void CancelSelectedGene()
    {
        UiSoundManager.Instance.CancelSound();
        _selectedGene = null;
        insertAlertPanel.SetActive(false);
    }
    
    #endregion

    #region DevelopmentChimera

    public void DevelopmentChimera()
    {
        GameImmortalManager.Instance.AddDailySupplyTokens();
        goMainButton.SetActive(true);
        ChimeraCreator.Instance.TargetChimeraData.SetCoefficientToStatus();
        GameImmortalManager.Instance.AddAccountChimeraData(ChimeraCreator.Instance.DevelopmentChimera());
        UiSoundManager.Instance.AcceptSound();
        developmentButton.gameObject.SetActive(false);
    }
    
    #endregion
    
    public void GoMain()
    {
        int index = possibleInputGenesContent.childCount;
        for (int i = 0; i < index; i++)
        {
            Button button = possibleInputGenesContent.GetChild(0).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            _chimeraButtonActions.Remove(button);
        }

        index = possibleCreateChimerasContent.childCount;
        for (int i = 0; i < index; i++)
        {
            Button button = possibleCreateChimerasContent.GetChild(0).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            _possibleInputGeneButtonActions.Remove(button);
        }

        
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }
    
    
}
