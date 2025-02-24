using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LobbyUiManager : MortalManager<LobbyUiManager>
{
    [SerializeField] private Button createChimeraButton;
    [SerializeField] private Button chimeraAndGeneButton;
    [SerializeField] private Button chimeraManagementButton;
    [SerializeField] private Button officialNonofficialExperimentButton;
    [SerializeField] private Button getChimeraGeneButton;
    [SerializeField] private Button exitAlertButton;
    [SerializeField] private Button exitCancelButton;
    [SerializeField] private Button exitGameButton;
    
    [SerializeField] private GameObject exitAlertPanel;

    [SerializeField] private TextMeshProUGUI remainDay;
    [SerializeField] private TextMeshProUGUI supplyGeneTokens;
    [SerializeField] private TextMeshProUGUI supplyChimeraTokens;
    
    private List<UnityAction> actions;

    protected override void Awake()
    {
        base.Awake();
        actions = new List<UnityAction>();
    }

    private void Start()
    {
        exitAlertPanel.SetActive(false);
        TurnOn();
        SetDailyResources(GameImmortalManager.Instance.RemainedDay, 
            GameImmortalManager.Instance.GeneSupplyTokenCount, 
            GameImmortalManager.Instance.ChimeraSupplyTokenCount);
    }

    private void SetDailyResources(int remainedDay, int geneTokens, int chimeraTokens)
    {
        remainDay.text = remainedDay.ToString();
        supplyGeneTokens.text = geneTokens.ToString();
        supplyChimeraTokens.text = chimeraTokens.ToString();
    }

    public void WhenCheat(int remainedDay, int geneTokens, int chimeraTokens)
    {
        SetDailyResources(remainedDay, geneTokens, chimeraTokens);
    }

    private void OnDestroy()
    {
        TurnOff();
    }

    private void TurnOn()
    {
        actions.Add(ClickCreateChimeraButton);
        actions.Add(ClickChimeraAndGeneButton);
        actions.Add(ClickChimeraManagementButton);
        actions.Add(ClickOfficialNonofficialExperimentButton);
        actions.Add(ClickGetChimeraGeneButton);
        actions.Add(ExitAlert);
        actions.Add(ExitCancel);
        actions.Add(ClickExitLabButton);
        
        createChimeraButton.onClick.AddListener(actions[0]);
        chimeraAndGeneButton.onClick.AddListener(actions[1]);
        chimeraManagementButton.onClick.AddListener(actions[2]);
        officialNonofficialExperimentButton.onClick.AddListener(actions[3]);
        getChimeraGeneButton.onClick.AddListener(actions[4]);
        exitAlertButton.onClick.AddListener(actions[5]);
        exitCancelButton.onClick.AddListener(actions[6]);
        exitGameButton.onClick.AddListener(actions[7]);
    }
    private void TurnOff()
    {
        createChimeraButton.onClick.RemoveListener(actions[0]);
        chimeraAndGeneButton.onClick.RemoveListener(actions[1]);
        chimeraManagementButton.onClick.RemoveListener(actions[2]);
        officialNonofficialExperimentButton.onClick.RemoveListener(actions[3]);
        getChimeraGeneButton.onClick.RemoveListener(actions[4]);
        exitAlertButton.onClick.RemoveListener(actions[5]);
        exitCancelButton.onClick.RemoveListener(actions[6]);
        exitGameButton.onClick.RemoveListener(actions[7]);
        
        actions.Clear();
    }

    private void ClickCreateChimeraButton()
    {
        UiSoundManager.Instance.EnterInsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(1);
    }

    private void ClickChimeraAndGeneButton()
    {
        UiSoundManager.Instance.EnterInsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(2);
    }

    private void ClickChimeraManagementButton()
    {
        UiSoundManager.Instance.EnterInsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(3);
    }

    private void ClickOfficialNonofficialExperimentButton()
    {
        UiSoundManager.Instance.EnterInsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(4);
    }

    private void ClickGetChimeraGeneButton()
    {
        UiSoundManager.Instance.EnterInsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(5);
    }

    private void ExitAlert()
    {
        UiSoundManager.Instance.AcceptSound();
        exitAlertPanel.SetActive(true);
    }

    private void ExitCancel()
    {
        UiSoundManager.Instance.CancelSound();
        exitAlertPanel.SetActive(false);
    }

    private void ClickExitLabButton()
    {
        UiSoundManager.Instance.EnterOutsideSound();
        ProjectSceneManager.Instance.CallAdditiveScene(11);
    }
}
