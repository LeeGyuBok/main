using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAchieveClear
{
    public bool IsCleared { get; }
    public bool IsRewarded { get; }
    public void SetCleared();
    public void GetReward();
}

public class OfficialTestAchieveInfo : IAchieveClear
{
    public int NeedClearStageIndex { get; }
    public bool IsCleared { get; private set; }
    public bool IsRewarded { get; private set; }
    public OfficialTestAchieveInfo(int stage)
    {
        NeedClearStageIndex = stage + 1;
        IsCleared = false;
        IsRewarded = false;
    }

    public void SetCleared()
    {
        if (IsCleared) return;
        IsCleared = true;
    }
    public void GetReward()
    {
        if (IsRewarded) return;
        GameImmortalManager.Instance.AddAccountGene(new Gene((int)AchieveManager.Instance.PlayerRank));
        IsRewarded = true;
    }
}

public class NonOfficialTestVictoryAchieveInfo : IAchieveClear
{
    public int NeedVictoryCount { get; }
    public bool IsRewarded { get; private set; }
    public bool IsCleared { get; private set; }
    public NonOfficialTestVictoryAchieveInfo(int keyValue)
    {
        NeedVictoryCount = (keyValue + 1) * 3;
        IsRewarded = false;
        IsCleared = false;
    }
    
    public void SetCleared()
    {
        if (IsCleared) return;
        IsCleared = true;
    }
    public void GetReward()
    {
        if (IsRewarded) return;
        GameImmortalManager.Instance.AddAccountGene(new Gene((int)AchieveManager.Instance.PlayerRank));
        IsRewarded = true;
    }
}

public class NonOfficialTestDefeatAchieveInfo : IAchieveClear
{
    public int NeedDefeatCount { get; }
    public bool IsRewarded { get; private set; }
    public bool IsCleared { get; private set; }
    public NonOfficialTestDefeatAchieveInfo(int keyValue)
    {
        NeedDefeatCount = (keyValue + 1) * 3;
        IsRewarded = false;
        IsCleared = false;
    }
    
    public void SetCleared()
    {
        if (IsCleared) return;
        IsCleared = true;
    }
    public void GetReward()
    {
        if (IsRewarded) return;
        GameImmortalManager.Instance.AddAccountGene(new Gene((int)AchieveManager.Instance.PlayerRank));
        IsRewarded = true;
    }
}

public class ReallocationAchieveInfo : IAchieveClear
{
    public int NeedReallocationCount { get; }
    public bool IsRewarded { get; private set; }
    public bool IsCleared { get; private set; }
    public ReallocationAchieveInfo(int keyValue)
    {
        NeedReallocationCount = (keyValue + 1) * 3 + 1;
        IsRewarded = false;
        IsCleared = false;
    }
    
    public void SetCleared()
    {
        if (IsCleared) return;
        IsCleared = true;
    }
    public void GetReward()
    {
        if (IsRewarded) return;
        GameImmortalManager.Instance.AddAccountGene(new Gene((int)AchieveManager.Instance.PlayerRank));
        IsRewarded = true;
    }
}

public class DevelopmentChimeraAchieveInfo : IAchieveClear
{
    public int NeedDevelopmentCount { get; }
    public bool IsRewarded { get; private set; }
    public bool IsCleared { get; private set; }
    public DevelopmentChimeraAchieveInfo(int keyValue)
    {
        NeedDevelopmentCount = keyValue * 5 + 1;
        IsRewarded = false;
        IsCleared = false;
    }   
    
    public void SetCleared()
    {
        if (IsCleared) return;
        IsCleared = true;
    }
    public void GetReward()
    {
        if (IsRewarded) return;
        GameImmortalManager.Instance.AddAccountGene(new Gene((int)AchieveManager.Instance.PlayerRank));
        IsRewarded = true;
    }
}

/*public class ResearcherRankInfo
{
    public ResearcherRankInfo(int rank)
    {
        
    }
}*/

public enum ResearcherRank
{
    Public,
    Junior,
    Senior,
    Principal,
    Director
}

public class AchieveManager : ImmortalObject<AchieveManager>, IDataReset
{
    private const int AchieveCount = 10;
    
    public ResearcherRank PlayerRank { get; private set; } = ResearcherRank.Junior;
    
    public int HighestOfficialTestClearStage { get; private set; } = 0;
    public int TotalNonOfficialTestVictoryCount { get; private set; } = 0;
    public int TotalNonOfficialTestDefeatCount { get; private set; } = 0;
    public int TotalReallocationCount { get; private set; } = 0;
    public int TotalDevelopmentChimeraCount { get; private set; } = 0;

