using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MoveState : IState
{
    private EnemyOperator friendlyOperator;
    private MyNode[,] grid;
    private List<MyNode> path; // A* 알고리즘을 통해 얻은 경로
    private int currentWaypointIndex = 0; // 현재 목표 지점 인덱스

    private float moveSpeed = 1f;
    private bool ChangeAttack;
    
    private Coroutine moveCoroutine;

    //어떤 녀석을 움직일 것인지?
    public MoveState(EnemyOperator character)
    {
        friendlyOperator = character as EnemyOperator;
    }

    public void Enter()
    {
        return;//테스트중..
        // 이동 상태에 들어갈 때 실행할 작업
        // 맵의 정보를 받고, 경로를 계산한다.

        grid = MySecondAStar.Instance.Grid;
        // A* 알고리즘을 사용해 경로를 계산
        path = MySecondAStar.Instance.GetPath();
        
        /*Debug.Log(path.Count);
        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log($"{path[i].x}, {path[i].z}");
        }*/
        
        currentWaypointIndex = 0; //<- 다른 상태에서 복귀할 때, 이 인덱스를 0으로 초기화해버리는데 이것도 수정해야함.
                                  //혹은 겟 패스에서 스타트포인트를 따로 지정해주거나 ㅋㅋ
        moveCoroutine = friendlyOperator.StartCoroutine(Move());

        //Debug.Log("이동 상태 시작");
    }

    public void Execute()
    {
        
       /*// 이동 상태에서 실행할 작업
       if (friendlyOperator.DetectEnemy)//<- 적, 아군의 상태를 분리함에 따라 수정예정
       {
           RaycastHit hitInfo = friendlyOperator.GetHitInfo();      
           float distance = Vector3.Magnitude(hitInfo.point - friendlyOperator.transform.position);
           //Debug.Log(distance);
           //적과의 거리비교. 특정 거리 이상이면 엄폐물 찾고 없으면 그냥 돌진!
           if (distance < 3f)//일단 공격 스테이트로 가야하니까
           {
               
               if (/*엄폐물이 없다 == #1#true)
               {
                   //공격
                   //ChangeAttack = true;
                   friendlyOperator.StopCoroutine(moveCoroutine);
                   moveCoroutine = null;
                   //Debug.Log("Coroutine is stopped, enter Attack");
                   friendlyOperator.ChangeState(friendlyOperator.AttackState);
                   
               }//엄폐물이 있다.
               {
                   friendlyOperator.StopCoroutine(moveCoroutine);
                   moveCoroutine = null;
                   //Debug.Log("Coroutine is stopped, enter TakeCover");
                   friendlyOperator.ChangeState(friendlyOperator.TakeCoverState);
               }
           }
            
           //Debug.Log(detectInfo.hitInfo.collider.gameObject.name);

           //Debug.Log("destroy");
       }*/
       
       // 여기를 반복한다잇
    }

    public void Exit()
    {
        // 이동 상태에서 나갈 때 실행할 작업
    }

    private IEnumerator Move()
    {
        while (path != null && currentWaypointIndex < path.Count)
        {
            Vector3 targetPosition = path[currentWaypointIndex].WorldPosition;
            friendlyOperator.transform.position = Vector3.MoveTowards(friendlyOperator.transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            Vector3 direction = targetPosition - friendlyOperator.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            friendlyOperator.transform.rotation = Quaternion.Slerp(
                friendlyOperator.transform.rotation, // 현재 회전
                targetRotation,                     // 목표 회전
                moveSpeed * 2 * Time.fixedDeltaTime // 회전 속도
            );           

            yield return null;
            if (ChangeAttack)
            {
                yield break;
            }
            // 목표 지점에 도달하면 다음 지점으로 이동
            if (Math.Abs((targetPosition - friendlyOperator.transform.position).magnitude) < 0.001f)
            {
                currentWaypointIndex++;

                // 모든 경로를 완료하면 상태 종료
                if (currentWaypointIndex >= path.Count)//패스가 4개인경우?
                {
                    friendlyOperator.ChangeState(friendlyOperator.AreaHoldState); // 공격 상태로 전환 (예시)
                    //Debug.Log("Path end, FriendlyAreaHoldState");
                }
            }
        }
    }
}
