using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : SceneSingleton<MyPlayerController>
{
    //Soldier
    public GameObject Characters;
    private Dictionary<int, GameObject> CharacterInstances = new();
    
    public GameObject GetNewCharacter(Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Characters;
        GameObject instance = Instantiate(prefab, position, rotation);
        CharacterInstances.Add(instance.GetInstanceID(), instance);
        if (instance.TryGetComponent(out Soldier soldier))
        {
            UiManager.Instance.UpgradeWeapon += soldier.Upgrade;
            Debug.Log("Invoke");
        }
        return instance;
    }
}
