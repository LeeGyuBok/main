using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AdditionalWeaponStatus
{
    public int Durability;
    public int AttackPower;

    public AdditionalWeaponStatus(int durability, int attackPower)
    {
        Durability = durability;
        AttackPower = attackPower;
    }
}

public interface IWeaponUpgradeable: IUpgradeable
{
    //차수 당, 내구도, 공격력
    public Dictionary<int, AdditionalWeaponStatus> AdditionalStatusByUpgrade { get; }
    //차수
    public Stack<AdditionalWeaponStatus> UpgradeCount { get; }
    
    public int TotalAdditionalDurability { get; }
    public int TotalAdditionalAttackPower { get; }
}
