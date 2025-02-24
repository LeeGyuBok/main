using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Feature
{
    육식,
    초식,
    무리,
    단일,
    사족보행,
    이족보행,
    사행
}

public class ImmortalScriptableObjectManager : ImmortalObject<ImmortalScriptableObjectManager>
{
    //처음부터 셋팅이 되어있어야함.
    public Dictionary<GeneType, FeatureList> FeaturesByGeneType { get; private set; }
    public Dictionary<GeneType, BaseStatus> NormalStatusesByGeneType { get; private set; }
    public Dictionary<GeneType, BaseStatus> MutantStatusesByGeneType { get; private set; }
    public Dictionary<GeneType, MainDna> MainDnaByGeneType { get; private set; }
    
    public Dictionary<GeneType, DnaSubSkill> DnaSubSkillsByGeneType { get; private set; }

    [SerializeField] private List<FeatureList> features;
    [SerializeField] private List<BaseStatus> normalStatuses;
    [SerializeField] private List<BaseStatus> mutantStatuses;

    protected override void Awake()
    {
        base.Awake();
        FeaturesByGeneType = new Dictionary<GeneType, FeatureList>();
        NormalStatusesByGeneType = new Dictionary<GeneType, BaseStatus>();
        MutantStatusesByGeneType = new Dictionary<GeneType, BaseStatus>();
        MainDnaByGeneType = new Dictionary<GeneType, MainDna>();
        DnaSubSkillsByGeneType = new Dictionary<GeneType, DnaSubSkill>();
        
        for (int i = 0; i < features.Count; i++)
        {
            FeatureList feature = features[i];
            FeaturesByGeneType.Add(feature.GeneType, feature);
            if (i < normalStatuses.Count)
            {
                MainDnaByGeneType[feature.GeneType] = new MainDna(feature.GeneType);    
            }
            
        }
        
        for (int i = 0; i < normalStatuses.Count; i++)
        {
            NormalStatusesByGeneType.Add(features[i].GeneType, normalStatuses[i]);
        }
        
        for (int i = 0; i < mutantStatuses.Count; i++)
        {
            MutantStatusesByGeneType.Add(features[i].GeneType, mutantStatuses[i]);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<Gene> allGene = new List<Gene>(GameImmortalManager.Instance.AllGenes);
        for (int i = 0; i < allGene.Count; i++)
        {
            DnaSubSkillsByGeneType[allGene[i].GeneType] = allGene[i].DnaSubSkill;
        }
    }
}
