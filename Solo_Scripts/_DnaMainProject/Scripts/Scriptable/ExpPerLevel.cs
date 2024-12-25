using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OperatorExpPerLevel", menuName = "Scriptable Object/ExpLevelData")]
public class ExpPerLevel : ScriptableObject
{
    public static Dictionary<int, int> expPerLevel { get; }= new Dictionary<int, int>()
    {
        {1, 2000},
        {2, 4000},
        {3, 6000},
        {4, 8000},
        {5, 10000},
        {6, 12000},
        {7, 14000},
        {8, 16000},
        {9, 18000},
        {10,20000}
    };
}
