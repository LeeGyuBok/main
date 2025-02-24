using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    VenomSnake,
    Buffalo,
    Tiger,
    Sheep,
    Horse,
    Elephant,
    Rhino,
    Dragon = 999
}

public class Gene
{
    public GeneType GeneType { get; private set; }
    public DnaSubSkill DnaSubSkill { get; private set; }
    public Sprite Sprite { get; private set; }
    public GeneGrade GeneGrade { get; private set; }
    /// <summary>
    /// 0 == Health, 1 == Attack, 2 == Defense, 3 == Speed
    /// </summary>
    public List<int> RandomStatusCoefficient { get; private set; } = new(4);
    public int StabilizationDegree { get; private set; }
    public int GeneID { get; private set; }
    public static int DataId { get; private set; } = 0;
    public string JsonFilePath { get; private set; } = "";
    public static int ExpressionChimeraNumber { get; private set; } = -1;
    public bool IsActivation { get; private set; } = false;
    private static List<Gene> _geneList = new(500);

    public Gene(int playerResearcherRank = 0)
    {
        //DragonType은 나오지 않음
        int randomIndex = Random.Range(0, Enum.GetValues(typeof(GeneType)).Length-1);
        GeneType = (GeneType)randomIndex;

        SetSubSkill(GeneType);
        
        RandomStatusCoefficient.Add(0);
        RandomStatusCoefficient.Add(0);

        //Gene등급평가
        var gradeSelector = 0;

        switch (playerResearcherRank)
        {
            case 0:
                // 나머지 두 개의 랜덤 값 추가 (4~10 사이)
                for (int i = 0; i < 2; i++)
                {
                    int randomCoefficient = Random.Range(4, 11);
                    RandomStatusCoefficient.Add(randomCoefficient);
                    gradeSelector += randomCoefficient;
                }
                break;
            case 4:
                for (int i = 0; i < 2; i++)
                {
                    int randomCoefficient = Random.Range(4+playerResearcherRank, 11);
                    RandomStatusCoefficient.Add(randomCoefficient);
                    gradeSelector += randomCoefficient;
                }
                break;
            default:
                for (int i = 0; i < 2; i++)
                {
                    int randomCoefficient = Random.Range(4+playerResearcherRank, 11);
                    RandomStatusCoefficient.Add(randomCoefficient);
                    gradeSelector += randomCoefficient;
                }
                break;
        }

        
        GeneGrade = (GeneGrade)((int)(gradeSelector/4)-1);
        StabilizationDegree = Enum.GetValues(typeof(GeneGrade)).Length - (int)GeneGrade;

        // 리스트 섞기 (0의 위치를 무작위로)
        ShuffleList(RandomStatusCoefficient);
        GeneID = DataId;
        //JSONWriter.Instance.SaveGeneData(this);
        //GameImmortalManager.Instance.AddUserGene(this);
        Sprite = Resources.Load<Sprite>($"GeneImage/{GeneType.ToString()}");
        DataId++;
    }
    /*/// <summary>
    /// combine
    /// </summary>
    public Gene(int minimum, GeneType? geneType = null)
    {
        if (geneType == null)
        {
            //DragonType은 나오지 않음
            int randomIndex = Random.Range(0, Enum.GetValues(typeof(GeneType)).Length-1);
            GeneType = (GeneType)randomIndex;
        }
        else
        {
            GeneType = geneType.Value;
        }
        
        SetSubSkill(GeneType);
        
        RandomStatusCoefficient.Add(0);
        RandomStatusCoefficient.Add(0);

        //Gene등급평가
        var gradeSelector = 0;

        if (minimum < 4)
        {
            minimum = 4;
        }

        if (minimum > 11)
        {
            minimum = 10;
        }
        
        for (int i = 0; i < 2; i++)
        {
            int randomCoefficient = Random.Range(minimum, 11);
            RandomStatusCoefficient.Add(randomCoefficient);
            gradeSelector += randomCoefficient;
        }
        // 나머지 두 개의 랜덤 값 추가 (4~10 사이)
        
        GeneGrade = (GeneGrade)((int)(gradeSelector/4)-1);
        StabilizationDegree = Enum.GetValues(typeof(GeneGrade)).Length - (int)GeneGrade;

        // 리스트 섞기 (0의 위치를 무작위로)
        ShuffleList(RandomStatusCoefficient);
        GeneID = DataId;
        //JSONWriter.Instance.SaveGeneData(this);
        //GameImmortalManager.Instance.AddUserGene(this);
        Sprite = Resources.Load<Sprite>($"Image/{GeneType.ToString()}");
        DataId++;
        
    }*/
    /// <summary>
    /// json's
    /// </summary>
    /// <param name="jsonData"></param>
    public Gene(GeneJsonData jsonData)
    {
        GeneType = (GeneType)jsonData.GeneType;
        SetSubSkill(GeneType);
        GeneGrade = (GeneGrade)jsonData.GeneGrade;
        RandomStatusCoefficient = jsonData.StatusCoefficient;
        StabilizationDegree = jsonData.StabilizationDegree;
        GeneID = jsonData.GeneID;
        StabilizationDegree = jsonData.StabilizationDegree;
        Sprite = Resources.Load<Sprite>($"GeneImage/{GeneType.ToString()}");
        DataId = jsonData.GeneID + 1;
    }

