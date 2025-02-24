using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AuthorityUiManager : MortalManager<AuthorityUiManager>, IGoMain 
{
    [SerializeField] private GameObject accountPanel;
    [SerializeField] private List<Button> mainFunctionButtons;
    
    [SerializeField] private TextMeshProUGUI[] playerInfos;
    
    
    [SerializeField] private GameObject developmentableChimeraListPanel;
    [SerializeField] private Transform developmentableChimeraListContent;
    
    [SerializeField] private GameObject myGeneListPanel;
    [SerializeField] private TMP_Dropdown myGeneListDropdown;
    [SerializeField] private Transform myGeneListDropDownContent;

    [SerializeField] private Transform myGeneListContent;
    [SerializeField] private Button myGeneListGeneButton;
    
    [SerializeField] private Button developmentableChimeraButtonPrefab;
    [SerializeField] private Button backgroundInfoButton;
    
    private Dictionary<Button, Chimera> _chimeraByButtons;
    private Dictionary<Button, UnityAction> _chimeraButtonActions;
    private Dictionary<Button, UnityAction> _geneButtonActions;
    
    private Dictionary<Button, Gene> _geneByButtons;

    private Dictionary<Button, bool> _clickedButtons;
    
    private List<GameObject> _basicChimeraButtons;
    private List<GameObject> _mutantChimeraButtons;

    private int _categoryIndex;

    protected override void Awake()
    {
        base.Awake();
        _chimeraByButtons = new Dictionary<Button, Chimera>();
        _chimeraButtonActions = new Dictionary<Button, UnityAction>();
        _geneButtonActions = new Dictionary<Button, UnityAction>();
        _clickedButtons = new Dictionary<Button, bool>();
        _basicChimeraButtons = new List<GameObject>();
        _mutantChimeraButtons = new List<GameObject>();
        _geneByButtons = new Dictionary<Button, Gene>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<Chimera>tempChimeras = new List<Chimera>(ChimeraManager.Instance.DevelopmentableChimeras);
        for (int i = 0; i < ChimeraManager.Instance.DevelopmentableChimeras.Count; i++)
        {
            Button button = Instantiate(developmentableChimeraButtonPrefab, developmentableChimeraListContent);
            UnityAction action = () => ShowSelectedChimera(button);
            _chimeraButtonActions[button] = action;
            button.onClick.AddListener(action);
            _chimeraByButtons[button] = tempChimeras[i];
            if (tempChimeras[i].IsMutant)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"{tempChimeras[i].GeneType}: 변종";
                _mutantChimeraButtons.Add(button.gameObject);
            }
            else
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"{tempChimeras[i].GeneType}: 원 종";
                _basicChimeraButtons.Add(button.gameObject);
            }
            
            _clickedButtons.Add(button, false);
            
            playerInfos[0].text = GameImmortalManager.Instance.PlayerName;
            playerInfos[1].text = AchieveManager.Instance.PlayerRank.ToString();
            playerInfos[2].text = AchieveManager.Instance.HighestOfficialTestClearStage.ToString();
            playerInfos[3].text = AchieveManager.Instance.TotalNonOfficialTestVictoryCount.ToString();
            playerInfos[4].text = AchieveManager.Instance.TotalNonOfficialTestDefeatCount.ToString();
            playerInfos[5].text = AchieveManager.Instance.TotalReallocationCount.ToString();
            playerInfos[6].text = AchieveManager.Instance.TotalDevelopmentChimeraCount.ToString();
        }

        //드롭다운은 하이어라키의 아이템을 지우면안된다. 그 아이를 기준으로 내부 토글들을 만들기 때문이다..
        
        myGeneListDropdown.ClearOptions();
        List<string> optionData = new List<string>()
        {
            "All"
        };
        for (int i = 0; i < Enum.GetValues(typeof(GeneType)).Length-1; i++)
        {
            optionData.Add(((GeneType)i).ToString());
        }
        myGeneListDropdown.AddOptions(optionData);
        myGeneListDropdown.onValueChanged.AddListener(DropDownValueChanged);
        
        //같은 토글을 눌렀을 때, 다시 켜고싶은뎅..
        
        List<Gene> tempGeneList = new List<Gene>(GameImmortalManager.Instance.AccountUseAbleGene);
        for (int i = 0; i < tempGeneList.Count; i++)
        {
            Button button = Instantiate(myGeneListGeneButton, myGeneListContent);
            
           
           
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{tempGeneList[i].GeneType} / {tempGeneList[i].GeneGrade}";
            UnityAction action = () => ShowSelectedGene(button);
            _geneButtonActions[button] = action;
            button.onClick.AddListener(action);
            _geneByButtons[button] = tempGeneList[i];

            _clickedButtons.Add(button, false);
        }
    }

    private void MainButtonSetter(bool set)
    {
        for (int i = 0; i < mainFunctionButtons.Count; i++)
        {
            mainFunctionButtons[i].gameObject.SetActive(set);
        }
    }
    
    public void MyChimera()
    {
        UiSoundManager.Instance.SceneTransitionSound();
        ProjectSceneManager.Instance.CallAdditiveScene(7);
    }
    
    public void DevelopmentableChimeraList()
    {
        MainButtonSetter(false);
        UiSoundManager.Instance.InDigitalSound();
        developmentableChimeraListPanel.SetActive(true);
    }
    
    public void CloseDevelopmentableChimeraList()
    {
        MainButtonSetter(true);
        UiSoundManager.Instance.InDigitalSound();
        developmentableChimeraListPanel.SetActive(false);
    }

    private void ShowSelectedChimera(Button button)
    {
        if (_clickedButtons[button]) return;
        _clickedButtons[button] = true;
        BaseStatus status;
        FeatureList featureList;
        if (_chimeraByButtons[button].IsMutant)
        {
            status = ImmortalScriptableObjectManager.Instance.MutantStatusesByGeneType[_chimeraByButtons[button].GeneType];
            featureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[_chimeraByButtons[button].GeneType];
            StartCoroutine(SwitchChimeraInfo(button, status, featureList));
        }
        else
        {
            status = ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[_chimeraByButtons[button].GeneType];
            featureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[_chimeraByButtons[button].GeneType];
            StartCoroutine(SwitchChimeraInfo(button, status, featureList));
        }
        UiSoundManager.Instance.InDigitalSound();
    }
    
    private void ShowSelectedGene(Button button)
    {
        if (_clickedButtons[button]) return;
        _clickedButtons[button] = true;
        StartCoroutine(SwitchGeneInfo(button));
        UiSoundManager.Instance.InDigitalSound();
    }

    //레이아웃그룹 컴포넌트가 자식오브젝트들의 순서를 관리한다. 확인결과, setActive(false)할 때, content내에서 하이어라키에 활성하되지 않은 오브젝트의 포지션이 변하지 않았다.
    
    public void ShowAllDevelopmentableChimera()
    {
        SetBasicChimeraButtons(true);
        SetMutantChimeraButtons(true);
        UiSoundManager.Instance.InDigitalSound();
    }

    public void ShowBasicDevelopmentableChimera()
    {
        SetBasicChimeraButtons(true);
        SetMutantChimeraButtons(false);
        UiSoundManager.Instance.InDigitalSound();
    }

    public void ShowMutantDevelopmentableChimera()
    {
        SetBasicChimeraButtons(false);
        SetMutantChimeraButtons(true);
        UiSoundManager.Instance.InDigitalSound();
    }
    
    private void SetBasicChimeraButtons(bool set)
    {
        for (int i = 0; i < _basicChimeraButtons.Count; i++)
        {
            _basicChimeraButtons[i].SetActive(set);
        }
    }
    
    private void SetMutantChimeraButtons(bool set)
    {
        for (int i = 0; i < _mutantChimeraButtons.Count; i++)
        {
            _mutantChimeraButtons[i].SetActive(set);
        }
    }

    private IEnumerator SwitchChimeraInfo(Button button, BaseStatus status, FeatureList featureList)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{status.MaxHealthPoint}/{status.AttackPoint}/{status.DefencePoint}/{status.AgilityPoint} / {featureList.Features[0]}/{featureList.Features[1]}/{featureList.Features[2]}";
        yield return new WaitForSeconds(3f);
        button.GetComponentInChildren<TextMeshProUGUI>().text =
            $"{_chimeraByButtons[button].GeneType}: {(_chimeraByButtons[button].IsMutant ? "변종" : "원 종")}";
        _clickedButtons[button] = false;
    }
    
    private IEnumerator SwitchGeneInfo(Button button)
    {
        Gene gene = _geneByButtons[button];
        button.GetComponentInChildren<TextMeshProUGUI>().text = 
            $"HP:{gene.RandomStatusCoefficient[0]} / ATK:{gene.RandomStatusCoefficient[1]} / DFE:{gene.RandomStatusCoefficient[2]} / AGI:{gene.RandomStatusCoefficient[3]}";
        yield return new WaitForSeconds(3f);
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{gene.GeneType} / {gene.GeneGrade}";
        _clickedButtons[button] = false;
    }
    
    public void SetMyGeneList()
    {
        UiSoundManager.Instance.InDigitalSound();
        MainButtonSetter(false);
        myGeneListPanel.SetActive(true);
    }
    
    public void CloseMyGeneList()
    {
        UiSoundManager.Instance.InDigitalSound();
        MainButtonSetter(true);
        myGeneListPanel.SetActive(false);
    }

    private void DropDownValueChanged(int index)
    {
        //index는 enum에서 각각의 Genetype이 갖는 int 값.
        //index로 정렬해주세요
        //유전자가 없네요 ~
        if (_geneByButtons.Count == 0) return;
        //index가 0이라는 뜻인 다시 전체를 눌렀다는 뜻.
        //전체를 보여달라고 하네요~
        if (index == 0)
        {
            foreach (var kvp in _geneByButtons)
            {
                kvp.Key.gameObject.SetActive(true);
            }
            myGeneListContent.gameObject.SetActive(true);
            _categoryIndex = index;
            return;
        }
        //이 번호 접수했습니다.
        _categoryIndex = index;
        foreach (var kvp in _geneByButtons)
        {
            kvp.Key.gameObject.SetActive((int)kvp.Value.GeneType == index-1);
        }
        UiSoundManager.Instance.InDigitalSound();
        myGeneListContent.gameObject.SetActive(true);
    }

    public void EnterSameToggle(bool sameToggle)
    {
        if (!sameToggle) return;
        myGeneListContent.gameObject.SetActive(true);
    }

    public void TurnOffMyGeneListContent()
    {
        if (myGeneListContent.gameObject.activeInHierarchy)
        {
            //Debug.Log("Turning off My Gene List");
            myGeneListContent.gameObject.SetActive(false);    
        }
    }
    
    public void Achieve()
    {
        UiSoundManager.Instance.SceneTransitionSound();
        ProjectSceneManager.Instance.CallAdditiveScene(8);
    }

    public void BackgroundInfoButton()
    {
        UiSoundManager.Instance.SceneTransitionSound();
        ProjectSceneManager.Instance.CallAdditiveScene(12);
    }

    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }
}
