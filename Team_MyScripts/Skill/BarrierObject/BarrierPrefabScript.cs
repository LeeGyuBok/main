using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPrefabScript : MonoBehaviour, IHealthpower
{
    public int MaxHealthPower { get; private set; }
    public int CurrentHealthPower { get; private set; }
    public bool IsDead { get; private set; }

    private GameObject player;

    private Vector3 interpolation;
     
    private void Awake()
    {
        //최대체력은 배리어스킬의 토탈스테이터스(플레이어최대체력 * 토탈스테이터스 / 100. 이때, 플레이어최대체력 == 100)
        //현재체력은 셋팅된 최대체력
        IsDead = false;
        player = FindFirstObjectByType<PlayerMoveController>().gameObject;
        interpolation = new Vector3(0, 0.95f, 0);
        CurrentHealthPower = (int)SkillFunction.Instance.liminex.Barrier.TotalStatus;

    }

    private void Start()
    {
        Invoke(nameof(DestroySelf), 3f);
    }

    private void Update()
    {
        transform.position = player.transform.position + interpolation;
        if (IsDead)
        {
            DestroySelf();
        }
    }

    public void UnderAttack(int dmg)
    {
        if (dmg < 0)
        {
            CurrentHealthPower += dmg;
            UiManager.Instance.SetPlayerShield(CurrentHealthPower);
            if (CurrentHealthPower <= 0) 
            {
                if (!IsDead)
                {
                    UiManager.Instance.Player.DestroyBarrier();
                    IsDead = true;
                }
            }
        }
        else
        {
            CurrentHealthPower -= dmg;
            UiManager.Instance.SetPlayerShield(CurrentHealthPower);
            if (CurrentHealthPower <= 0) 
            {
                if (!IsDead)
                {
                    UiManager.Instance.Player.DestroyBarrier();
                    IsDead = true;
                }
            }
        }
    }

    private void DestroySelf()
    {
        DestroyImmediate(gameObject);
    }
}
