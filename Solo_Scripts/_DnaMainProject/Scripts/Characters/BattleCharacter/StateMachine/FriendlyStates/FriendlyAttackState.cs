using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyAttackState : IState
{
    private FriendlyOperator friendlyOperator;
    private Coroutine attackCoroutine;
    private EnemyOperator targetEnemy;

    public FriendlyAttackState(FriendlyOperator character)
    {
        friendlyOperator = character;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis.
    // 젯브레인의 성능분석도구가 무거운 호출은 감지하지 못하게함 개신기하네?
    public void Enter()
    {
        friendlyOperator.CharacterRigidbody.isKinematic = false;
        Debug.Log("Enter attackState");
        //적에가 가까이 다가가는 것도 포함.
        if (!targetEnemy)
        {
            friendlyOperator.ChangeState(friendlyOperator.SearchState);
        }
        
        friendlyOperator.GetCombatCapsuleCollider().enabled = true;    
        
        //Debug.Log(detectInfo.hitInfo.collider.gameObject.name + "Attacked"); //<- 잘나오네요
        //Debug.Log(enemy.name + "coroutine");//코루틴까지들어감 
        //어택이 온콜리전엔터보다 먼저돈다.
        attackCoroutine = friendlyOperator.StartCoroutine(Attack());
    }

    public void Execute()
    {
        if (!targetEnemy)
        {
            friendlyOperator.ChangeState(friendlyOperator.SearchState);
        }
    }

    public void Exit()
    {
        targetEnemy = null;
        friendlyOperator.GetCombatCapsuleCollider().enabled = false;
    }

    private IEnumerator Attack()
    {
        //Debug.Log(enemies.Count);
        if (targetEnemy)
        {
            //Debug.Log(!enemies[i]);
            //yield return friendlyOperator.StartCoroutine(Assault(targetEnemy));//지짜 이거 뭐임 개쩖!
            //Debug.Log(friendlyOperator.GetAttachToTarget());
            if (friendlyOperator.GetAttachToTarget())
            {
                while (targetEnemy && targetEnemy.Health.CurrentPoint > 0)
                {
                    targetEnemy.Health.TakeDamage(friendlyOperator.Attack.CurrentPoint);
                    //Debug.Log(friendlyOperator.Attack.CurrentPoint);
                    //Debug.Log(targetEnemy.Health.CurrentPoint);
                    yield return new WaitForSeconds(2f);
                    if (targetEnemy && targetEnemy.Health.CurrentPoint <= 0)
                    {
                        targetEnemy = null;
                        friendlyOperator.NowAttachToTarget = false;
                        friendlyOperator.GetCombatCapsuleCollider().enabled = false;
                        yield break;
                    }
                }
            }
        }
    }

    private IEnumerator Assault(EnemyOperator enemyOperator)
    {
        //최초할당
        Rigidbody characterRigidbody = friendlyOperator.CharacterRigidbody;
        Vector3 characterRigidBodyPosition = characterRigidbody.position;
        Vector3 targetPosition = enemyOperator.transform.position;
        characterRigidBodyPosition.y = 0;
        targetPosition.y = 0;
        Vector3 direction = targetPosition - characterRigidBodyPosition;
        direction.y = 0f;
        direction.Normalize();
        
        //안 닿아있으면
        if ((targetPosition - characterRigidBodyPosition).magnitude  < 0.2f)
        {
            while ((targetPosition - characterRigidBodyPosition).magnitude  > 0.1f)
            {
                //거리에 따른 재할당
                characterRigidBodyPosition = characterRigidbody.position;
                targetPosition = enemyOperator.transform.position;
                characterRigidBodyPosition.y = 0;
                targetPosition.y = 0;
                direction = targetPosition - characterRigidBodyPosition;
                direction.y = 0f;
                direction.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(direction);
            
                friendlyOperator.CharacterRigidbody.Move(Vector3.MoveTowards(friendlyOperator.CharacterRigidbody.position, targetPosition, friendlyOperator.moveSpeed * 1.4f * Time.fixedDeltaTime),  Quaternion.Slerp(
                    friendlyOperator.transform.rotation, // 현재 회전
                    targetRotation, // 목표 회전
                    friendlyOperator.moveSpeed * 40 * Time.fixedDeltaTime)); // 회전 속도
                if (friendlyOperator.NowAttachToTarget)
                {
                    yield break;
                }
                yield return null;
            }
        }
        
    }

    public void SetTarget(EnemyOperator target)
    {
        targetEnemy = target;
    }
}