    /// <summary>
    /// GeneData, not Useable, selected GeneType
    /// </summary>
    /// <param name="geneType">GeneType</param>
    public Gene(GeneType geneType)
    {
        //DragonType은 나오지 않음
        GeneType = geneType;

        SetSubSkill(GeneType);
        
        RandomStatusCoefficient.Add(0);
        RandomStatusCoefficient.Add(0);

        //Gene등급평가
        var gradeSelector = 0;
        
        // 나머지 두 개의 랜덤 값 추가 (4~10 사이)
        for (int i = 0; i < 2; i++)
        {
            int randomCoefficient = Random.Range(10, 11);
            RandomStatusCoefficient.Add(randomCoefficient);
            gradeSelector += randomCoefficient;
        }
        
        GeneGrade = (GeneGrade)((int)(gradeSelector/4)-1);
        StabilizationDegree = Enum.GetValues(typeof(GeneGrade)).Length - (int)GeneGrade;

        // 리스트 섞기 (0의 위치를 무작위로)
        ShuffleList(RandomStatusCoefficient);
        GeneID = DataId;
        //JSONWriter.Instance.SaveGeneData(this);
        //GameImmortalManager.Instance.AddUserGene(this);
        Sprite = Resources.Load<Sprite>($"GeneImage/{GeneType.ToString()}");
        IsActivation = true;
        DataId++;
    }
    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        
            //이게 위와 동일하대요.
            /*int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;*/
        }
    }
    private void SetSubSkill(GeneType geneType)
    {
        switch (geneType)
        {
            case GeneType.Wolf:
                DnaSubSkill = new WolfDnaSubSkill(geneType);
                break;
            case GeneType.Bear:
                DnaSubSkill = new BearDnaSubSkill(geneType);
                break;
            case GeneType.VenomSnake:
                DnaSubSkill = new VenomSnakeDnaSubSkill(geneType);
                break;
            case GeneType.Buffalo:
                DnaSubSkill = new BuffaloDnaSubSkill(geneType);
                break;
            case GeneType.Tiger:
                DnaSubSkill = new TigerDnaSubSkill(geneType);
                break;
            case GeneType.Sheep:
                DnaSubSkill = new SheepDnaSubSkill(geneType);
                break;
            case GeneType.Horse:
                DnaSubSkill = new HorseDnaSubSkill(geneType);
                break;
            case GeneType.Elephant:
                DnaSubSkill = new ElephantDnaSubSkill(geneType);
                break;
            case GeneType.Rhino:
                DnaSubSkill = new RhinoDnaSubSkill(geneType);
                break;
        }
    }
    
    public void SetNewStatusCoefficient(int minimum = default)//디폴트값은 값타입(0, 0.0, false 등)과 참조타입(null)에 따라 다름
    {
        if (minimum < 4)
        {
            minimum = 4;
        }
        else if (minimum > 10)
        {
            minimum = 10;
        }
        
        RandomStatusCoefficient.Clear();
        
        RandomStatusCoefficient.Add(0);
        RandomStatusCoefficient.Add(0);

        //Gene등급평가
        var gradeSelector = 0;

        for (int i = 0; i < 2; i++)
        {
            int randomCoefficient = Random.Range(minimum, 11);
            RandomStatusCoefficient.Add(randomCoefficient);
            gradeSelector += randomCoefficient;
        }
 
        
        GeneGrade = (GeneGrade)((int)(gradeSelector/4)-1);
        StabilizationDegree = Enum.GetValues(typeof(GeneGrade)).Length - (int)GeneGrade;

        // 리스트 섞기 (0의 위치를 무작위로)
        ShuffleList(RandomStatusCoefficient);
        //이 코드를 실행한 진은 값이 바뀌어요. 그럼 바귄 값을Json 에 저장해야되요.
        //JSONWriter.Instance.SaveRenewalGeneData(this);
    }
    public void SetActivationTrue()
    {
        IsActivation = true;
    }
    public void SetActivationFalse()
    {
        ResetChimeraNumber();
        IsActivation = false;
    }
    public void SetFilePath(string jsonFilePath)
    {
        this.JsonFilePath = jsonFilePath;
    }

    public void SetChimeraNumber(int chimeraNumber)
    {
        ExpressionChimeraNumber = chimeraNumber;
    }
    private void ResetChimeraNumber()
    {
        ExpressionChimeraNumber = -1;
    }
    /// <summary>
    /// only in the JsonWriter
    /// </summary>
    /// <param name="dataId"></param>
    public static void SetDataIDbyJson(int dataId)
    {
        if (DataId <= dataId)
        {
            DataId = dataId;
        }
        
    }

    /// <summary>
    /// researcher Gene Create
    /// </summary>
    /// <param name="rank"></param>
    public Gene(ResearcherRank rank)
    {
        //DragonType은 나오지 않음
        //DragonType은 나오지 않음
        int randomIndex = Random.Range(0, Enum.GetValues(typeof(GeneType)).Length-1);
        GeneType = (GeneType)randomIndex;

        SetSubSkill(GeneType);

        RandomStatusCoefficient.Add(0);
        RandomStatusCoefficient.Add(0);

        //Gene등급평가
        var gradeSelector = 0;


        int randomCoefficient;
            
        switch (rank)
        {
            case ResearcherRank.Public:
            case ResearcherRank.Junior:
                for (int i = 0; i < 2; i++)
                {
                    randomCoefficient = Random.Range(5, 9);
                    RandomStatusCoefficient.Add(randomCoefficient);
                    gradeSelector += randomCoefficient;
                }
                break;
            case ResearcherRank.Senior:
            case ResearcherRank.Principal:
                for (int i = 0; i < 2; i++)
                {
                    randomCoefficient = Random.Range(8, 11);
                    RandomStatusCoefficient.Add(randomCoefficient);
                    gradeSelector += randomCoefficient;
                }
                break;
            case ResearcherRank.Director:
                for (int i = 0; i < 2; i++)
                {
                    randomCoefficient = 10;
                    RandomStatusCoefficient.Add(randomCoefficient);
                    gradeSelector += randomCoefficient;
                }
                break;
        }
        
        GeneGrade = (GeneGrade)((int)(gradeSelector/4)-1);
        StabilizationDegree = Enum.GetValues(typeof(GeneGrade)).Length - (int)GeneGrade;
        
        

        // 리스트 섞기 (0의 위치를 무작위로)
        ShuffleList(RandomStatusCoefficient);
        GeneID = DataId;
        //JSONWriter.Instance.SaveGeneData(this);
        //GameImmortalManager.Instance.AddUserGene(this);
        Sprite = Resources.Load<Sprite>($"Image/{GeneType.ToString()}");
        DataId++;
    }

    public static void DestroyGene(Gene gene)
    {
        if (gene.IsActivation) return;
        _geneList.Add(gene);
        DataId--;
    }

    public static void DataReset()
    {
        _geneList.Clear();
        DataId = 0;
    }
}
