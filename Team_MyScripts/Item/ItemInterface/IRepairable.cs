using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRepairable
{
    public int CurrentDurability { get; }
    public int MaxDurability { get; }
    public void RepairDurability(int restoreDegree);
    public void ConsumesDurability(int damage);
    public bool IsCrash { get; }

    public bool PossibleRepair { get; }
}