    private int _currentOfficialTestAchieveInfoKey;
    private int _currentNonOfficialTestVictoryAchieveInfoKey;
    private int _currentNonOfficialTestDefeatAchieveInfoKey;
    private int _currentReallocationAchieveInfoKey;
    private int _currentDevelopmentChimeraAchieveInfoKey;

    public Dictionary<int, OfficialTestAchieveInfo> OfficialTestAchieveInfos { get; private set; }
    public Dictionary<int, NonOfficialTestVictoryAchieveInfo> NonOfficialTestVictoryAchieveInfos { get; private set; }
    public Dictionary<int, NonOfficialTestDefeatAchieveInfo> NonOfficialTestDefeatAchieveInfos { get; private set; }
    public Dictionary<int, ReallocationAchieveInfo> ReallocationAchieveInfos { get; private set; }
    public Dictionary<int, DevelopmentChimeraAchieveInfo> DevelopmentChimeraAchieveInfos { get; private set; }
    
    public OfficialTestAchieveInfo NextOfficialTestAchieve { get; private set; }
    public NonOfficialTestVictoryAchieveInfo NextNonOfficialTestVictory { get; private set; }
    public NonOfficialTestDefeatAchieveInfo NextNonOfficialTestDefeat { get; private set; }
    public ReallocationAchieveInfo NextReallocationAchieve { get; private set; }
    public DevelopmentChimeraAchieveInfo NextDevelopmentChimera { get; private set; }

    private float totalAchieveInfo = 0;
    
    
    protected override void Awake()
    {
        base.Awake();
        OfficialTestAchieveInfos = new Dictionary<int, OfficialTestAchieveInfo>();
        NonOfficialTestVictoryAchieveInfos = new Dictionary<int, NonOfficialTestVictoryAchieveInfo>();
        NonOfficialTestDefeatAchieveInfos = new Dictionary<int, NonOfficialTestDefeatAchieveInfo>();
        ReallocationAchieveInfos = new Dictionary<int, ReallocationAchieveInfo>();
        DevelopmentChimeraAchieveInfos = new Dictionary<int, DevelopmentChimeraAchieveInfo>();
        
        for (int i = 0; i < AchieveCount; i++)
        {
            NonOfficialTestVictoryAchieveInfos[i] = new NonOfficialTestVictoryAchieveInfo(i);
            NonOfficialTestDefeatAchieveInfos[i] = new NonOfficialTestDefeatAchieveInfo(i);
            ReallocationAchieveInfos[i] = new ReallocationAchieveInfo(i);
            DevelopmentChimeraAchieveInfos[i] = new DevelopmentChimeraAchieveInfo(i);
        }

        int officialTestResearcherCount = OfficialTestManager.TotalResearchers;
        
        for (int i = 0; i < officialTestResearcherCount; i++)
        {
            OfficialTestAchieveInfos[i] = new OfficialTestAchieveInfo(i);    
        }
        
        _currentOfficialTestAchieveInfoKey = 0;
        NextOfficialTestAchieve = OfficialTestAchieveInfos[_currentOfficialTestAchieveInfoKey];
        
        _currentNonOfficialTestVictoryAchieveInfoKey = 0;
        NextNonOfficialTestVictory = NonOfficialTestVictoryAchieveInfos[_currentNonOfficialTestVictoryAchieveInfoKey];
        
        _currentNonOfficialTestDefeatAchieveInfoKey = 0;
        NextNonOfficialTestDefeat = NonOfficialTestDefeatAchieveInfos[_currentNonOfficialTestDefeatAchieveInfoKey];
        
        _currentReallocationAchieveInfoKey = 0;
        NextReallocationAchieve = ReallocationAchieveInfos[_currentReallocationAchieveInfoKey];
        
        _currentDevelopmentChimeraAchieveInfoKey = 0;
        NextDevelopmentChimera = DevelopmentChimeraAchieveInfos[_currentDevelopmentChimeraAchieveInfoKey];

        SetResult();
    }

