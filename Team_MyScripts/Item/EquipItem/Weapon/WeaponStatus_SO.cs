using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ArmData", fileName = "ArmData_ItemName")]
public class WeaponStatus_SO : ScriptableObject
{
    [SerializeField] private int itemCode;
    public int ItemCode => itemCode;
    
    [SerializeField] private int durability;
    public int Durability => durability;
    
    [SerializeField] private int attackRange;
    public int AttackRange => attackRange;
    
    [SerializeField] private int guardEfficiency;
    public int GuardEfficiency => guardEfficiency;
    
    [SerializeField] private int attackPower;
    public int AttackPower => attackPower;
    
    [SerializeField] private int attackSpeed;
    public int AttackSpeed => attackSpeed;
}
