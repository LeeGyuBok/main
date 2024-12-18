using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GeneGrade
{
    None,
    Crude,
    Processed,
    Advanced,
    Pure
}

public enum GeneType
{
    Wolf,
    Bear,
    PoisonSnake,
    Buffalo,
    Tiger,
    Sheep,
    Dragon = 999
}

//얘는 뽑기용
public class GeneManager : MortalManager<GeneManager>
{
    //구상된 유전자 개수
    public int GeneTypeCount { get; private set; } = Enum.GetValues(typeof(GeneType)).Length;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("GeneTypeCount: "+GeneTypeCount);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //4가지의 가챠타입
    //1. 올랜덤, 2. 최대 능력치 고정, 3. 타입 선택, 4.타입 선택 + 최대 능력치 고정

    //타입:랜덤, 수치:랜덤
    public Gene GetRandomGene()
    {
        Gene gene = new Gene();
        //[4]가 들어가면 당연히 에러. <- 확인완료:  리스트에 더 많은 값들이 들어갔는지 확인용
        Debug.Log($"{gene.GeneType}, {gene.GeneGrade}, {gene.StabilizationDegree}, " +
                  $"{gene.RandomStatusCoefficient[0]},{gene.RandomStatusCoefficient[1]}, " +
                  $"{gene.RandomStatusCoefficient[2]},{gene.RandomStatusCoefficient[3]}");
        JSONWriter.Instance.SaveGeneData(gene);
        return gene;
    }
    //타입: 랜덤, 수치: 최고
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
    }
}