    public void SetOfficialTestAchieveInfo(int clearedStage)
    {
        //인자로 받는 int는 클리어한 스테이지. ex) 7단계 클리어시 clearedStage == 7;
        if (clearedStage > HighestOfficialTestClearStage)
        {
            HighestOfficialTestClearStage = clearedStage;    
        }
        else
        {
            return;
        }
        
        //인자로 받은 최종 클리어 스테이지가 다음 요구 클리어 스테이지보다 크거나 같다면
        if (HighestOfficialTestClearStage >= NextOfficialTestAchieve.NeedClearStageIndex)
        {
            //다음 업적을 클리어하고
            NextOfficialTestAchieve.SetCleared();
            //키값을 중가시킨 후
            int nextInfoKey = _currentOfficialTestAchieveInfoKey + 1; 
            if (OfficialTestAchieveInfos.TryGetValue(nextInfoKey, out OfficialTestAchieveInfo info))
            {
                _currentOfficialTestAchieveInfoKey = nextInfoKey;
                NextOfficialTestAchieve = info;
                SetResult();
            }
            else
            {  if (_currentOfficialTestAchieveInfoKey == 10)
                {
                    return;
                }
                _currentOfficialTestAchieveInfoKey = 10;
                SetResult();
                return;
            }
            //그 다음 업적을 설정하고
            //다시비교한다
            
            //만약 새로 설정한 단계의 요구 클리어 스테이지보다 인자로 받은 최종 클리어 스테이지가 여전히 크거나 같다면
            if (HighestOfficialTestClearStage > NextOfficialTestAchieve.NeedClearStageIndex)
            {
                //재실행한다.
                SetOfficialTestAchieveInfo(HighestOfficialTestClearStage);
                SetResult();
                return;
            }
        }
        /*foreach (var pair in OfficialTestAchieveInfos)
        {
            //key값과 clearedStage값 비교
            //클리어한 스테이지가 밸류값의 요구 클리어 단계 보다 크거나 같으면? 해당 딕셔너리값 밸류 의 IsCleared를 true 로 하고 다음으로 넘어가기
            if (HighestOfficialTestClearStage >= pair.Value.NeedClearStageIndex)
            {
                pair.Value.SetCleared();
            }
            else//작아지면 해당 값의 밸류를 다음 목표로 설정하기
            {
                NextOfficialTestAchieve = pair.Value;
                break;
            }
        }*/
    }
    
    public void SetNonOfficialTestVictoryAchieveInfo()
    {
        //승리시 승리 카운트 증가
        TotalNonOfficialTestVictoryCount++;
        if (TotalNonOfficialTestVictoryCount >= NextNonOfficialTestVictory.NeedVictoryCount)
        {
            NextNonOfficialTestVictory.SetCleared();
            int nextInfoKey = _currentNonOfficialTestVictoryAchieveInfoKey + 1; 
            if (NonOfficialTestVictoryAchieveInfos.TryGetValue(nextInfoKey, out NonOfficialTestVictoryAchieveInfo info))
            {
                _currentNonOfficialTestVictoryAchieveInfoKey = nextInfoKey;
                NextNonOfficialTestVictory = info;
                SetResult();
            }
            else
            {
                if (_currentNonOfficialTestVictoryAchieveInfoKey == 10)
                {
                    return;
                }
                _currentNonOfficialTestVictoryAchieveInfoKey = 10;
                SetResult();
                return;
            }
            
            if (TotalNonOfficialTestVictoryCount > NextNonOfficialTestVictory.NeedVictoryCount)
            {
                //TotalNonOfficialTestVictoryCount--;
                SetNonOfficialTestVictoryAchieveInfo();
                SetResult();
                return;
            }
        }
        
        /*
        foreach (var pair in NonOfficialTestVictoryAchieveInfos)
        {
            //승리한 회 수 가 밸류값의 요구 승리 회 수 보다 크거나 같으면?
            if (TotalNonOfficialTestVictoryCount >= pair.Value.NeedVictoryCount)
            {
                pair.Value.SetCleared();
            }
            else//작아지면 해당 값의 밸류를 다음 목표로 설정하기
            {
                NextNonOfficialTestVictory = pair.Value;
                break;
            }
        }*/
    }
    
