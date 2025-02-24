using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CageUiManager : MortalManager<CageUiManager>, IGoMain
{
    private ChimeraData _showingChimeraData;

    #region simpleInfo

    [SerializeField] private GameObject simpleInfoPanel;

    [SerializeField] private TextMeshProUGUI chimeraGeneType;
    [SerializeField] private TextMeshProUGUI[] chimeraFeatures = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] totalStatus = new TextMeshProUGUI[4];
    [SerializeField] private Image[] skillsImages = new Image[4];
    [SerializeField] private TextMeshProUGUI[] skillsNames = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] skillsDetails = new TextMeshProUGUI[4];

    #endregion
    
    #region DetailInfo

    [SerializeField] private GameObject detailInfoPanel;
    
    [SerializeField] private TextMeshProUGUI chimeraGeneTypeInfo;
    [SerializeField] private TextMeshProUGUI[] chimeraFeaturesInfo = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] baseStatusInfo = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] totalCoefficientInfo = new TextMeshProUGUI[4];
    
    [SerializeField] private GameObject coefficientDetailInfoPrefabs;
    [SerializeField] private Transform coefficientTableContent;
    
    
    
    #endregion
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _showingChimeraData = GameImmortalManager.Instance.CageChimeraData;
        simpleInfoPanel.SetActive(true);
        detailInfoPanel.SetActive(false);
        Invoke(nameof(InstantiateChimera), 1f);

        chimeraGeneType.text = _showingChimeraData.MainDna.GeneType.ToString();
        for (int i = 0; i < chimeraFeatures.Length; i++)
        {
            chimeraFeatures[i].text = _showingChimeraData.MainDna.DnaFeatureList.Features[i].ToString();
        }

        totalStatus[0].text = _showingChimeraData.MaxHealthPoint.ToString(CultureInfo.CurrentCulture);
        totalStatus[1].text = _showingChimeraData.AttackPoint.ToString(CultureInfo.CurrentCulture);
        totalStatus[2].text = _showingChimeraData.DefencePoint.ToString(CultureInfo.CurrentCulture);
        totalStatus[3].text = _showingChimeraData.AgilityPoint.ToString(CultureInfo.CurrentCulture);

        skillsImages[0].sprite = _showingChimeraData.MainDna.Sprite;
        skillsNames[0].text = _showingChimeraData.MainDna.DnaMainSkill.SkillName;
        skillsDetails[0].text = _showingChimeraData.MainDna.DnaMainSkill.SkillDescription;
        
        for (int i = 0; i < 3 ; i++)
        {
            if (i < _showingChimeraData.MainDna.DnaSubSkills.Count)
            {
                skillsImages[i + 1].sprite = Resources.Load<Sprite>($"GeneImage/{_showingChimeraData.MainDna.DnaSubSkills[i].GeneType.ToString()}");
                skillsNames[i + 1].text = _showingChimeraData.MainDna.DnaSubSkills[i].SkillName;
                skillsDetails[i + 1].text = _showingChimeraData.MainDna.DnaSubSkills[i].SkillDescription;
                continue;
            }
            skillsImages[i + 1].gameObject.SetActive(false);
            skillsNames[i + 1].text = "";
            skillsDetails[i + 1].gameObject.SetActive(false);
        }
        
        /*
        skillsImages[1].sprite = _showingChimeraData.MainDna.GeneList[0].Sprite;
        skillsImages[2].sprite = _showingChimeraData.MainDna.GeneList[1].Sprite;
        skillsImages[3].sprite = _showingChimeraData.MainDna.GeneList[2].Sprite;

        
        skillsNames[1].text = _showingChimeraData.MainDna.GeneList[0].DnaSubSkill.SkillName;
        skillsNames[2].text = _showingChimeraData.MainDna.GeneList[1].DnaSubSkill.SkillName;
        skillsNames[3].text = _showingChimeraData.MainDna.GeneList[2].DnaSubSkill.SkillName;
        
        
        skillsDetails[1].text = _showingChimeraData.MainDna.GeneList[0].DnaSubSkill.SkillDescription;
        skillsDetails[2].text = _showingChimeraData.MainDna.GeneList[1].DnaSubSkill.SkillDescription;
        skillsDetails[3].text = _showingChimeraData.MainDna.GeneList[2].DnaSubSkill.SkillDescription;*/

        chimeraGeneTypeInfo.text = chimeraGeneType.text;
        
        for (int i = 0; i < chimeraFeaturesInfo.Length; i++)
        {
            chimeraFeaturesInfo[i].text = chimeraFeatures[i].text;
        }
        
        baseStatusInfo[0].text = _showingChimeraData.BaseStatus.MaxHealthPoint.ToString();
        baseStatusInfo[1].text = _showingChimeraData.BaseStatus.AttackPoint.ToString();
        baseStatusInfo[2].text = _showingChimeraData.BaseStatus.DefencePoint.ToString();
        baseStatusInfo[3].text = _showingChimeraData.BaseStatus.AgilityPoint.ToString();

        totalCoefficientInfo[0].text = _showingChimeraData.MainDna.TotalHealthCoefficient.ToString();
        totalCoefficientInfo[1].text = _showingChimeraData.MainDna.TotalAttackCoefficient.ToString();
        totalCoefficientInfo[2].text = _showingChimeraData.MainDna.TotalDefenceCoefficient.ToString();
        totalCoefficientInfo[3].text = _showingChimeraData.MainDna.TotalAgilityCoefficient.ToString();


        SetCoefficientTableByGene();
    }

    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }

    public void BackToManage()
    {
        UiSoundManager.Instance.InDigitalSound();
        ProjectSceneManager.Instance.CallAdditiveScene(7);
    }

    public void ShowDetailInfoPanel()
    {
        UiSoundManager.Instance.InDigitalSound();
        simpleInfoPanel.SetActive(false);
        detailInfoPanel.SetActive(true);
    }
    
    public void CloseDetailInfoPanel()
    {
        UiSoundManager.Instance.InDigitalSound();
        simpleInfoPanel.SetActive(true);
        detailInfoPanel.SetActive(false);
    }

    private void InstantiateChimera()
    {
        UiSoundManager.Instance.InstantiateCagedSound();
        _showingChimeraData.CagedInstantiateChimera();
    }

    private void SetCoefficientTableByGene()
    {
        int geneCount = _showingChimeraData.MainDna.GeneList.Count;
        if (geneCount < 1) return;

        List<Gene> geneList = new List<Gene>(_showingChimeraData.MainDna.GeneList);
        for (int i = 0; i < geneList.Count; i++)
        {
            GameObject panel = Instantiate(coefficientDetailInfoPrefabs, coefficientTableContent);
            for (int j = 0; j < panel.transform.childCount; j++)
            {
                for (int k = 0; k < geneList[i].RandomStatusCoefficient.Count; k++)
                {
                    panel.transform.GetChild(k+1).GetComponent<TextMeshProUGUI>().text = (geneList[i].RandomStatusCoefficient[k] + _showingChimeraData.MainDna.FeatureCoefficientByGene[geneList[i]]).ToString();
                }
            }
            panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = geneList[i].GeneType.ToString();
        }
        
    }
}
