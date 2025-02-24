using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ManagingChimeraSceneUiManager : MortalManager<ManagingChimeraSceneUiManager>, IGoMain
{
    [SerializeField] private Transform myChimerasContent;
    
    [SerializeField] private Button chimeraButton;
    
    private Dictionary<Button, ChimeraData> chimeraByButton;
    
    private Dictionary<Button, UnityAction> chimeraButtonActions;

    [SerializeField] private GameObject reallocationPanel;
    
    [SerializeField] private TextMeshProUGUI[] feature;
    [SerializeField] private TextMeshProUGUI[] baseStatus;

    [SerializeField] private GameObject insertedGeneInfo;
    [SerializeField] private Transform insertedGenesContent;
    
    [SerializeField] private TextMeshProUGUI insertedGeneInfoText;

    private List<Button> _InstantiatedChimeraButtons;
    
    private ChimeraData _reallocationChimeraData;
    private Button _reallocationChimeraButton;
    
    

    private List<ChimeraData> _myChimeras;

    protected override void Awake()
    {
        base.Awake();
        chimeraByButton = new Dictionary<Button, ChimeraData>();
        chimeraButtonActions = new Dictionary<Button, UnityAction>();
        _InstantiatedChimeraButtons = new List<Button>();
    }

    private void Start()
    {
        _myChimeras = new List<ChimeraData>(ChimeraManager.Instance.MyChimeraDatas);
        for (int i = 0; i < _myChimeras.Count; i++)
        {
            Button button = Instantiate(chimeraButton, myChimerasContent);
            UnityAction chimeraButtonAction = () => ShowChimeraDetail(button);
            chimeraButtonActions[button] = chimeraButtonAction;
            button.onClick.AddListener(chimeraButtonAction);
            chimeraByButton.Add(button, _myChimeras[i]);
            button.GetComponentInChildren<TextMeshProUGUI>().text = 
                _myChimeras[i].Chimera.IsMutant ? $"Mutant\n{_myChimeras[i].MainDna.GeneType}" : _myChimeras[i].MainDna.GeneType.ToString();
            _InstantiatedChimeraButtons.Add(button);
        }

        reallocationPanel.SetActive(false);
    }


    public void ReallocationChimera()
    {
        if (!ChimeraManager.Instance.ReallocateChimera(chimeraByButton[_reallocationChimeraButton])) return;
        //버튼 온클릭 리스너 연결 해제
        _reallocationChimeraButton.onClick.RemoveListener(chimeraButtonActions[_reallocationChimeraButton]);
        chimeraButtonActions.Remove(_reallocationChimeraButton);
        
        _myChimeras = new List<ChimeraData>(ChimeraManager.Instance.MyChimeraDatas);
        
        chimeraByButton.Remove(_reallocationChimeraButton);

        _InstantiatedChimeraButtons.Remove(_reallocationChimeraButton);
        Destroy(_reallocationChimeraButton.gameObject);
        //버튼의 온클릭 리스너 삭제
        //해당 키메라 데이터를 할당하는 버튼을 삭제
        //해당 키메라 데이터 삭제.
        AchieveManager.Instance.SetReallocationAchieveInfo();
        UiSoundManager.Instance.AcceptSound();
        reallocationPanel.SetActive(false);
    }
    
    public void CancelReallocation()
    {
        UiSoundManager.Instance.CancelSound();
        reallocationPanel.SetActive(false);
    }

    private void ShowChimeraDetail(Button button)
    {
        GameImmortalManager.Instance.ShowCagedChimera(chimeraByButton[button]);
        //생성되었던 모든 버튼에 대해서
        for (int i = 0; i < _InstantiatedChimeraButtons.Count; i++)
        {
            //리스너 지우고
            _InstantiatedChimeraButtons[i].onClick.RemoveAllListeners();
            //액션 지우고
            chimeraButtonActions.Remove(_InstantiatedChimeraButtons[i]);
            //키메라 지우고
            chimeraByButton.Remove(_InstantiatedChimeraButtons[i]);
        }
        //버튼 모두 지우기
        _InstantiatedChimeraButtons.Clear();
        UiSoundManager.Instance.SceneTransitionSound();
        ProjectSceneManager.Instance.CallAdditiveScene(6);
        /*Chimera chimera = chimeraByButton[button].CagedInstantiateChimera();
        Debug.Log(chimera.GeneType.ToString());
        Debug.Log(chimera.MaxHealthPoint);
        Debug.Log(chimera.AttackPoint);
        Debug.Log(chimera.DefencePoint);
        Debug.Log(chimera.AgilityPoint);*/
        //씬전환
    }

    public void ShowReallocationPanel(Button button)
    {
        UiSoundManager.Instance.InDigitalSound();
        reallocationPanel.SetActive(true);

        _reallocationChimeraButton = button;
        _reallocationChimeraData = chimeraByButton[_reallocationChimeraButton];
        
        feature[0].text = _reallocationChimeraData.MainDna.GeneType.ToString();
        feature[1].text = _reallocationChimeraData.MainDna.DnaFeatureList.Features[0].ToString();
        feature[2].text = _reallocationChimeraData.MainDna.DnaFeatureList.Features[1].ToString();
        feature[3].text = _reallocationChimeraData.MainDna.DnaFeatureList.Features[2].ToString();

        baseStatus[0].text = _reallocationChimeraData.MaxHealthPoint.ToString(CultureInfo.InvariantCulture);
        baseStatus[1].text = _reallocationChimeraData.AttackPoint.ToString(CultureInfo.InvariantCulture);
        baseStatus[2].text = _reallocationChimeraData.DefencePoint.ToString(CultureInfo.InvariantCulture);
        baseStatus[3].text = _reallocationChimeraData.AgilityPoint.ToString(CultureInfo.InvariantCulture);


        for (int i = 0; i < _reallocationChimeraData.MainDna.GeneList.Count; i++)
        {
            TextMeshProUGUI textObject = Instantiate(insertedGeneInfoText, insertedGenesContent);
            textObject.text =
                $"{_reallocationChimeraData.MainDna.GeneList[i].GeneType.ToString()} / " +
                $"{_reallocationChimeraData.MainDna.GeneList[i].RandomStatusCoefficient[0]}/" +
                $"{_reallocationChimeraData.MainDna.GeneList[i].RandomStatusCoefficient[1]}/" +
                $"{_reallocationChimeraData.MainDna.GeneList[i].RandomStatusCoefficient[2]}/" +
                $"{_reallocationChimeraData.MainDna.GeneList[i].RandomStatusCoefficient[3]}";
        }
    }

    public void GoMain()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(0);
    }

    public void GoBack()
    {
        UiSoundManager.Instance.SceneTransitionSound();
        ProjectSceneManager.Instance.CallAdditiveScene(3);
    }
}
