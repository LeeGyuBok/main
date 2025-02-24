using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class NonOfficialTestManager: MortalManager<NonOfficialTestManager>, ITestManager
{
    private ResearcherDataScriptableObject _researcherDataScriptableObject;
    public ChimeraData PlayerChimeraData { get; private set; }
    public ChimeraData OpponentChimeraData { get; private set; }
    public ChimeraData WinChimeraData { get; private set; }
    
    public Chimera PlayerChimera { get; private set; }
    public Chimera OpponentChimera { get; private set; }
    
    public CombatMachine CombatMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
    }

    private void Start()
    {
        _researcherDataScriptableObject = ResearcherManager.Instance.GetRandomOpponentResearcher();
        PlayerChimeraData = ChimeraManager.Instance.TestChimeraData;
        OpponentChimeraData = ResearcherManager.Instance.ChimeraDataByResearcher[_researcherDataScriptableObject];
        
        Invoke(nameof(StartTesting), 1.5f);
    }
    private void StartTesting()
    {
        SetChimera();
        
        (List<IChimeraState>, List<IChimeraState>) statesChain = CombatMachine.GetChimeraStatesChain();
        //추후에 결과용지 출력 시 필요할 듯.
        NonOfficialTestUiManager.Instance.TurnOnGoMainButton();
    }
    
    private void SetChimera()
    {
        PlayerChimera = PlayerChimeraData.InstantiateChimera();
        OpponentChimera = OpponentChimeraData.InstantiateChimera();
        CombatMachine = new CombatMachine(PlayerChimera, OpponentChimera, this);
    }

   
    public void WinnerIs(Chimera chimera)
    {
        if (chimera.Equals(PlayerChimera))
        {
            WinChimeraData = PlayerChimeraData;
            AchieveManager.Instance.SetNonOfficialTestVictoryAchieveInfo();
            Gene gene = new Gene((int)AchieveManager.Instance.PlayerRank);
            gene.SetActivationFalse();
            GameImmortalManager.Instance.AddAccountGene(gene);
            if (_researcherDataScriptableObject is MutantResearcherDataScriptableObject mutantResearcher)
            {
                ChimeraManager.Instance.AddDevelopmentableChimera(mutantResearcher.RewardMutantChimera);
                NonOfficialTestUiManager.Instance.SetResultText($"{_researcherDataScriptableObject.ResearcherRank}: {_researcherDataScriptableObject.ResearcherName}", "랜덤 유전자1개, 돌연변이 권한" + mutantResearcher.RewardMutantChimera.GeneType, "승리");
            }
            else
            {
                NonOfficialTestUiManager.Instance.SetResultText($"{_researcherDataScriptableObject.ResearcherRank}: {_researcherDataScriptableObject.ResearcherName}", "랜덤 유전자 1개", "승리");
            }
            ResearcherManager.Instance.SetNewChimeraData(_researcherDataScriptableObject);
        }
        else
        {
            WinChimeraData = OpponentChimeraData;
            AchieveManager.Instance.SetNonOfficialTestDefeatAchieveInfo();
            ChimeraManager.Instance.RemoveDeadChimera(PlayerChimeraData);
            NonOfficialTestUiManager.Instance.SetResultText($"{_researcherDataScriptableObject.ResearcherRank}: {_researcherDataScriptableObject.ResearcherName}", "-", "패배");
        }
        Destroy(PlayerChimera.gameObject);
        Destroy(OpponentChimera.gameObject);
    }
}
