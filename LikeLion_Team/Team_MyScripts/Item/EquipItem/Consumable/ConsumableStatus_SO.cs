using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumable", fileName = "Consumable_ItemName")]
public class ConsumableStatus_SO : ScriptableObject
{
    [SerializeField] private float restoreDegree;
    public float RestoreDegree => restoreDegree;
}
