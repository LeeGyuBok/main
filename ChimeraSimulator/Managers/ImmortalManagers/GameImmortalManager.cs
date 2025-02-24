using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameImmortalManager : ImmortalObject<GameImmortalManager>
{
    //기본데이터들
    [SerializeField] private List<Chimera> basicChimeras;
    [SerializeField] private List<Chimera> rewardChimera;
    public Dictionary<GeneType, Chimera> DictionaryChimera { get; private set; }
    public List<Gene> AllGenes { get; private set; }
    public List<Chimera> AllChimeras { get; private set; }
    
    //이 계정 또는 사용자 또는 플레이어가 사용할 수 있는 데이터들
    public List<Gene> AccountUseAbleGene { get; private set; }

    public const int MyChimerasCapacity = 100;
    private const int MyGenesCapacity = 300;

    private int _testGeneCount = 0;
    
    //Cage Scene 용 변수
    public ChimeraData CageChimeraData { get; private set; }
    
    //게임 진행용 변수
    public const int BasicRemainDay = 301;
    public string PlayerName { get; private set; } = "";
    public int RemainedDay { get; private set; } = BasicRemainDay;
    
    public int GeneSupplyTokenCount { get; private set; } = 0;
    public int ChimeraSupplyTokenCount { get; private set; } = 0;
    
    
    public List<(BackGroundInfoDataScriptableObject, int)> TimeLineBackgroundInfoData { get; private set; }
    public List<(BackGroundInfoDataScriptableObject, int)> ResearcherBackgroundInfoData { get; private set; }
    public List<BackGroundInfoDataScriptableObject> EndingInfoData { get; private set; }
    public List<ScenarioJson> testScenarios { get; private set; }
    public (BackGroundInfoDataScriptableObject, int) ScenarioDataObject { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AccountUseAbleGene = new List<Gene>(MyGenesCapacity);
        
        AllGenes = new List<Gene>(Enum.GetValues(typeof(GeneType)).Length-1);
        AllChimeras = new List<Chimera>(basicChimeras.Count + rewardChimera.Count);
        TimeLineBackgroundInfoData = new List<(BackGroundInfoDataScriptableObject, int)>();
        ResearcherBackgroundInfoData = new List<(BackGroundInfoDataScriptableObject, int)>();
        testScenarios = new List<ScenarioJson>();
        EndingInfoData = new List<BackGroundInfoDataScriptableObject>();
        DictionaryChimera = new Dictionary<GeneType, Chimera>();
        
        for (int i = 0; i < Enum.GetValues(typeof(GeneType)).Length-1; i++)
        {
            AllGenes.Add(new Gene((GeneType)i));
        }
        
    }

    private void Start()
    {
        //나중에 계정정보 받아서 넣어주는 것으로 바꿔야함.
        for (int i = 0; i < basicChimeras.Count; i++)
        {
            DictionaryChimera.Add(basicChimeras[i].GeneType, basicChimeras[i]);
            if (!ChimeraManager.Instance.AddDevelopmentableChimera(basicChimeras[i])) break;
        }
        AllChimeras.AddRange(basicChimeras);
        AllChimeras.AddRange(rewardChimera);
        SetBackgroundInfoData();
        /*for (int i = 0; i < rewardChimera.Count; i++)
        {
            if (!ChimeraManager.Instance.AddDevelopmentableChimera(rewardChimera[i])) return;
        }*/
        
        //각 리스트를 순차 추가
        
        /*Invoke(nameof(SetSoBackgroundInfoData), 1f);*/

        
    }

    private void Update()
    {
        if (ProjectSceneManager.Instance.CurrentSceneName.Equals("LobbyScene") && Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                RemainedDay = 100;
                for (int i = 0; i < 46; i++)
                {
                    AchieveManager.Instance.SetReallocationAchieveInfo();
                    AchieveManager.Instance.SetDevelopmentChimeraAchieveInfo();
                    AchieveManager.Instance.SetNonOfficialTestVictoryAchieveInfo();
                    AchieveManager.Instance.SetNonOfficialTestDefeatAchieveInfo();
                }

                for (int i = 0; i < 78; i++)
                {
                    AddAccountGene(new Gene((int)AchieveManager.Instance.PlayerRank));    
                }
                LobbyUiManager.Instance.WhenCheat(RemainedDay, GeneSupplyTokenCount ,ChimeraSupplyTokenCount);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                AchieveManager.Instance.SetReallocationAchieveInfo();
                AchieveManager.Instance.SetDevelopmentChimeraAchieveInfo();
                AchieveManager.Instance.SetNonOfficialTestVictoryAchieveInfo();
                AchieveManager.Instance.SetNonOfficialTestDefeatAchieveInfo();
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                RemainedDay = 1;
                LobbyUiManager.Instance.WhenCheat(RemainedDay, GeneSupplyTokenCount ,ChimeraSupplyTokenCount);
            }
        }
    
        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                RemainedDay-=5;
                LobbyUiManager.Instance.WhenCheat(RemainedDay, GeneSupplyTokenCount ,ChimeraSupplyTokenCount);
            }
        }
    }

    #region AtStartGame

    public void SetPlayerName(string playerName)
    {
        PlayerName = playerName;
    }
    
    public void SetInitialGameResources()
    {
        if (RemainedDay != BasicRemainDay)
        {
            RemainedDay = BasicRemainDay;
            Instance.ResetGameResources();
        }
        AddDailySupplyTokens();
    }

    private void SetBackgroundInfoData()
    {
        //파일을 숫자로 인덱싱한 경우, 그 자리숫자를 맞춰줘야한다. ex) 00 ~ 99 이런식으로
        List<ScenarioJson> scenarios = BackgroundInfoJsonManager.Instance.ReadAllScenarioData();
        
        Dictionary<int, ScenarioJson> tempDict = new Dictionary<int, ScenarioJson>();
        
        List<TimeLineScenario> timeLineScenarios = new List<TimeLineScenario>();
        List<ResearcherRankScenario> researcherRankScenarios = new List<ResearcherRankScenario>();

        foreach (var scenario in scenarios)
        {
            //Debug.Log($"{scenario is TimeLineScenario} | {scenario is ResearcherRankScenario}");

            switch (scenario)
            {
                case TimeLineScenario ts:
                    timeLineScenarios.Add(ts);
                    break;
                case ResearcherRankScenario rsr:
                    researcherRankScenarios.Add(rsr);
                    break;
            }
        }
        
        //Debug.Log(timeLineScenarios.Count);

        for (int i = 0; i < timeLineScenarios.Count; i++)
        {
            tempDict[timeLineScenarios[i].NeedTimeLineDate] = timeLineScenarios[i];
            //Debug.Log($"{i}, {timeLineScenarios[i].NeedTimeLineDate}");
        }

        for (int i = 0; i < tempDict.Count; i++)
        {
            BackGroundInfoDataScriptableObject timeLineScenario =
                ScriptableObject.CreateInstance<BackGroundInfoDataScriptableObject>();
            timeLineScenario.SetBooks(tempDict[i].ScenarioBooks);
            if (i < 20)
            {
                TimeLineBackgroundInfoData.Add((timeLineScenario, timeLineScenarios[i].NeedTimeLineDate));
                //Debug.Log(timeLineScenarios[i].NeedTimeLineDate);
                continue;
            }
            EndingInfoData.Add(timeLineScenario);
        }
        
        tempDict.Clear();
        
        for (int i = 0; i < researcherRankScenarios.Count; i++)
        {
            tempDict[researcherRankScenarios[i].NeedResearcherRankInt] = researcherRankScenarios[i];
        }

        for (int i = 0; i < tempDict.Count; i++)
        {
            BackGroundInfoDataScriptableObject researcherRankScenario =
                ScriptableObject.CreateInstance<BackGroundInfoDataScriptableObject>();
            researcherRankScenario.SetBooks(tempDict[i+1].ScenarioBooks);
            ResearcherBackgroundInfoData.Add((researcherRankScenario, researcherRankScenarios[i].NeedResearcherRankInt));
        }

        tempDict.Clear();
    }

    #endregion

    #region InLab
    
    public void AddDailySupplyTokens()
    {
        RemainedDay--;
        if (RemainedDay == 0)
        {
            GetEndingScene();
            return;
        }
        GeneSupplyTokenCount += 2;
        
        if (RemainedDay % 7 == 6)
        {
            ChimeraSupplyTokenCount += 1;    
        }
        
    }

    public bool UseGeneTokens()
    {
        if (GeneSupplyTokenCount == 0) return false;
        GeneSupplyTokenCount--;
        return true;
    }

    public bool UseChimeraTokens()
    {
        if (ChimeraSupplyTokenCount == 0) return false;
        ChimeraSupplyTokenCount--;
        return true;
    }

    public void AddAccountGene(Gene gene)
    {
        AccountUseAbleGene.Add(gene);
        _testGeneCount = AccountUseAbleGene.Count;
    }
    
    public void AddAccountChimeraData(ChimeraData chimera, bool inGetRandom = false)
    {
        ChimeraManager.Instance.AddNewMyChimera(chimera);
        if (inGetRandom) return;
        AchieveManager.Instance.SetDevelopmentChimeraAchieveInfo();
    }

    public void ShowCagedChimera(ChimeraData chimera)
    {
        CageChimeraData = chimera;
    }

    public Chimera GetChimeraByGeneType(GeneType geneType, bool isMutant = false)
    {
        Chimera selectedChimera;
        if (isMutant)
        {
            selectedChimera = rewardChimera.Find(chimera => chimera.GeneType.Equals(geneType));
            return selectedChimera;
        }
        selectedChimera = basicChimeras.Find(chimera => chimera.GeneType.Equals(geneType));
        return selectedChimera;
    }

    public void SetScenarioData((BackGroundInfoDataScriptableObject, int) scenarioDataObject)
    {
        ScenarioDataObject = scenarioDataObject;
    }

    #endregion

    
    private void GetEndingScene()
    {
        if ((int)AchieveManager.Instance.PlayerRank >= (int)ResearcherRank.Director)
        {
            //굿엔딩. 인체실험에 합류
            ScenarioDataObject = (EndingInfoData[0], 20);
            ProjectSceneManager.Instance.CallAdditiveScene(13);
        }
        else
        {
            //배드엔딩. 인체실험에 합류하지 못함
            ScenarioDataObject = (EndingInfoData[1], 21);
            ProjectSceneManager.Instance.CallAdditiveScene(13);
        }
    }

    private void ResetGameResources()
    {
        RemainedDay = BasicRemainDay;
        GeneSupplyTokenCount = 0;
        ChimeraSupplyTokenCount = 0;
        
        AccountUseAbleGene.Clear();//이건 정상작동
        
        AllGenes.Clear();
        AllChimeras.Clear();
        DictionaryChimera.Clear();
        
        for (int i = 0; i < Enum.GetValues(typeof(GeneType)).Length-1; i++)
        {
            AllGenes.Add(new Gene((GeneType)i));
        }
        
        ChimeraManager.Instance.DataReset();
        for (int i = 0; i < basicChimeras.Count; i++)
        {
            DictionaryChimera.Add(basicChimeras[i].GeneType, basicChimeras[i]);
            if (!ChimeraManager.Instance.AddDevelopmentableChimera(basicChimeras[i])) break;
        }
        
        AllChimeras.AddRange(basicChimeras);
        AllChimeras.AddRange(rewardChimera);
        
        //여기는 왜 안 정상작동? -> 중간 for문에 return있었음...
        Gene.DataReset();
        AchieveManager.Instance.DataReset();
        
    }

    /*    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // PlayMode 종료
#else
                Application.Quit(); // 빌드된 환경에서 게임 종료
#endif
    }*/
    /*
 public void SetBackgroundInfoData(ScenarioJson jsonData)
 {
     testScenarios.Add(jsonData);
 }

 public void SetSoBackgroundInfoData()
 {
     for (int i = 0; i < testScenarios.Count; i++)
     {
         ScenarioJson scenario = testScenarios[i];
         if (scenario is TimeLineScenario)
         {
             BackGroundInfoDataScriptableObject timeLineScenario = ScriptableObject.CreateInstance<BackGroundInfoDataScriptableObject>();
             timeLineScenario.SetBooks(scenario.ScenarioBooks);
             TimeLineBackgroundInfoData.Add(timeLineScenario);
         }
         else if(scenario is ResearcherRankScenario)
         {
             BackGroundInfoDataScriptableObject researcherScenario = ScriptableObject.CreateInstance<BackGroundInfoDataScriptableObject>();
             researcherScenario.SetBooks(scenario.ScenarioBooks);
             ResearcherBackgroundInfoData.Add(researcherScenario);
         }
     }
 }*/


}
