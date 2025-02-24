using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//인터페이스라고! 제대로 정의해달라고!
public interface IPhaseState
{
    bool GoNextState { get; set; }
    //인터페이스에서 속성구현은 프로퍼티로 가능
    int ClearCount { get; }
    public void Enter()
    {
        //상태 진입시 실행되는 코드, 원본코드와 동일
    }

    public void Transition()
    {
        //프레임당 로직. 새로운 상태로 전환하는 조건 포함, 원본코드에서는 Execute
    }

    public void Exit()
    {
        //상태 벗어날 때 실행되는 코드, 원본코드와 동일
    }
}
