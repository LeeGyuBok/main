using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlySearchState : IState
{
    private FriendlyOperator friendlyOperator;
    private Collider[] hitColliders = new Collider[10];
    private List<EnemyOperator> closedEnemies = new List<EnemyOperator>();
    private EnemyOperator targetEnemy;

    public FriendlySearchState(FriendlyOperator character)
    {
        friendlyOperator = character;
    }
    
    public void Enter()
    {
        Debug.Log("enter search state");
        int detected = Physics.OverlapSphereNonAlloc(friendlyOperator.CharacterRigidbody.position, friendlyOperator.Sight, hitColliders, friendlyOperator.EnemyLayerMask);
        if (detected > 1)
        {
            //감지하고 주변 적 찾기
            for (int i = 0; i < detected; i++) 
            {
                EnemyOperator detectedEnemy = hitColliders[i].gameObject.GetComponent<EnemyOperator>();
                if (!detectedEnemy)
                {
                    break;
                }
                if (closedEnemies.Count == 0)
                {
                    closedEnemies.Add(detectedEnemy);
                    continue;
                }
                //TargetEnemies의 마지막 요소를 찾는 다른 방법. 미쳤다리 => ^1
                if (!closedEnemies[closedEnemies.Count - 1].Equals(detectedEnemy))
                {
                    closedEnemies.Add(detectedEnemy);    
                }
            }
            
            for (int i = 0; i < closedEnemies.Count; i++)
            {
                if (!targetEnemy)
                {
                    targetEnemy = closedEnemies[i];
                    /*if (i == closedEnemies.Count)
                    {
                        
                    }*/
                    continue;
                }
                float distanceToTargeted =
                    (targetEnemy.transform.position - friendlyOperator.CharacterRigidbody.position).magnitude;
                float distancePossibleTarget = (closedEnemies[i].transform.position - friendlyOperator.CharacterRigidbody.position).magnitude;
                if (distanceToTargeted > distancePossibleTarget)
                {
                    targetEnemy = closedEnemies[i];
                }
            }
            Debug.Log(targetEnemy.name);
        }
        else
        {
            Debug.Log("No enemies found");
            friendlyOperator.ChangeState(friendlyOperator.MoveState);
        }
    }

    public void Execute()
    {
        if (targetEnemy)
        {
            friendlyOperator.AssaultState.SetFirstTarget(targetEnemy);
            friendlyOperator.ChangeState(friendlyOperator.AssaultState);    
        }
        else
        {
            friendlyOperator.ChangeState(friendlyOperator.MoveState);
        }
    }

    public void Exit()
    {
        hitColliders = new Collider[10];
        closedEnemies.Clear();
        targetEnemy = null;
    }
}
