using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyAssaultState : IState
{
    private FriendlyOperator friendlyOperator;
    private EnemyOperator firstTarget;

    public FriendlyAssaultState(FriendlyOperator character)
    {
        friendlyOperator = character;
    }
    
    public void Enter()
    {
        Debug.Log("Enter assaultState");
        if (!firstTarget)
        {
            friendlyOperator.ChangeState(friendlyOperator.MoveState);
        }
    }

    public void Execute()
    {
        Rigidbody characterRigidbody = friendlyOperator.CharacterRigidbody;
        Vector3 targetPosition = firstTarget.transform.position;
        Vector3 characterRigidBodyPosition = characterRigidbody.position;
        Vector3 direction = targetPosition - characterRigidBodyPosition;
        direction.y = 0f;
        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        friendlyOperator.MoveState.MoveWithRigidbody(characterRigidbody, targetPosition, targetRotation,0.9f ,40f);
        /*friendlyOperator.CharacterRigidbody.Move(Vector3.MoveTowards(friendlyOperator.CharacterRigidbody.position, targetPosition, friendlyOperator.moveSpeed * 0.9f * Time.fixedDeltaTime),  Quaternion.Slerp(
            friendlyOperator.transform.rotation, // 현재 회전
            targetRotation, // 목표 회전
            friendlyOperator.moveSpeed * 40 * Time.fixedDeltaTime)); // 회전 속도*/
        //Debug.Log(Math.Abs((targetPosition - characterRigidBodyPosition).magnitude) < 0.2f);
        //Debug.Log(friendlyOperator.NowAttachToTarget);
        if ((targetPosition - characterRigidBodyPosition).magnitude < 0.8f && friendlyOperator.NowAttachToTarget)
        {
            friendlyOperator.AttackState.SetTarget(firstTarget);
            
            characterRigidbody.velocity = Vector3.zero;
            characterRigidbody.angularVelocity = Vector3.zero;
            characterRigidbody.isKinematic = true;
            
            friendlyOperator.ChangeState(friendlyOperator.AttackState);
        }
    }

    public void Exit()
    {
        
    }

    public void SetFirstTarget(EnemyOperator enemy)
    {
        firstTarget = enemy;
    }
}
