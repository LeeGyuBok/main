using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainDna
{
    public GeneType GeneType { get; private set; }
    public DnaMainSkill DnaMainSkill{ get; private set; }
    public FeatureList DnaFeatureList { get; private set; }
    public static int MaxGeneCapacity { get; } = 20;
    public List<Gene> GeneList { get; private set; }
    
    public Sprite Sprite { get; private set; }
    
    public List<DnaSubSkill> DnaSubSkills { get; private set; }
    
    public static int MaxSubSkillsCount { get; } = 3;
    
    public int TotalHealthCoefficient { get; private set; } = 0;
    public int TotalAttackCoefficient { get; private set; } = 0;
    public int TotalDefenceCoefficient { get; private set; } = 0;
    public int TotalAgilityCoefficient { get; private set; } = 0;
    
    public Dictionary<Gene, int> FeatureCoefficientByGene { get; private set; }
    
    public string MainDnaDescription { get; private set; }
    
    public int TotalFeatureCoefficient { get; private set; }

    public MainDna(GeneType gene)
    {
        GeneType = gene;
        GeneList = new List<Gene>();
        FeatureCoefficientByGene = new Dictionary<Gene, int>();
        TotalFeatureCoefficient = 0;
        Sprite = Resources.Load<Sprite>($"MainSkillsImage/{GeneType.ToString()}");
        
        switch (gene)
        {
            case GeneType.Wolf:
                DnaMainSkill = new WolfOverExSkill();
                DnaFeatureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene];
                MainDnaDescription = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene].Description;
                break;
            case GeneType.Bear:
                DnaMainSkill = new BearOverExSkill();
                DnaFeatureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene];
                MainDnaDescription = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene].Description;
                break;
            case GeneType.VenomSnake:
                DnaMainSkill = new VenomSnakeOverExSkill();
                DnaFeatureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene];
                MainDnaDescription = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene].Description;
                break;
            case GeneType.Buffalo:
                DnaMainSkill = new BuffaloOverExSkill();
                DnaFeatureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene];
                MainDnaDescription = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene].Description;
                break;
            case GeneType.Tiger:
                DnaMainSkill = new TigerOverExSkill();
                DnaFeatureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene];
                MainDnaDescription = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene].Description;
                break;
            case GeneType.Sheep:
                DnaMainSkill = new SheepOverExSkill();
                DnaFeatureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene];
                MainDnaDescription = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene].Description;
                break;
        }

        DnaSubSkills = new List<DnaSubSkill>(MaxSubSkillsCount);
    }

    //유전자 삽입가능 여부 확인하기
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
                    return true;
                }
                //새로운 수용량이 최대 수용량보다 크면 리턴한다 == 돌려줘야되는데?
                return false;
            }
            //현재 수용량이 최대 수용량과 같거나 그보다 크면
            return false;
        }
        //유전자 리스트에 아무것도 없으면 그냥 넣는다.
        return true;
    }
    
    public void InsertGene(Gene gene)
    {
        GeneList.Add(gene);
        SetTotalCoefficient(gene);
        SetSubSkill(gene);
    }

    private void SetTotalCoefficient(Gene gene)
    {
        SetGeneCoefficient(gene);
        SetFeatureCoefficient(gene);
        TotalHealthCoefficient += FeatureCoefficientByGene[gene];
        TotalAttackCoefficient += FeatureCoefficientByGene[gene];
        TotalDefenceCoefficient += FeatureCoefficientByGene[gene];
        TotalAgilityCoefficient += FeatureCoefficientByGene[gene];
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
                
                int agilityCoefficient = gene.RandomStatusCoefficient[3];
                TotalAgilityCoefficient += agilityCoefficient;
                
                gene.SetActivationTrue();
            }
            //Debug.Log("already Activation");
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
                
                int agilityCoefficient = gene.RandomStatusCoefficient[3];
                TotalAgilityCoefficient -= agilityCoefficient;

                gene.SetActivationFalse();
            }
            //Debug.Log("already Deactivation");
        }
    }
    
    //20250128 - 유전자 개수가 늘어나면 피쳐코이피션트가 유전자 총개수 -1만큼 더 적용되는 현상. -> 가장 앞에 있는 것(들)이 적용됨
    //SetFeatureCoefficient에서 TotalCoefficient에 한번 적용하고, SetTotalCoefficient에서 TotalCoefficient를 한번더 적용하면서 버그발생
    //타입 유사도에 따른 추가 계수
    private void SetFeatureCoefficient(Gene gene, bool add = true)
    {
        //추가이면?
        if (add)
        {
            //참조할 피쳐리스트가져오기
            FeatureList featureList = ImmortalScriptableObjectManager.Instance.FeaturesByGeneType[gene.GeneType];
            //유전자를 키로 수용량 추가하기
            FeatureCoefficientByGene.Add(gene, 0);
            //수용된 유전자의 피처리스트의 각 피처에 대해서
            foreach (var feature in featureList.Features)
            {
                //MainDna의 피처리스트를 가져온다.
                for (int i = 0; i < DnaFeatureList.Features.Length; i++)
                {
                    //그 피처리스트의 각각의 피처와 비교한다.
                    if (feature.Equals(DnaFeatureList.Features[i]))
                    {
                        //일치하는게 있다면 값을 1 증가시킨다.
                        FeatureCoefficientByGene[gene]++;
                    }
                }
            }
        }
        else
        {
            TotalFeatureCoefficient -= FeatureCoefficientByGene[gene];
            FeatureCoefficientByGene.Remove(gene);
        }
        
        //수용된 유전자 하나에 대한 피처리스트 가져오기
    }

    //유전자를 삽입해야만 해당 과정을 진행한다. 인자로 받은 Gene은 실제 리스트에 있는 Gene이다
    /*
     * 1. 먼저 서브스킬을 추가할 수 있는지 확인한다(수량 확인).
     * 2. 서브스킬을 추가할 수 있다면 인자로 받은 Gene과 같은 GeneType의 DnaSubSkill을 활성화되어있는지 확인한다.
     * 3. 같은 GeneType의 DnaSubSkill을 활성화되어있다면 그대로 종료한다.
     */

    
    //서브스킬 활성화 여부 확인하기
    public bool TrySetSubSkill(Gene gene)
    {
        //공간이 남아있고
        if (DnaSubSkills.Count < MaxSubSkillsCount)
        {
            foreach (var subSkill in DnaSubSkills)
            {
                //같은 GeneType이 하나라도 있으면
                if (subSkill.GeneType.Equals(gene.GeneType))
                {
                    //Debug.Log("Failed to set sub skill: Same gene type");
                    return false;//종료
                }
            }//같은 GeneType이 하나도 없으면
            return true;
        }
        //Debug.Log("Failed to set sub skill: list is Full");
        return false;
    }
    private void SetSubSkill(Gene gene)
    {
        if (TrySetSubSkill(gene))
        {
            DnaSubSkills.Add(gene.DnaSubSkill);
            //Debug.Log(gene.GeneType);
        }
    }
    //여기서 반복적으로 다해버리네.
    /*private void SetGeneCoefficientAtOnce(bool add = true)
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
                        TotalAgilityCoefficient += spdCoefficient;
                
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
                        TotalAgilityCoefficient -= spdCoefficient;

                        gene.SetActivationFalse();
                    }
                    Debug.Log("already Deactivation");
                }
            }
        }
    }*/
    
    /*public void RemoveGene(Gene gene)
    {
        TotalHealthCoefficient -= TotalFeatureCoefficient;
        TotalAttackCoefficient -= TotalFeatureCoefficient;
        TotalDefenceCoefficient -= TotalFeatureCoefficient;
        TotalAgilityCoefficient -= TotalFeatureCoefficient;
        SetFeatureCoefficient(gene, false);
        SetGeneCoefficient(gene, false);
        GeneList.Remove(gene);
    }*/
}
