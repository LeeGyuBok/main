using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthpower
{
    public int MaxHealthPower { get; }
    public int CurrentHealthPower { get; }
    
    public bool IsDead { get; }
    
    /// <summary>
    /// dmg is positive.
    /// </summary>
    /// <param name="dmg"></param>
    public void UnderAttack(int dmg);
}
