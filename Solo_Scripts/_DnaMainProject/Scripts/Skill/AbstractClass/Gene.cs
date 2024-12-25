using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;



public class Gene
{
    public GeneType GeneType { get; private set; }
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
    public string ExpressionCharacterName { get; private set; } = "";
    
    public bool IsActivation { get; private set; } = false;

    public void SetActivationTrue()
    {
        IsActivation = true;
    }

    public void SetActivationFalse()
    {
        IsActivation = false;
    }

    public void SetFilePath(string jsonFilePath)
    {
        this.JsonFilePath = jsonFilePath;
    }

    public void SetCharacterName(string characterName)
    {
        this.ExpressionCharacterName = characterName;
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

    public Gene(GeneType? geneType = null, bool setMaxCoefficient = false)
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
        
        RandomStatusCoefficient.Add(0);
        RandomStatusCoefficient.Add(0);

        //Gene등급평가
        var gradeSelector = 0;
        if (setMaxCoefficient)
        {
            RandomStatusCoefficient.Add(10);
            RandomStatusCoefficient.Add(10);
            gradeSelector = 20;
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                int randomCoefficient = Random.Range(4, 11);
                RandomStatusCoefficient.Add(randomCoefficient);
                gradeSelector += randomCoefficient;
            }
        }
        // 나머지 두 개의 랜덤 값 추가 (4~10 사이)
 
        
        GeneGrade = (GeneGrade)((int)(gradeSelector/4)-1);
        StabilizationDegree = Enum.GetValues(typeof(GeneGrade)).Length - (int)GeneGrade;

        // 리스트 섞기 (0의 위치를 무작위로)
        ShuffleList(RandomStatusCoefficient);
        GeneID = DataId;
        JSONWriter.Instance.SaveGeneData(this);
        GameImmortalManager.Instance.AddGene(this);
        Sprite = Resources.Load<Sprite>($"Image/{GeneType.ToString()}");
        DataId++;
    }

    /// <summary>
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
        JSONWriter.Instance.SaveGeneData(this);
        GameImmortalManager.Instance.AddGene(this);
        Sprite = Resources.Load<Sprite>($"Image/{GeneType.ToString()}");
        DataId++;
        
    }

    /// <summary>
    /// json's
    /// </summary>
    /// <param name="data"></param>
    public Gene(GeneData data)
    {
        this.GeneType = (GeneType)data.GeneType;
        this.GeneGrade = (GeneGrade)data.GeneGrade;
        this.RandomStatusCoefficient = data.StatusCoefficient;
        this.StabilizationDegree = data.StabilizationDegree;
        this.GeneID = data.GeneID;
        this.StabilizationDegree = data.StabilizationDegree;
        Sprite = Resources.Load<Sprite>($"Image/{GeneType.ToString()}");
        DataId = data.GeneID + 1;
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
            int randomCoefficient = Random.Range(4, 11);
            RandomStatusCoefficient.Add(randomCoefficient);
            gradeSelector += randomCoefficient;
        }
 
        
        GeneGrade = (GeneGrade)((int)(gradeSelector/4)-1);
        StabilizationDegree = Enum.GetValues(typeof(GeneGrade)).Length - (int)GeneGrade;

        // 리스트 섞기 (0의 위치를 무작위로)
        ShuffleList(RandomStatusCoefficient);
        //이 코드를 실행한 진은 값이 바뀌어요. 그럼 바귄 값을Json 에 저장해야되요.
        JSONWriter.Instance.SaveRenewalGeneData(this);
    }
}
