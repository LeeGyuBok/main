using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FriendlyMoveState : IState
{
    private FriendlyOperator friendlyOperator;
    private MyNode[,] grid;
    private List<MyNode> path; // A* 알고리즘을 통해 얻은 경로
    private int currentWaypointIndex = 0; // 현재 목표 지점 인덱스
    private Coroutine moveCoroutine;
    
    //아군의 적 감지.
    public bool DetectEnemy { get; private set; }
    public bool CloseEnemy { get; private set; }
    
    private bool ChangeAttack;
    
    private Collider[] hitColliders = new Collider[10];
    private List<EnemyOperator> TargetEnemies = new List<EnemyOperator>();
    private LayerMask EnemyObstacleCombineLayerMask;
    private EnemyOperator target;
    private Coroutine detectCoroutine;
    
    
    

    //어떤 녀석을 움직일 것인지?
    public FriendlyMoveState(FriendlyOperator character)
    {
        friendlyOperator = character;
        friendlyOperator.SetCurrentSignPoint(SignPoint.StartPoint);
    }

    public void Enter()
    {
        if (target)
        {
            target = null;
        }
        TargetEnemies.Clear();
        
        Debug.Log("Enter moveState" + "targetEnemiesList is" + TargetEnemies.Count);
        // 이동 상태에 들어갈 때 실행할 작업
        // 맵의 정보를 받고, 경로를 계산한다.
        grid = MySecondAStar.Instance.Grid;
        // A* 알고리즘을 사용해 경로를 계산
        Vector3 currentPosition = friendlyOperator.transform.position;
        //아래 if문은 변경예정임(실제 인게임의 목표달성여부 상태에 따라서)
        //현재위치가 목표노드인가?
        /*if (MySecondAStar.Instance.GetCurrentNode(currentPosition).Equals(MySecondAStar.Instance.GetTargetNode()))//맞으면 현재 위치에서 복귀노드로
        {
            Debug.Log("mission state: complete");
            path = MySecondAStar.Instance.FindNewPathByNodes(MySecondAStar.Instance.GetCurrentNode(currentPosition), MySecondAStar.Instance.GetReturnNode());    
        }
        else//아니면 현재 위치에서 목표노드로
        {
            Debug.Log("mission state: yet");
            friendlyOperator.SetCurrentSignPoint(SignPoint.StartPoint);
            path = MySecondAStar.Instance.FindNewPathByNodes(MySecondAStar.Instance.GetCurrentNode(currentPosition), MySecondAStar.Instance.GetTargetNode());
        }*/

        //현재 상태는?
        switch (friendlyOperator.CurrentSignPoint)
        {
            case SignPoint.StartPoint:// 아직 시작지점. == 목표지점에 도달하지못함.
                Debug.Log("mission state: yet");
                friendlyOperator.SetLastSignPoint(SignPoint.StartPoint);
                Debug.Log(MySecondAStar.Instance.GetTargetNode() is null);
                path = MySecondAStar.Instance.FindNewPathByNodes(MySecondAStar.Instance.GetCurrentNode(currentPosition), MySecondAStar.Instance.GetTargetNode());
                //오후 6:58 2024-12-20 이거 왜 널임?
                Debug.Log(path.Count);
                break;
            case SignPoint.TargetPoint:
                Debug.Log("mission state: complete");
                friendlyOperator.SetLastSignPoint(SignPoint.TargetPoint);
                path = MySecondAStar.Instance.FindNewPathByNodes(MySecondAStar.Instance.GetCurrentNode(currentPosition), MySecondAStar.Instance.GetReturnNode());
                break;
        }
        
        
        EnemyObstacleCombineLayerMask = CombineLayers("Enemy", "Obstacles");//와 진짜 나는 바보멍청이 똥개야 Obstacle로 해놓고 왜 안되지 이러고있었네
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
        //구형 레이를 쏜다. 아군위치에서, 아군의 시야거리만큼의 구체를 생성 <- 이러면 스피어콜라이더 필요없는거아님? 그래서 삭제함.
        //이 캐릭터를 중심으로 sight를 반지름으로하는 구를 영역전개, 이 영역은 에너미레이어마스크인 애들만 확인
        int detected = Physics.OverlapSphereNonAlloc(friendlyOperator.CharacterRigidbody.position, friendlyOperator.Sight, hitColliders, friendlyOperator.EnemyLayerMask);
        
        
        //적을 1개라도 감지했으면
        if (detected > 0)
        {
            //Debug.Log("Detected: " + detected);
            //감지된 적의 콜라이더에 대해서
            for (int i = 0; i < detected; i++) 
            {
                Collider hitCollider = hitColliders[i];

                // 현재 객체의 전방 벡터와 충돌체 방향 계산
                Vector3 directionToTarget = (hitCollider.transform.position - friendlyOperator.CharacterRigidbody.position).normalized;
                float angle = Vector3.Angle(friendlyOperator.transform.forward, directionToTarget);

                // 각도 필터링
                if (angle <= friendlyOperator.SightAngle / 2f)
                {
                    EnemyOperator detectedEnemy = hitColliders[i].gameObject.GetComponent<EnemyOperator>();
                    if (!detectedEnemy)
                    {
                        break;
                    }
                    if (TargetEnemies.Count == 0)
                    {
                        TargetEnemies.Add(detectedEnemy);
                        continue;
                    }
                    //TargetEnemies의 마지막 요소를 찾는 다른 방법. 미쳤다리
                    if (!TargetEnemies[TargetEnemies.Count - 1].Equals(detectedEnemy))
                    {
                        TargetEnemies.Add(detectedEnemy);    
                    }
                }
            }
            //적이 근처에 있음을 알림
            CloseEnemy = true;
        }
        else
        {
            return;
        }

        //Debug.Log($"CloseEnemy: {CloseEnemy}");
        //주변에 적이 있다면 <- 레이캐스트를 지속적으로 쏴야함.
        if (CloseEnemy)
        {
            //이미 실행중인 코루틴을 종료함.
            if (detectCoroutine != null)
            {
                friendlyOperator.StopCoroutine(detectCoroutine);
                detectCoroutine = null;
            }
            //새로운 코루틴 할당.
            detectCoroutine = friendlyOperator.StartCoroutine(LockOnEnemy());
        }
        else//없으면 그냥 종료
        {
            return;
        }

        //Debug.Log($"DetectEnemy: {DetectEnemy}");
        //적을 감지했다.
        if (DetectEnemy)
        {
            Vector3 origin = friendlyOperator.CharacterRigidbody.position;
            Vector3 direction = target.transform.position - origin;
            float distance = Vector3.Magnitude(direction);
            
            if (distance < friendlyOperator.Sight) //일단 공격 스테이트로 가야하니까
            {
                //엄폐물이 없다.
                if (true)
                {
                    Debug.Log(target.gameObject.name + ": detected");
                    //공격
                    //ChangeAttack = true;
                    friendlyOperator.StopCoroutine(moveCoroutine);
                    moveCoroutine = null;
                    //Debug.Log("Coroutine is stopped, enter Attack");
                    friendlyOperator.AssaultState.SetFirstTarget(target);
                    friendlyOperator.ChangeState(friendlyOperator.AssaultState);

                }//엄폐물이 있다.
                else
                {
                    friendlyOperator.StopCoroutine(moveCoroutine);
                    moveCoroutine = null;
                    //Debug.Log("Coroutine is stopped, enter TakeCover");
                    friendlyOperator.ChangeState(friendlyOperator.TakeCoverState);
                }
            }
        } 
        //Debug.Log("i'm moving");
    }
    
    public void Exit()
    {
     
        if (DetectEnemy)
        {
            Debug.Log("Exit moveState");
            DetectEnemy = false;
            CloseEnemy = false;
        }
        // 이동 상태에서 나갈 때 실행할 작업
    }

    private IEnumerator Move()
    {
        Rigidbody characterRigidbody = friendlyOperator.CharacterRigidbody;
        //Debug.Log("moving");
        while (path != null && currentWaypointIndex < path.Count)
        {
            Vector3 targetPosition = path[currentWaypointIndex].WorldPosition;
            Vector3 characterRigidBodyPosition = characterRigidbody.position;
            characterRigidBodyPosition.y = 0;
            Vector3 direction = targetPosition - characterRigidBodyPosition;
            direction.y = 0f;
            direction.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            MoveWithRigidbody(characterRigidbody, targetPosition, targetRotation, 0.3f,2f);
            
            //friendlyOperator.transform.position = Vector3.MoveTowards(friendlyOperator.transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            //friendlyOperator.transform.rotation =            
            yield return new WaitForFixedUpdate();
            if (ChangeAttack)
            {
                yield break;
            }
            
            // 목표 지점에 도달하면 다음 지점으로 이동. 리지드바디로 바꾸고나서 이렇게 하니까 잘움직이네?
            if ((targetPosition - characterRigidBodyPosition).magnitude < 0.5f)
            {
                currentWaypointIndex++;
                //Debug.Log(currentWaypointIndex);

                // 모든 경로를 완료하면 상태 종료. -> 그리드상 목적지에 진입하게되는 순간에
                // 위와 같이, 추가적으로 이동하기에, 자신의 위치를 새로 할당받아야함.
                if (currentWaypointIndex >= path.Count)//패스가 4개인경우?
                {
                    //아니 이게 원래 맞는 값이었다고? <- 위에서 받아왔을 때의 값.
                    //Debug.Log($"{targetPosition}, {characterRigidBodyPosition}, {Math.Abs((targetPosition - characterRigidBodyPosition).magnitude)}");//이게 왜 0.4나 되냐? 아니 Y값?
                    
                    //자신의 위치를 다시 동기화시켜야한다! 으아아아아악 이거때문에 몇시간날린거야
                    while ((targetPosition - characterRigidBodyPosition).magnitude  > 0.1f)
                    {
                        //이동시키면서 업데이트해야되는 값들.
                        characterRigidBodyPosition = characterRigidbody.position;
                        characterRigidBodyPosition.y = 0;
                        direction = targetPosition - characterRigidBodyPosition;
                        direction.y = 0f;
                        direction.Normalize();
                        targetRotation = Quaternion.LookRotation(direction);
                        //Debug.Log("moving");//여기서묶였다.
                        MoveWithRigidbody(characterRigidbody, targetPosition, targetRotation, 0.3f,2f);
                        yield return new WaitForFixedUpdate();
                        //이거 충돌하면서 리지드바디날아간거아냐? <- 이게 일단 맞다. 이것을 방지하기 위해선 무브포지션을 해야한다. <- 의미가 없진 않으건데
                        //Debug.Log($"{targetPosition}, {characterRigidBodyPosition}, {Math.Abs((targetPosition - characterRigidBodyPosition).magnitude)}");//이게 왜 0.4나 되냐? 아니 Y값?
                        //근데 이거도아닌데?
                        //아 변수 재할당이 일어나지 않아서그렇다...
                    }
                    Debug.Log("Path end, go to FriendlyAreaHoldState");//나온다~
                }
            }
        }
        //목표지점까지 도달했을 때
        switch (friendlyOperator.LastSignPoint)
        {
            case SignPoint.StartPoint:
                friendlyOperator.SetCurrentSignPoint(SignPoint.TargetPoint);
                friendlyOperator.ChangeState(friendlyOperator.MoveState);//진짜 감격이야..
                break;
            case SignPoint.TargetPoint:
                friendlyOperator.SetCurrentSignPoint(SignPoint.ReturnPoint);
                friendlyOperator.SetLastSignPoint(SignPoint.TargetPoint);
                friendlyOperator.ChangeState(friendlyOperator.AreaHoldState);//진짜 감격이야..
                break;
        }
    }

    //need Coroutine.
    public void MoveWithRigidbody(Rigidbody characterRigidbody, Vector3 targetPosition, Quaternion targetRotation, float speedCoefficient, float rotationCoefficient)
    {
        //리지드바디를 활용한 캐릭터이동에서 이상해졌던 부분이 이 부분때문일지도?
        // 위치 이동
        Vector3 newPosition = Vector3.MoveTowards(
            characterRigidbody.position, // 현재 위치
            targetPosition,              // 목표 위치
            friendlyOperator.moveSpeed * speedCoefficient * Time.fixedDeltaTime // 이동 속도
        );
        
        characterRigidbody.MovePosition(newPosition);
        /*Vector3 forceDirection = (targetPosition - characterRigidbody.position).normalized;
        characterRigidbody.AddForce(
            forceDirection * (friendlyOperator.moveSpeed * speedCoefficient)/*,
            ForceMode.VelocityChange // 즉각적인 속도 변경을 원할 경우#1#
        );*/

        // 회전 처리
        Quaternion newRotation = Quaternion.Slerp(
            characterRigidbody.rotation, // 현재 회전
            targetRotation,              // 목표 회전
            friendlyOperator.moveSpeed * rotationCoefficient * Time.fixedDeltaTime // 회전 속도
        );
        characterRigidbody.MoveRotation(newRotation);
    }


    public List<EnemyOperator> GetEnemies()
    {
        return TargetEnemies;
    }
    
    private LayerMask CombineLayers(params string[] layerNames)
    {
        int combinedMask = 0;
        foreach (string layerName in layerNames)
        {
            combinedMask |= 1 << LayerMask.NameToLayer(layerName);
        }
        return combinedMask;
    }

    private IEnumerator LockOnEnemy()
    {
        //Debug.Log("LockOnEnemy Start");
        while (!DetectEnemy)
        {
            //Debug.Log($"LockOnEnemy: {TargetEnemies.Count}");
            for (int i = 0; i < TargetEnemies.Count; i++)
            {
                if (!TargetEnemies[0])//0번째가 널객체니?
                {
                    TargetEnemies.Remove(TargetEnemies[0]);//새로뽑아
                    continue;
                }
                // 방향 벡터 계산
                Vector3 direction = (TargetEnemies[0].CharacterRigidbody.position - friendlyOperator.CharacterRigidbody.position).normalized;
                
                // 레이캐스트 실행. 두번째 변수는 좌표가 아닌 방향 벡터여야한다.
                if (Physics.Raycast(friendlyOperator.CharacterRigidbody.position, direction, out RaycastHit hitInfo, friendlyOperator.Sight, EnemyObstacleCombineLayerMask))
                {
                    //Debug.DrawRay(friendlyOperator.CharacterRigidbody.position, direction * friendlyOperator.Sight, Color.red, 3f);//잘 쏜다.
                    /*Debug.Log($"hitInfo: {hitInfo.collider != null}");
                    Debug.Log($"layer: {1 << hitInfo.collider.gameObject.layer}");//이게 10이었다. 
                    Debug.Log($"layer: {friendlyOperator.EnemyLayerMask}");//에너미레이어마스크가 1024지. 지피티티님 저랑 한판해요ㅡㅡ//수정완료 */ 
                    //0번째가 널이 아니야? 그럼 0번째쪽으로 레이캐스트했을 때 장애물이 있니?
                    if (hitInfo.collider != null && 1 << hitInfo.collider.gameObject.layer == friendlyOperator.ObstacleLayerMask)
                    {
                        TargetEnemies.Remove(TargetEnemies[0]);//새로뽑아
                        continue;
                    }
                    //Debug.Log(hitInfo.collider.gameObject.name);
                    // 히트인포가 널일 수 있다. 레이어는 인트값이다. 충돌한 객체가 EnemyLayerMask인 경우
                    //0번째가 널도아니고 장애물도 없니?
                    if (hitInfo.collider != null &&
                        1 << hitInfo.collider.gameObject.layer == friendlyOperator.EnemyLayerMask)
                    {
                        //걔가 타겟이야
                        target = TargetEnemies[0];

                        DetectEnemy = true;
                        //Debug.Log("DetectEnemy");
                        yield break; // 코루틴 종료
                    }
                }
            }
            // 체크 주기 설정 (0.1초 대기)
            yield return new WaitForSeconds(0.1f);
        }
    }
}
