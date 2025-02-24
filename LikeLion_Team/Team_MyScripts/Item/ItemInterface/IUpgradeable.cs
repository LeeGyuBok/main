using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeable
{
    //재료 아이템
    public Dictionary<ItemData_SO, int> UpgradeMaterialsAndNeedCount { get; }
    public bool IsUpgradeable { get; }
    public int PossibleUpgradeCount { get; }
    public int CurrentUpgradeCount { get; }
    public void SetUpgradeState();
    
    
}