    public void SetNonOfficialTestDefeatAchieveInfo()
    {
        //패배시 성과 기록 용 패배 카운트 증가
        TotalNonOfficialTestDefeatCount++;
        if (TotalNonOfficialTestDefeatCount >= NextNonOfficialTestDefeat.NeedDefeatCount)
        {
            NextNonOfficialTestDefeat.SetCleared();
            int nextInfoKey = _currentNonOfficialTestDefeatAchieveInfoKey + 1; 
            if (NonOfficialTestDefeatAchieveInfos.TryGetValue(nextInfoKey, out NonOfficialTestDefeatAchieveInfo info))
            {
                _currentNonOfficialTestDefeatAchieveInfoKey = nextInfoKey;
                NextNonOfficialTestDefeat = info;
                SetResult();
            }
            else
            {   if (_currentNonOfficialTestDefeatAchieveInfoKey == 10)
                {
                    return;
                }
                _currentNonOfficialTestDefeatAchieveInfoKey = 10;
                SetResult();
                return;
            }
            
            if (TotalNonOfficialTestDefeatCount > NextNonOfficialTestDefeat.NeedDefeatCount)
            {
                //TotalNonOfficialTestDefeatCount--;
                SetNonOfficialTestDefeatAchieveInfo();
                SetResult();
                return;
            }
        }
        
        /*foreach (var pair in NonOfficialTestDefeatAchieveInfos)
        {
            //패배한 회 수 가 밸류값의 요구 패배 회 수 보다 크거나 같으면?
            if (TotalNonOfficialTestDefeatCount >= pair.Value.NeedDefeatCount)
            {
                pair.Value.SetCleared();
            }
            else//작아지면 해당 값의 밸류를 다음 목표로 설정하기
            {
                NextNonOfficialTestDefeat = pair.Value;
                break;
            }
        }*/
    }
    
    public void SetReallocationAchieveInfo()
    {
        //총 횟수 ++
        TotalReallocationCount++;
        // 총 횟수가 업적이 요구하는 요구치와 같거나 보다 크면
        if (TotalReallocationCount >= NextReallocationAchieve.NeedReallocationCount)
        {
            //클리어 처리
            NextReallocationAchieve.SetCleared();
            //업적 다음 단계 확인
            int nextInfoKey = _currentReallocationAchieveInfoKey + 1; 
            if (ReallocationAchieveInfos.TryGetValue(nextInfoKey, out ReallocationAchieveInfo info))
            {
                _currentReallocationAchieveInfoKey = nextInfoKey;
                NextReallocationAchieve = info;
                SetResult();
            }
            else
            {
                if (_currentReallocationAchieveInfoKey == 10)
                {
                    return;
                }
                _currentReallocationAchieveInfoKey = 10;
                SetResult();
                return;
            }
            
            if (TotalReallocationCount > NextReallocationAchieve.NeedReallocationCount)
            {
                //TotalReallocationCount--;
                SetReallocationAchieveInfo();
                SetResult();
                return;
            }
        }
        /*foreach (var pair in ReallocationAchieveInfos)
        {
            //key값과 clearedStage값 비교
            //클리어한 스테이지가 키값보다 크거나 같으면? 해당 딕셔너리값 밸류 의 IsCleared를 true 로 하고 다음으로 넘어가기
            if (TotalReallocationCount >= pair.Value.NeedReallocationCount)
            {
                pair.Value.SetCleared();
            }
            else//작아지면 해당 값의 밸류를 다음 목표로 설정하기
            {
                NextReallocationAchieve = pair.Value;
                break;
            }
        }*/
    }
    
    public void SetDevelopmentChimeraAchieveInfo()
    {
        //총 회 수 ++
        TotalDevelopmentChimeraCount++;
        //총 발생 회 수가 현재 설정된 업적의 요구치보다 크거나 같으면
        if (TotalDevelopmentChimeraCount >= NextDevelopmentChimera.NeedDevelopmentCount)
        {
            NextDevelopmentChimera.SetCleared();
            //int nextInfoKey = _currentDevelopmentChimeraAchieveInfoKey++; //선할당 후 증가
            int nextInfoKey = _currentDevelopmentChimeraAchieveInfoKey + 1;
            if (DevelopmentChimeraAchieveInfos.TryGetValue(nextInfoKey, out DevelopmentChimeraAchieveInfo info))
            {
                _currentDevelopmentChimeraAchieveInfoKey = nextInfoKey;
                NextDevelopmentChimera = info;
                SetResult();
            }
            else
            {
                if (_currentDevelopmentChimeraAchieveInfoKey == 10)
                {
                    return;
                }
                _currentDevelopmentChimeraAchieveInfoKey = 10;
                SetResult();
                return;
            }
            
            if (TotalDevelopmentChimeraCount > NextDevelopmentChimera.NeedDevelopmentCount)
            {
                //TotalDevelopmentChimeraCount--;
                SetDevelopmentChimeraAchieveInfo();
                SetResult();
                return;
            }
        }
        /*foreach (var pair in DevelopmentChimeraAchieveInfos)
        {
            //key값과 clearedStage값 비교
            //클리어한 스테이지가 키값보다 크거나 같으면? 해당 딕셔너리값 밸류 의 IsCleared를 true 로 하고 다음으로 넘어가기
            if (TotalDevelopmentChimeraCount >= pair.Value.NeedDevelopmentCount)
            {
                pair.Value.SetCleared();
            }
            else//작아지면 해당 값의 밸류를 다음 목표로 설정하기
            {
                NextDevelopmentChimera = pair.Value;
                break;
            }
        }*/
    }

