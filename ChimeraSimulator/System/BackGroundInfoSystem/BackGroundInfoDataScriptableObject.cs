using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackGroundInfoDataScriptableObject", menuName = "Scriptable Objects/BackGroundInfoDataScriptableObject")]

[Serializable]
public class BackGroundInfoDataScriptableObject : ScriptableObject
{
    public List<Scenario> ScenarioBooks { get; private set; }
    public bool Created { get; private set; } = false;
    public void SetBooks(List<Scenario> scenarioBooks)
    {
        if (Created) return;
        ScenarioBooks = scenarioBooks;
        Created = true;
    }
}
