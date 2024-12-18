using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrowdControlType
{
    Stun,
    Poisoning,
    Fear,
    Focusing
}

public class CrowdControl
{
    public CrowdControlType Type { get; private set; }

    public CrowdControl(CrowdControlType type)
    {
        Type = type;
    }

    public void OnCrowdControl(EnemyOperator enemy, GameObject friendly)
    {
        switch (Type)
        {
            case CrowdControlType.Stun:
                enemy.StartCoroutine(enemy.OnStun());
                return;
            case CrowdControlType.Poisoning:
                enemy.StartCoroutine(enemy.OnPoisoning());
                return;
            case CrowdControlType.Fear:
                enemy.StartCoroutine(enemy.OnFear(friendly));
                return;
            case CrowdControlType.Focusing:
                return;
            default:
                return;
        }
    }
}
