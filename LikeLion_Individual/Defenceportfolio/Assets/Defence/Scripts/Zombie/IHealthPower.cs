using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthPower
{
    public int MaxHp { get; set; }
    public int HpIncrement { get; set; }
    public void TakeDmg(int dmg);
}
