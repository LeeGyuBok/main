using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//얘는 뽑기용
public class RandomGeneAndChimeraCreator : MortalManager<RandomGeneAndChimeraCreator>
{
    //구상된 유전자 개수
    public int GeneTypeCount { get; private set; } = Enum.GetValues(typeof(GeneType)).Length;

    protected override void Awake()
    {
        base.Awake();
        //Debug.Log("GeneTypeCount: "+GeneTypeCount);
    }

    public void GetGene()
    {
        if (!GameImmortalManager.Instance.UseGeneTokens())
        {
            UiSoundManager.Instance.CancelSound();
            return;
        }
        UiSoundManager.Instance.AcceptSound();
        GetChimeraAndGeneUiManager.Instance.SetGeneTokensCount();
        GameImmortalManager.Instance.AddAccountGene(GetRandomGene());
    }

    public void GetChimera()
    {
        if (!GameImmortalManager.Instance.UseChimeraTokens())
        {
            UiSoundManager.Instance.CancelSound();
            return;
        }
        UiSoundManager.Instance.AcceptSound();
        GetChimeraAndGeneUiManager.Instance.SetChimeraTokensCount();
        GameImmortalManager.Instance.AddAccountChimeraData(GetRandomChimeraData(), true);
    }
    
    //4가지의 가챠타입
    //1. 올랜덤, 2. 최대 능력치 고정, 3. 타입 선택, 4.타입 선택 + 최대 능력치 고정

    //타입:랜덤, 수치:랜덤. 단, 수치의 경우 현재 직급에 영향을 받음.
    private Gene GetRandomGene()
    {
        Gene gene = new Gene((int)AchieveManager.Instance.PlayerRank);
        return gene;
    }
    /*//타입: 랜덤, 수치: 최고
    public Gene GetRandomPureGene()
    {
        Gene gene = new Gene(null,true);
        JSONWriter.Instance.SaveGeneData(gene);
        return gene;
    }

    //타입: 선택, 수치: 랜덤
    public Gene GetRandomGradeGene(GeneType geneType)
    {
        Gene gene = new Gene(geneType);
        JSONWriter.Instance.SaveGeneData(gene);
        return gene;
    }

    //타입: 선택, 수치: 최고
    public Gene GetSelectTypeGene(GeneType geneType)
    {
        Gene gene = new Gene(geneType, true);
        JSONWriter.Instance.SaveGeneData(gene);
        return gene;
    }*/

    /// <summary>
    /// No Mutant
    /// </summary>
    /// <returns></returns>
    private ChimeraData GetRandomChimeraData()
    {
        
        int randomIndex = Random.Range(0, ImmortalScriptableObjectManager.Instance.MainDnaByGeneType.Count);
        GeneType randomGeneType = (GeneType)randomIndex;
        ChimeraData data = ScriptableObject.CreateInstance<ChimeraData>();
        Chimera chimera = GameImmortalManager.Instance.GetChimeraByGeneType(randomGeneType);
        
        data.SetChimeraData(chimera,
            ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[chimera.GeneType],
            ImmortalScriptableObjectManager.Instance.NormalStatusesByGeneType[chimera.GeneType]);



        Gene gene = GetRandomGene();
        while (data.MainDna.TryInsertGene(gene))
        {
            data.MainDna.InsertGene(gene);        
            gene = GetRandomGene();
        }
        Gene.DestroyGene(gene);
        
        data.SetCoefficientToStatus();
        return data;
    }
}