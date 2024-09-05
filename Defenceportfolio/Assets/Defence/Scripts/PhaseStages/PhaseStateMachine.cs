using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseStateMachine
{
    public IPhaseState currentPhase { get; private set; }
    
    //페이즈별 단계가 올 곳
    public PhaseState_1 state1;
    public PhaseState_2 state2;
    public PhaseState_Boss stateBoss;
    
    public int CurrentStateClearCount { get; private set; }

    public event Action<IPhaseState> phaseChanged;

    public PhaseStateMachine(PhaseManager phaseManager)
    {
        state1 = new PhaseState_1(phaseManager);
        state2 = new PhaseState_2(phaseManager);
        stateBoss = new PhaseState_Boss(phaseManager);
    }

    public void Initialize(IPhaseState phase1)
    {
        //현재상태에 시작 상태를 할당해요
        currentPhase = phase1;
        CurrentStateClearCount = phase1.ClearCount;
        phase1.Enter();
    }

    public void TransitionTo(IPhaseState nextPhase)
    {
        currentPhase.Exit();
        // 현재 상태를 다음 상태로 바꾸고
        currentPhase = nextPhase;
        CurrentStateClearCount = nextPhase.ClearCount;
        currentPhase.Enter();//nextPhase.Enter(); 해도 되지만 주석이 생략되어 가독이 쉬워졌으므로 이와 같이 작성.
        
    }

    public void Update()
    {
        if (currentPhase != null)
        {
            currentPhase.Transition();
        }
    }
}
