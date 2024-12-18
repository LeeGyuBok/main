using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "NPC_Interaction", fileName = "NPCData")]
public class NPCData_SO : ScriptableObject
{
    [SerializeField] private string npcName;
    public string Name
    {
        get { return npcName; }
    }

    [SerializeField] private string koreanName;
    public string KoreanName
    {
        get { return koreanName; }
    }

    [SerializeField] private string npcCode;
    public string Code
    {
        get { return npcCode; }
    }
    
    [SerializeField] private string koreanDetail;
    public string KoreanDetail
    {
        get { return koreanDetail; }
    }
    
    [SerializeField] private int age;
    public int Age
    {
        get { return age; }
    }
    
    [SerializeField] private string sex;
    public string Sex
    {
        get { return sex; }
    }

    /// <summary>
    /// 최소1줄, 최대10줄
    /// </summary>
    [TextArea(1, 10)]
    [SerializeField] private string[] dialogData;

    public string[] DialogData
    {
        get { return dialogData; }
    }

    [SerializeField]private bool haveFunction;

    public bool HaveFunction
    {
        get { return haveFunction; }
    }

}
