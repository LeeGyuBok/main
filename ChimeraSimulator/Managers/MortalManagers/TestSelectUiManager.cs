using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TestSelectUiManager : MortalManager<TestSelectUiManager>, IGoMain
{
    [SerializeField] private GameObject nonOfficialPanel;
    [SerializeField] private GameObject officialPanel;
    
    [SerializeField] private GameObject playerChimerasPanel;
    [SerializeField] private Transform playerChimerasContent;
    
    [SerializeField] private Button playerChimeraButtonPrefab;
    
    [SerializeField] private GameObject playerChimeraInfoPanel;

    [SerializeField] private GameObject notSetPanel;
    
    [SerializeField] private TextMeshProUGUI chimeraGeneType;
    [SerializeField] private TextMeshProUGUI[] totalStatus = new TextMeshProUGUI[4];
    [SerializeField] private Image[] skillsImages = new Image[4];
    [SerializeField] private TextMeshProUGUI[] skillsNames = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] skillsDetails = new TextMeshProUGUI[4];

    [SerializeField] private Button testEnterButton;

    private Dictionary<Button, UnityAction> chimeraButtonActions;
    private Dictionary<Button, ChimeraData> chimeraByButton;

    protected override void Awake()
    {
        base.Awake();
        chimeraButtonActions = new Dictionary<Button, UnityAction>();
        chimeraByButton = new Dictionary<Button, ChimeraData>();
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<ChimeraData> myChimeras = new List<ChimeraData>(ChimeraManager.Instance.MyChimeraDatas);
        for (int i = 0; i < myChimeras.Count; i++)
        {
            Button button = Instantiate(playerChimeraButtonPrefab, playerChimerasContent);
            if (myChimeras[i].Chimera.IsMutant)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"Mutant\n{myChimeras[i].Chimera.GeneType}";    
            }
            else
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"{myChimeras[i].Chimera.GeneType}";
            }
            UnityAction chimeraButtonAction = () => SetInfo(button);
            chimeraButtonActions[button] = chimeraButtonAction;
            button.onClick.AddListener(chimeraButtonAction);
            chimeraByButton.Add(button, myChimeras[i]);
        }
    }

    private void SetOfficialPanel(bool set)
    {
        nonOfficialPanel.SetActive(set);
        officialPanel.SetActive(set);
        UiSoundManager.Instance.InDigitalSound();
    }

    public void SetTestModeOfficial()
    {
        testEnterButton.onClick.RemoveAllListeners();
        testEnterButton.onClick.AddListener(EnterOfficialTest);
        playerChimerasPanel.SetActive(true);
        SetOfficialPanel(false);
    }

    public void SetTestModeNonOfficial()
    {
        testEnterButton.onClick.RemoveAllListeners();
        testEnterButton.onClick.AddListener(EnterNonOfficialTest);
        playerChimerasPanel.SetActive(true);
        SetOfficialPanel(false);
    }

    public void CloseChimerasPanel()
    {
        playerChimerasPanel.SetActive(false);
        SetOfficialPanel(true);
    }
    
    public void CloseInfoPanel()
    {
        for (int i = 0; i < totalStatus.Length; i++)
        {
            totalStatus[i].text = "Null";
            if (!skillsImages[i].gameObject.activeInHierarchy)
            {
                skillsImages[i].gameObject.SetActive(true);
                skillsNames[i].gameObject.SetActive(true);
                skillsDetails[i].gameObject.SetActive(true);
            }
            skillsImages[i].sprite = null;
            skillsNames[i].text = "Null";
            skillsDetails[i].text = "Null";
        }
        ChimeraManager.Instance.SetTestChimeraData(null);
        playerChimeraInfoPanel.SetActive(false);
        playerChimerasPanel.SetActive(true);
        UiSoundManager.Instance.InDigitalSound();
    }

    private void SetInfo(Button button)
    {
        ChimeraData data = chimeraByButton[button];
        chimeraGeneType.text = data.MainDna.GeneType.ToString();

        totalStatus[0].text = data.MaxHealthPoint.ToString(CultureInfo.CurrentCulture);
        totalStatus[1].text = data.AttackPoint.ToString(CultureInfo.CurrentCulture);
        totalStatus[2].text = data.DefencePoint.ToString(CultureInfo.CurrentCulture);
        totalStatus[3].text = data.AgilityPoint.ToString(CultureInfo.CurrentCulture);

        skillsImages[0].sprite = data.MainDna.Sprite;
        skillsNames[0].text = data.MainDna.DnaMainSkill.SkillName;
        skillsDetails[0].text = data.MainDna.DnaMainSkill.SkillDescription;
        
        for (int i = 0; i < 3 ; i++)
        {
            if (i < data.MainDna.DnaSubSkills.Count)
            {
                skillsImages[i + 1].sprite = Resources.Load<Sprite>($"GeneImage/{data.MainDna.DnaSubSkills[i].GeneType.ToString()}");
                skillsNames[i + 1].text = data.MainDna.DnaSubSkills[i].SkillName;
                skillsDetails[i + 1].text = data.MainDna.DnaSubSkills[i].SkillDescription;
                continue;
            }
            skillsImages[i + 1].gameObject.SetActive(false);
            skillsNames[i + 1].text = "";
            skillsDetails[i + 1].gameObject.SetActive(false);
        }
        
        ChimeraManager.Instance.SetTestChimeraData(data);
        UiSoundManager.Instance.InDigitalSound();
        playerChimeraInfoPanel.SetActive(true);
        playerChimerasPanel.SetActive(false);
    }

    private void EnterOfficialTest()
    {
        /*SetOfficialPanel(false);
        playerChimeraInfoPanel.SetActive(false);
        notSetPanel.SetActive(true);
        return;*/
        UiSoundManager.Instance.InDigitalSound();
        ProjectSceneManager.Instance.CallAdditiveScene(9);
    }
    
    private void EnterNonOfficialTest()
    {
        UiSoundManager.Instance.InDigitalSound();
        ProjectSceneManager.Instance.CallAdditiveScene(10);
    }

    public void CloseNotSetPanel()
    {
        SetOfficialPanel(true);
        notSetPanel.SetActive(false);
    }

    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }
}
