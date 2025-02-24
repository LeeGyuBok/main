using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class OfficialTestManager : MortalManager<OfficialTestManager>, ITestManager
{
    private ResearcherDataScriptableObject _researcherDataScriptableObject;
    private List<ResearcherDataScriptableObject> _researcherDataScriptableObjects;
    public ChimeraData PlayerChimeraData { get; private set; }
    public ChimeraData OpponentChimeraData { get; private set; }
    public ChimeraData WinChimeraData { get; private set; }
    
    public Chimera PlayerChimera { get; private set; }
    public Chimera OpponentChimera { get; private set; }
    public CombatMachine CombatMachine { get; private set; }
    
    private int _totalWinCount = 0;
    
    private readonly WaitForSeconds _testDelay = new (1.5f);

    public static int JrCount = 6;
    public static int SrPrCount = 4;
    public static int DrCount = 2;
    
    public static int TotalResearchers = JrCount + SrPrCount + DrCount;
    

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        /*//초기 테스트용코드
        _researcherDataScriptableObject = ResearcherManager.Instance.GetRandomOpponentResearcher();*/
        _researcherDataScriptableObjects = new List<ResearcherDataScriptableObject>();

        PickAndInsertResearchers(new List<ResearcherDataScriptableObject>(ResearcherManager.Instance.TotalJrResearchers));
        PickAndInsertResearchers(new List<ResearcherDataScriptableObject>(ResearcherManager.Instance.TotalSrPrResearchers));
        PickAndInsertResearchers(new List<ResearcherDataScriptableObject>(ResearcherManager.Instance.TotalDrResearchers));
        
        PlayerChimeraData = ChimeraManager.Instance.TestChimeraData;
        
        StartTesting();
    }
    
    public void StartTesting()
    {
        SetChimera();
        
        StartCoroutine(TestResultDelay());
        //테스트 시작 및 종료
        //종료시, 결과창 출력.
    }

    private void SetChimera()
    {
        _researcherDataScriptableObject = _researcherDataScriptableObjects[_totalWinCount];
        OpponentChimeraData = ResearcherManager.Instance.ChimeraDataByResearcher[_researcherDataScriptableObject];
        
        PlayerChimera = PlayerChimeraData.InstantiateChimera();
        OpponentChimera = OpponentChimeraData.InstantiateChimera();

        if (CombatMachine == null)
        {
            CombatMachine = new CombatMachine(PlayerChimera, OpponentChimera, this);    
        }
        else
        {
            CombatMachine.RestartCombat(PlayerChimera, OpponentChimera);
        }
        
    }
    
    public void WinnerIs(Chimera chimera)
    {
        if (chimera.Equals(PlayerChimera))
        {
            WinChimeraData = PlayerChimeraData;
            
            Gene gene = new Gene((int)AchieveManager.Instance.PlayerRank);
            gene.SetActivationFalse();
            GameImmortalManager.Instance.AddAccountGene(gene);
            
            if (_researcherDataScriptableObject is MutantResearcherDataScriptableObject mutantResearcher)
            {
                if (ChimeraManager.Instance.AddDevelopmentableChimera(mutantResearcher.RewardMutantChimera))
                {
                    OfficialTestUiManager.Instance.SetResultText($"{_researcherDataScriptableObject.ResearcherRank}: " +
                                                                 $"{_researcherDataScriptableObject.ResearcherName}", 
                        "랜덤 유전자1개, 돌연변이 권한: " + mutantResearcher.RewardMutantChimera.GeneType, "승리");    
                }
                else
                {
                    OfficialTestUiManager.Instance.SetResultText($"{_researcherDataScriptableObject.ResearcherRank}: " +
                                                                 $"{_researcherDataScriptableObject.ResearcherName}", 
                        "랜덤 유전자1개: " + mutantResearcher.RewardMutantChimera.GeneType, "승리");
                }
            }
            else
            {
                OfficialTestUiManager.Instance.SetResultText($"{_researcherDataScriptableObject.ResearcherRank}: " +
                                                             $"{_researcherDataScriptableObject.ResearcherName}", 
                    "랜덤 유전자 1개", "승리");
            }
            ResearcherManager.Instance.SetNewChimeraData(_researcherDataScriptableObject);
            _totalWinCount++;
            if (_researcherDataScriptableObjects.Count < _totalWinCount)
            {
                OfficialTestUiManager.Instance.SetResultText($"{_researcherDataScriptableObject.ResearcherRank}: {_researcherDataScriptableObject.ResearcherName}", "-", "승리", _totalWinCount);
                AchieveManager.Instance.SetOfficialTestAchieveInfo(_totalWinCount);
                OfficialTestUiManager.Instance.SetNextMatchButton(false);
                OfficialTestUiManager.Instance.TurnOnGoMainButton();
                return;
            }
            OfficialTestUiManager.Instance.SetNextMatchButton(true);
        }
        else
        {
            WinChimeraData = OpponentChimeraData;
            AchieveManager.Instance.SetOfficialTestAchieveInfo(_totalWinCount);
            ChimeraManager.Instance.RemoveDeadChimera(PlayerChimeraData);
            OfficialTestUiManager.Instance.SetResultText($"{_researcherDataScriptableObject.ResearcherRank}: {_researcherDataScriptableObject.ResearcherName}", "-", "패배", _totalWinCount);
            OfficialTestUiManager.Instance.SetNextMatchButton(false);
            OfficialTestUiManager.Instance.TurnOnGoMainButton();
        }
        Destroy(PlayerChimera.gameObject);
        Destroy(OpponentChimera.gameObject);
    }

    private void PickAndInsertResearchers(List<ResearcherDataScriptableObject> listData)
    {
        Shuffle(listData);
        
        switch (listData[0].ResearcherRank)
        {
            case ResearcherRank.Junior:
                _researcherDataScriptableObjects.AddRange(listData.GetRange(0, JrCount));
                break;
            case ResearcherRank.Senior:
            case ResearcherRank.Principal:
                _researcherDataScriptableObjects.AddRange(listData.GetRange(0, SrPrCount));
                break;
            case ResearcherRank.Director:
                _researcherDataScriptableObjects.AddRange(listData.GetRange(0, DrCount));
                break;
            default:
                //Debug.Log("Researcher Rank Not Supported");
                break;
                
        }
    }
    
    // Fisher-Yates Shuffle 알고리즘
    private void Shuffle<T>(List<T> list)
    {
        Random rand = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]); // Swap
        }
    }

    private IEnumerator TestResultDelay()
    {
        yield return _testDelay;
        (List<IChimeraState>, List<IChimeraState>) statesChain = CombatMachine.GetChimeraStatesChain();
        //추후에 결과용지 출력 시 필요할 듯.
    }
}