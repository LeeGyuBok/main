using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Sex
{
    male,
    female
}
[CreateAssetMenu(fileName = "OperatorTrustable", menuName = "Scriptable Object/CharacterName")]
public class TrustableData : ScriptableObject
{
    [SerializeField] private string characterName;
    public string CharacterName => characterName;
    
    
    [SerializeField] private Sex sex;
    public Sex Sex => sex;
    
    
    [SerializeField] private int age;
    public int Age => age;
    
    
    [SerializeField] private int baseTrust;
    public int BaseTrust => baseTrust;
}
