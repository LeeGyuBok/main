using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseState_2 : IPhaseState
{
    private PhaseManager manager;
    public bool GoNextState { get; set; }
    public int ClearCount { get; } = 10;

    public PhaseState_2(PhaseManager _manager)
    {
        manager = _manager;
    }

    public void Enter()
    {
        /*Debug.Log("state 2 enter");//정상 작동 확인*/
        ZombieSpawner.Instance.spawnCount = 0;
        ZombieSpawner.Instance.targetSpawnCount = ClearCount;
        GoNextState = false;
    }

    public void Transition()
    {
        if (ZombieSpawner.Instance.spawnCount == ClearCount)
        {
            manager.PSM.TransitionTo(manager.PSM.stateBoss);        
        }
        else
        {
            ZombieSpawner.Instance.PublicSpawnZombie();
        }
    }

    public void Exit()
    {
        Debug.Log("state 2 exit");
    }
}
