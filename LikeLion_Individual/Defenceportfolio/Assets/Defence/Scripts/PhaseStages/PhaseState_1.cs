using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseState_1 : IPhaseState
{
    private PhaseManager manager;

    //초기화 이후 값이 변동되지 않는 경우, 기본값을 보존해야하는 경우, 불변객체를 생성하는 경우
    //인터페이스에서 정수타입변수인 클리어카운트를 선언, 이 클리어카운트는 get할 수 있으나 set할 수 없음
    //클리어카운트는 스테이트마다 달라질 예정
    public bool GoNextState { get; set; }
    public int ClearCount { get; } = 10;

    public PhaseState_1(PhaseManager _manager)
    {
        manager = _manager;
    }

    public void Enter()
    {
        //1페이즈 시작
        ZombieSpawner.Instance.spawnCount = 0;
        ZombieSpawner.Instance.targetSpawnCount = ClearCount;
        GoNextState = false;
        /*Debug.Log("state 1 enter");*/
    }

    public void Transition()
    {
        if (ZombieSpawner.Instance.spawnCount == ClearCount)
        {
            manager.PSM.TransitionTo(manager.PSM.state2);    
        }
        else
        {
            ZombieSpawner.Instance.PublicSpawnZombie();
        }
        
    }

    public void Exit()
    {
        Debug.Log("state 1 exit");
    }
}
