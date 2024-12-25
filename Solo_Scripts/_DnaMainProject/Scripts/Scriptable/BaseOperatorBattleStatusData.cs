using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "StatusAndRole", menuName = "Scriptable Object/Role")]
public class BaseOperatorBattleStatusData : ScriptableObject
{
    [SerializeField] private int healthPoint;
    public int HealthPoint => healthPoint;
    [SerializeField] private int attackPower;
    public int AttackPower => attackPower;
    [SerializeField] private int defensePower;
    public int DefensePower => defensePower;
    [SerializeField] private int speed;
    public int Speed => speed;
    [SerializeField] private Role role;
    public Role Role => role;
    
    [SerializeField] private GeneType gene;
    public GeneType Gene => gene;
}
