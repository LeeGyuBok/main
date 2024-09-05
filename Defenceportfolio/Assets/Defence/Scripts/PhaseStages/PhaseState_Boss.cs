using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseState_Boss : IPhaseState
{
    private PhaseManager manager;

    private GameObject boss;
    
    public bool GoNextState { get; set; }

    public bool SpawnBoss { get; private set; }
    public int ClearCount { get; } = 1;

    public PhaseState_Boss(PhaseManager _manager)
    {
        manager = _manager;
    }

    public void Enter()
    {
        SpawnBoss = false;
        UiManager.Instance.Alarm();
        //상태 진입시 실행되는 코드, 원본코드와 동일
    }

    public void Transition()
    {
        //프레임당 로직. 새로운 상태로 전환하는 조건 포함, 원본코드에서는 Execute
        if (!SpawnBoss)
        {
            boss = ZombieSpawner.Instance.PublicSpawnBoss();
            SpawnBoss = true;
        }
        
        if (!boss.TryGetComponent(out BossStatus bossState))
        {
            Debug.Log("BossStatus script error");
        }

        if (bossState.Dead)
        {
            manager.PSM.TransitionTo(manager.PSM.state1);    
        }
           
    }

    public void Exit()
    {
        //상태 벗어날 때 실행되는 코드, 원본코드와 동일
    }
}
