using System.Collections.Generic;
using UnityEngine;

public class ResearcherManager : ImmortalObject<ResearcherManager>
{

    [SerializeField] private List<ResearcherDataScriptableObject> totalResearchers;
    
    public List<MutantResearcherDataScriptableObject> MutantResearchers { get; private set; }
    public List<ResearcherDataScriptableObject> NormalResearchers { get; private set; }
    
    private List<ChimeraData> _normalResearchersChimeraData = new List<ChimeraData>();
    private List<ChimeraData> _mutantResearchersChimeraData = new List<ChimeraData>();
    
    public Dictionary<ResearcherDataScriptableObject, ChimeraData> ChimeraDataByResearcher { get; private set; }
    
    public List<ResearcherDataScriptableObject> TotalJrResearchers { get; private set; }
    public List<ResearcherDataScriptableObject> TotalSrPrResearchers { get; private set; }
    public List<ResearcherDataScriptableObject> TotalDrResearchers { get; private set; }
    
    

    protected override void Awake()
    {
        base.Awake();
        MutantResearchers = new List<MutantResearcherDataScriptableObject>();
        NormalResearchers = new List<ResearcherDataScriptableObject>();
        
        TotalJrResearchers = new List<ResearcherDataScriptableObject>();
        TotalSrPrResearchers = new List<ResearcherDataScriptableObject>();
        TotalDrResearchers = new List<ResearcherDataScriptableObject>();
        
        _normalResearchersChimeraData = new List<ChimeraData>();
        _mutantResearchersChimeraData = new List<ChimeraData>();
        ChimeraDataByResearcher = new Dictionary<ResearcherDataScriptableObject, ChimeraData>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < totalResearchers.Count; i++)
        {
            if (totalResearchers[i] is MutantResearcherDataScriptableObject)
            {
                _mutantResearchersChimeraData.Add(totalResearchers[i]
                    .SetChimeraData(true));
            }
            else
            {
                _normalResearchersChimeraData.Add(totalResearchers[i]
                    .SetChimeraData());
            }
        }
        foreach (var researcher in totalResearchers)
        {
            switch (researcher.ResearcherRank)
            {
                case ResearcherRank.Junior:
                    TotalJrResearchers.Add(researcher);
                    break;
                case ResearcherRank.Senior:
                case ResearcherRank.Principal:
                    TotalSrPrResearchers.Add(researcher);
                    break;
                case ResearcherRank.Director:
                    TotalDrResearchers.Add(researcher);
                    break;
                default:
                    //Debug.Log("Researcher Rank Not Supported");
                    break;
            }
        }
        foreach (var researcher in totalResearchers)
        {
            if (researcher is MutantResearcherDataScriptableObject mutantResearcher)
            {
                MutantResearchers.Add(mutantResearcher);
            }
            else
            {
                NormalResearchers.Add(researcher);    
            }
        }
        
        for (int i = 0; i < MutantResearchers.Count; i++)
        {
            ChimeraDataByResearcher[MutantResearchers[i]] = _mutantResearchersChimeraData[i];
        }

        for (int i = 0; i < NormalResearchers.Count; i++)
        {
            ChimeraDataByResearcher[NormalResearchers[i]] = _normalResearchersChimeraData[i];
        }
        //debug
        foreach (var kvp in ChimeraDataByResearcher)
        {
            //Debug.Log($"{kvp.Key.ResearcherName}: {kvp.Value.Chimera.GeneType.ToString()}");
        }
    }
    
    public ResearcherDataScriptableObject GetRandomOpponentResearcher()
    {
        int index = Random.Range(0, totalResearchers.Count);
        return totalResearchers[index];
    }

    public void SetNewChimeraData(ResearcherDataScriptableObject researcher)
    {
        //Debug.Log($"{researcher.ResearcherName} ReDevelopment Chimera");
        if (researcher is MutantResearcherDataScriptableObject)
        {
            researcher.SetChimeraData(true);
        }
        else
        {
            researcher.SetChimeraData();    
        }
        
    }
}
