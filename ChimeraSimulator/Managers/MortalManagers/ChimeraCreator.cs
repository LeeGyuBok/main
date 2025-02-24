using System.Collections;
using UnityEngine;

public class ChimeraCreator : MortalManager<ChimeraCreator>
{
    [SerializeField] private Embryo embryoPrefab;
    
    public Syringe Syringe { get; private set; }
    
    private ChimeraData _targetChimeraData;
    public ChimeraData TargetChimeraData => _targetChimeraData;

    private readonly WaitForSeconds _delay = new WaitForSeconds(3.5f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Syringe = FindFirstObjectByType<Syringe>();
    }

    public void SetTargetChimeraData(Chimera chimeraPrefab)
    {
        _targetChimeraData = ScriptableObject.CreateInstance<ChimeraData>();
        if (chimeraPrefab.IsMutant)
        {
            _targetChimeraData.SetChimeraData(chimeraPrefab, ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[chimeraPrefab.GeneType], ImmortalScriptableObjectManager.Instance.MutantStatusesByGeneType[chimeraPrefab.GeneType]);    
        }
        else
        {
            _targetChimeraData.SetChimeraData(chimeraPrefab, ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[chimeraPrefab.GeneType], ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[chimeraPrefab.GeneType]);
        }
        Instantiate(embryoPrefab);
    }

    public void InsertGene(Gene gene)
    {
        if (_targetChimeraData == null)
        {
            return;
        }
        Syringe.MoveToEmbryo();
        _targetChimeraData.MainDna.InsertGene(gene);
        GameImmortalManager.Instance.AccountUseAbleGene.Remove(gene);
        StartCoroutine(MoveAwayFromEmbryo());

    }

    public ChimeraData DevelopmentChimera()
    {
        ChimeraData chimeraData = _targetChimeraData;
        //Debug.Log(chimeraData.SubSkills.Count);
        //chimeraData.SetChimeraData(chimera);
        _targetChimeraData = null;
        return chimeraData;
    }

    private IEnumerator MoveAwayFromEmbryo()
    {
        yield return _delay;
        Syringe.MoveAwayFromEmbryo();
    }

}
