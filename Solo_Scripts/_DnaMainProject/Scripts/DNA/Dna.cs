using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dna
{
    public GeneType GeneType { get; private set; }
    public static int MaxGeneCapacity { get; } = 20;
    public List<Gene> GeneList { get; private set; }
    public CycleSkill CycleSkill{ get; private set; }
    public OverExpressionSkill OverExpressionSkill{ get; private set; }
    

    public int TotalHealthCoefficient { get; protected set; } = 0;
    public int TotalAttackCoefficient { get; protected set; } = 0;
    public int TotalDefenceCoefficient { get; protected set; } = 0;
    public int TotalSpeedCoefficient { get; protected set; } = 0;

    public Dna(GeneType gene)
    {
        GeneType = gene;
        GeneList = new List<Gene>();
        
        switch (gene)
        {
            case GeneType.Wolf:
                CycleSkill = new WolfCycleSkill();
                OverExpressionSkill = new WolfOverExSkill();
                //Debug.Log("Wolf");
                break;
            case GeneType.Bear:
                CycleSkill = new BearCycleSkill();
                OverExpressionSkill = new BearOverExSkill();
                //Debug.Log("Bear");
                break;
            case GeneType.PoisonSnake:
                CycleSkill = new PoisonSnakeCycleSkill();
                OverExpressionSkill = new PoisonSnakeOverExSkill();
                //Debug.Log("PoisonSnake");
                break;
            case GeneType.Buffalo:
                CycleSkill = new BuffaloCycleSkill();
                OverExpressionSkill = new BuffaloOverExSkill();
                //Debug.Log("Buffalo");
                break;
            case GeneType.Tiger:
                CycleSkill = new TigerCycleSkill();
                OverExpressionSkill = new TigerOverExSkill();
                //Debug.Log("Tiger");
                break;
            case GeneType.Sheep:
                CycleSkill = new SheepCycleSkill();
                OverExpressionSkill = new SheepOverExSkill();
                //Debug.Log("Sheep");
                break;
        }
    }

    public bool TryInsertGene(Gene gene)
    {
        //현재 수용량을 계산하여 할당할 지역변수 선언
        int currentGeneCapacity = 0;
        //유전자 리스트에 1개라도 있으면
        if (GeneList.Count > 0)
        {
            //각각의 유전자에 대해서
            foreach (Gene g in GeneList)
            {
                //안정화정도를 현재 수용량에 더한다.
                currentGeneCapacity += g.StabilizationDegree;
            }
            
            //현재 수용량이 최대 수용량보다적으면 == 전달받은 유전자를 넣을 가능성이 있다는 뜻
            if (currentGeneCapacity < MaxGeneCapacity)
            {
                //넣으려는 유전자의 안정화정도를 가지고온다.
                int geneSize = gene.StabilizationDegree;
                
                //새로운 수용량 지역변수를 선언하고 그 변수에 현재수용량과 넣으려는 유전자의 안정화정도를 더한다.
                int newCapacity = currentGeneCapacity + geneSize;
                //새로운 수용량이 최대 수용량보다 작거나, 같으면
                if (newCapacity <= MaxGeneCapacity)
                {
                    //넣고 리턴한다.
                    GeneList.Add(gene);
                    SetGeneCoefficient(gene);
                    return true;
                }
                //새로운 수용량이 최대 수용량보다 크면 리턴한다 == 돌려줘야되는데?
                return false;
            }
            //현재 수용량이 최대 수용량과 같거나 그보다 크면
            return false;
        }
        //유전자 리스트에 아무것도 없으면 그냥 넣는다.
        GeneList.Add(gene);
        SetGeneCoefficient(gene);
        return true;
    }

    public void RemoveGene(Gene gene)
    {
        GeneList.Remove(gene);
        SetGeneCoefficient(gene, false);
    }

    //여기서 반복적으로 다해버리네.
    private void SetGeneCoefficientAtOnce(bool add = true)
    {
        if (GeneList.Count > 0)
        {
            foreach (var gene in GeneList)
            {
                if (add)
                {
                    if (!gene.IsActivation)
                    {
                        int hpCoefficient = gene.RandomStatusCoefficient[0];
                        TotalHealthCoefficient += hpCoefficient;
                
                        int atkCoefficient = gene.RandomStatusCoefficient[1];
                        TotalAttackCoefficient += atkCoefficient;
                
                        int defCoefficient = gene.RandomStatusCoefficient[2];
                        TotalDefenceCoefficient += defCoefficient;
                
                        int spdCoefficient = gene.RandomStatusCoefficient[3];
                        TotalSpeedCoefficient += spdCoefficient;
                
                        gene.SetActivationTrue();
                        return;
                    }
                    Debug.Log("already Activation");
                    return;
                }
                else
                {
                    if (gene.IsActivation)
                    {
                        int hpCoefficient = gene.RandomStatusCoefficient[0];
                        TotalHealthCoefficient -= hpCoefficient;
                
                        int atkCoefficient = gene.RandomStatusCoefficient[1];
                        TotalAttackCoefficient -= atkCoefficient;
                
                        int defCoefficient = gene.RandomStatusCoefficient[2];
                        TotalDefenceCoefficient -= defCoefficient;
                
                        int spdCoefficient = gene.RandomStatusCoefficient[3];
                        TotalSpeedCoefficient -= spdCoefficient;

                        gene.SetActivationFalse();
                    }
                    Debug.Log("already Deactivation");
                }
            }
        }
    }

    private void SetGeneCoefficient(Gene gene, bool add = true)
    {
        if (add)
        {
            if (!gene.IsActivation)
            {
                int hpCoefficient = gene.RandomStatusCoefficient[0];
                TotalHealthCoefficient += hpCoefficient;
                
                int atkCoefficient = gene.RandomStatusCoefficient[1];
                TotalAttackCoefficient += atkCoefficient;
                
                int defCoefficient = gene.RandomStatusCoefficient[2];
                TotalDefenceCoefficient += defCoefficient;
                
                int spdCoefficient = gene.RandomStatusCoefficient[3];
                TotalSpeedCoefficient += spdCoefficient;
                
                gene.SetActivationTrue();
                return;
            }
            Debug.Log("already Activation");
            
        }
        else
        {
            if (gene.IsActivation)
            {
                int hpCoefficient = gene.RandomStatusCoefficient[0];
                TotalHealthCoefficient -= hpCoefficient;
                
                int atkCoefficient = gene.RandomStatusCoefficient[1];
                TotalAttackCoefficient -= atkCoefficient;
                
                int defCoefficient = gene.RandomStatusCoefficient[2];
                TotalDefenceCoefficient -= defCoefficient;
                
                int spdCoefficient = gene.RandomStatusCoefficient[3];
                TotalSpeedCoefficient -= spdCoefficient;

                gene.SetActivationFalse();
            }
            Debug.Log("already Deactivation");
        }
    }
    

    public void EmptyGeneList()
    {
        GeneList.Clear();
    }
}
