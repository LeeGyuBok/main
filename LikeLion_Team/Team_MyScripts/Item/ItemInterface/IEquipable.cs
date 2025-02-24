using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipable
{
    public WeaponStatus_SO WeaponStatus { get; }
    public bool IsEquip { get; set; }
}
