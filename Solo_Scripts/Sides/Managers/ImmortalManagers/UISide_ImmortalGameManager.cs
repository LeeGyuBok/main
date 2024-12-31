using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public enum PlayMode
{
    Archive,
    CardGame
}
public class UISide_ImmortalGameManager : ImmortalObject<UISide_ImmortalGameManager>
{
    [SerializeField] private List<FriendlyOperator> characterPrefab;
    public List<FriendlyOperator> CharacterPrefab => characterPrefab;

    public FriendlyOperator SelectedOperator { get; private set; }
    
    public ScenarioJson selectedScenarioJsonData { get; private set; }
    
    public ScenarioBook selectedScenarioBook { get; private set; }
    
    public int OperatorCurrentTrust { get; private set; }
    
    public PlayMode SelectedPlayMode { get; private set; }

    public void SetPlayMode(PlayMode playMode)
    {
        SelectedPlayMode = playMode;
    }
    
    public void SelectOperator(FriendlyOperator selectOperator)
    {
        SelectedOperator = selectOperator;
        selectedScenarioJsonData = UISide_ImmortalJsonManager.Instance.ReadScenarioData(SelectedOperator.TrustData.CharacterName);
        OperatorCurrentTrust = selectedScenarioJsonData.CurrentTrust;
    }

    public void SetScenarioBook(ScenarioBook scenarioBook)
    {
        selectedScenarioBook = scenarioBook;
    }

    public void DeselectOperator()
    {
        SelectedOperator = null;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // PlayMode 종료
#else
                Application.Quit(); // 빌드된 환경에서 게임 종료
#endif    
    }
}
