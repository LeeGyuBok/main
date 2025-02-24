using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BackGroundInfoUiManager : MortalManager<BackGroundInfoUiManager>, IGoMain
{
    //시나리오 북을 이아이가 할당해 줘야해요
    //시나리오북은 스크립터블 오브젝트로 만들어서 게임이모탈매니저가 들고있어야해요
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Transform byDateContent;
    [SerializeField] private Transform byResearcherRankContent;
    [SerializeField] private GameObject scenarioEnterPanel;

    [SerializeField] private Button scenarioSelectButton;

    private Dictionary<Button, (BackGroundInfoDataScriptableObject, int)> _backGroundInfoDataScriptableObjects;
    private Dictionary<Button, UnityAction> _buttonActions;

    protected override void Awake()
    {
        base.Awake();
        _backGroundInfoDataScriptableObjects = new Dictionary<Button, (BackGroundInfoDataScriptableObject, int)>();
        _buttonActions = new Dictionary<Button, UnityAction>();
    }

    void Start()
    {
        scenarioEnterPanel.SetActive(false);
        
        for (int i = 0; i < GameImmortalManager.Instance.TimeLineBackgroundInfoData.Count; i++)
        {
            Button button = Instantiate(scenarioSelectButton, byDateContent);
            _backGroundInfoDataScriptableObjects[button] = GameImmortalManager.Instance.TimeLineBackgroundInfoData[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text =
                _backGroundInfoDataScriptableObjects[button].Item2.ToString();
            UnityAction action = () => SetScenario(button);
            _buttonActions[button] = action;
            button.onClick.AddListener(action);
            //20개의 스토리.
            //낮은 순서부터 열리기
            //총 300일.
            //15로나눠.
            //300일 일 때 0번 열림 286일까지는 0번이 열린 상태
            //285일 일 때 1번 열림 271일까지는 1번이 열린 상태
            //270일 일 때 2번 열림
            //타임 스토리가 요구하는 값이 그 날인지?
            //15일마다 열림.
            //ex 277일 일 때,
            //0번 스토리 요구 값 0 > 20 - {올림(20 - 277/15f == 18.46666) == 19} -> 켜진 상태로 둠(300일째에 열리며 현재 277일이면 열린 것)
            //1번 스토리 요구 값 1 > 20 - {올림(20 - 277/15f == 18.46666) == 19} -> 켜진 상태로 둠(285일째에 열리며 현재 277일이면 열린 것)
            //2번 스토리 요구 값 2 > 20 - {올림(20 - 277/15f == 18.46666) == 19} -> 끔.(270일째에 열리며 현재 277일이면 안 열린 것)
            //3번 스토리 요구 값 3 > 20 - {올림(20 - 277/15f == 18.46666) == 19} -> 끔.(255일째에 열리며 현재 277일이면 안 열린 것)
            if (_backGroundInfoDataScriptableObjects[button].Item2 > 20 - Mathf.Ceil(GameImmortalManager.Instance.RemainedDay/(float)15))
            {
                button.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < GameImmortalManager.Instance.ResearcherBackgroundInfoData.Count; i++)
        {
            Button button = Instantiate(scenarioSelectButton, byResearcherRankContent);
            _backGroundInfoDataScriptableObjects[button] = GameImmortalManager.Instance.ResearcherBackgroundInfoData[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text =
                ((ResearcherRank)_backGroundInfoDataScriptableObjects[button].Item2).ToString();
            UnityAction action = () => SetScenario(button);
            _buttonActions[button] = action;
            
            button.onClick.AddListener(action);

            //현재 플레이어의 직급이, 리서처랭크인포가 요구하는 직급보다 낮은경우
            if ((int)AchieveManager.Instance.PlayerRank < _backGroundInfoDataScriptableObjects[button].Item2)
            {
                button.gameObject.SetActive(false);
            }
            else
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    private void SetScenario(Button targetButton)
    {
        UiSoundManager.Instance.AcceptSound();
        GameImmortalManager.Instance.SetScenarioData((_backGroundInfoDataScriptableObjects[targetButton].Item1, _backGroundInfoDataScriptableObjects[targetButton].Item2));
        scenarioEnterPanel.SetActive(true);
    }

    public void EnterScenario()
    {
        UiSoundManager.Instance.SceneTransitionSound();
        ProjectSceneManager.Instance.CallAdditiveScene(13);
    }

    public void CancelScenario()
    {
        UiSoundManager.Instance.CancelSound();
        scenarioEnterPanel.SetActive(false);
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
