using UnityEngine;

[CreateAssetMenu(fileName = "ResearcherDataScriptableObject", menuName = "Scriptable Objects/ResearcherDataScriptableObject")]
public class ResearcherDataScriptableObject : ScriptableObject
{
    [SerializeField] private GeneType chimeraGeneType;
    public GeneType ChimeraGeneType => chimeraGeneType;
    [SerializeField] private string researcherName;
    public string ResearcherName => researcherName;
    [SerializeField] private ResearcherRank researcherRank;
    public ResearcherRank ResearcherRank => researcherRank;

    public ChimeraData SetChimeraData(bool isMutantResearcher = false)
    {
        GeneType type = chimeraGeneType;
        ChimeraData data = CreateInstance<ChimeraData>();
        Chimera chimera = GameImmortalManager.Instance.GetChimeraByGeneType(type, isMutantResearcher);
        if (isMutantResearcher)
        {
            data.SetChimeraData(chimera,
                ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[chimera.GeneType],
                ImmortalScriptableObjectManager.Instance.MutantStatusesByGeneType[chimera.GeneType]);
        }
        else
        {
            data.SetChimeraData(chimera,
                ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[chimera.GeneType],
                ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[chimera.GeneType]);
        }
        
        Gene randomGene;
        switch (researcherRank)
        {
            
            case ResearcherRank.Public:
            case ResearcherRank.Junior://최소 5개
                randomGene = new Gene(ResearcherRank.Junior);
                while (data.MainDna.TryInsertGene(randomGene))
                {
                    data.MainDna.InsertGene(randomGene);        
                    randomGene = new Gene(ResearcherRank.Junior);
                }
                Gene.DestroyGene(randomGene);
                break;
            case ResearcherRank.Senior: //최소 6개
                randomGene = new Gene(ResearcherRank.Senior);
                while (data.MainDna.TryInsertGene(randomGene))
                {
                    data.MainDna.InsertGene(randomGene);        
                    randomGene = new Gene(ResearcherRank.Senior);
                }
                Gene.DestroyGene(randomGene);
                break;
            case ResearcherRank.Principal: //최소 10개
                randomGene = new Gene(ResearcherRank.Principal);
                while (data.MainDna.TryInsertGene(randomGene))
                {
                    data.MainDna.InsertGene(randomGene);        
                    randomGene = new Gene(ResearcherRank.Principal);
                }
                Gene.DestroyGene(randomGene);
                break;
            case ResearcherRank.Director: // 20개
                randomGene = new Gene(ResearcherRank.Director);
                while (data.MainDna.TryInsertGene(randomGene))
                {
                    data.MainDna.InsertGene(randomGene);        
                    randomGene = new Gene(ResearcherRank.Director);
                }
                Gene.DestroyGene(randomGene);
                break;
        }
        data.SetCoefficientToStatus();
        //Debug.Log($"{researcherRank}.{researcherName}: {data.MainDna.GeneList.Count}");
        return data;
    }
}