    private void SetResult()
    {
        totalAchieveInfo = _currentReallocationAchieveInfoKey + _currentDevelopmentChimeraAchieveInfoKey +
                           _currentOfficialTestAchieveInfoKey + _currentNonOfficialTestDefeatAchieveInfoKey + 
                           _currentNonOfficialTestVictoryAchieveInfoKey;
        
        int info = (int)(totalAchieveInfo / 10);
        switch (info)
        {
            case 0:
            case 1:
                PlayerRank = ResearcherRank.Junior;//19개까지 주니어
                break;
            case 2:
                PlayerRank = ResearcherRank.Senior;//29까지 시니어
                break;
            case 3:
                PlayerRank = ResearcherRank.Principal;//39까지 프린시펄
                break;
            case 4:
                PlayerRank = ResearcherRank.Director;//40개부터 디렉터
                break;
            default:
                PlayerRank = ResearcherRank.Director;
                break;
                
        }
    }

    public void DataReset()
    {
        PlayerRank = ResearcherRank.Junior;
        int officialTestResearcherCount = OfficialTestManager.TotalResearchers;
        
        
        for (int i = 0; i < AchieveCount; i++)
        {
            NonOfficialTestVictoryAchieveInfos[i] = null;
            NonOfficialTestDefeatAchieveInfos[i] = null;
            ReallocationAchieveInfos[i] = null;
            DevelopmentChimeraAchieveInfos[i] = null;
        }
        
        for (int i = 0; i < officialTestResearcherCount; i++)
        {
            OfficialTestAchieveInfos[i] = null;    
        }
        
        OfficialTestAchieveInfos.Clear();
        NonOfficialTestVictoryAchieveInfos.Clear();
        NonOfficialTestDefeatAchieveInfos.Clear();
        ReallocationAchieveInfos.Clear();
        DevelopmentChimeraAchieveInfos.Clear();
        
        for (int i = 0; i < AchieveCount; i++)
        {
            NonOfficialTestVictoryAchieveInfos[i] = new NonOfficialTestVictoryAchieveInfo(i);
            NonOfficialTestDefeatAchieveInfos[i] = new NonOfficialTestDefeatAchieveInfo(i);
            ReallocationAchieveInfos[i] = new ReallocationAchieveInfo(i);
            DevelopmentChimeraAchieveInfos[i] = new DevelopmentChimeraAchieveInfo(i);
        }

        
        
        for (int i = 0; i < officialTestResearcherCount; i++)
        {
            OfficialTestAchieveInfos[i] = new OfficialTestAchieveInfo(i);    
        }
        
        _currentOfficialTestAchieveInfoKey = 0;
        NextOfficialTestAchieve = OfficialTestAchieveInfos[_currentOfficialTestAchieveInfoKey];
        
        _currentNonOfficialTestVictoryAchieveInfoKey = 0;
        NextNonOfficialTestVictory = NonOfficialTestVictoryAchieveInfos[_currentNonOfficialTestVictoryAchieveInfoKey];
        
        _currentNonOfficialTestDefeatAchieveInfoKey = 0;
        NextNonOfficialTestDefeat = NonOfficialTestDefeatAchieveInfos[_currentNonOfficialTestDefeatAchieveInfoKey];
        
        _currentReallocationAchieveInfoKey = 0;
        NextReallocationAchieve = ReallocationAchieveInfos[_currentReallocationAchieveInfoKey];
        
        _currentDevelopmentChimeraAchieveInfoKey = 0;
        NextDevelopmentChimera = DevelopmentChimeraAchieveInfos[_currentDevelopmentChimeraAchieveInfoKey];

        HighestOfficialTestClearStage = _currentOfficialTestAchieveInfoKey;
        TotalNonOfficialTestVictoryCount = _currentNonOfficialTestVictoryAchieveInfoKey;
        TotalNonOfficialTestDefeatCount = _currentNonOfficialTestDefeatAchieveInfoKey;
        TotalReallocationCount = _currentReallocationAchieveInfoKey;
        TotalDevelopmentChimeraCount = _currentDevelopmentChimeraAchieveInfoKey;
        
        SetResult();
    }
}
