using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private EnemyOperator friendlyOperator;
    private FriendlyOperator enemy;

    private Coroutine attackCoroutine;

    public AttackState(EnemyOperator character)
    {
        friendlyOperator = character;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis.
    // 젯브레인의 성능분석도구가 무거운 호출은 감지하지 못하게함 개신기하네?
    public void Enter()
    {
        //적에가 가까이 다가가는 것도 포함.
        Debug.Log("FriendlyAttackState");
        enemy = friendlyOperator.GetEnemy();
        if (enemy)
        {
            friendlyOperator.GetCombatCapsuleCollider().enabled = true;    
        }
        //Debug.Log(detectInfo.hitInfo.collider.gameObject.name + "Attacked"); //<- 잘나오네요
        attackCoroutine = friendlyOperator.StartCoroutine(Attack());
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        enemy = null;
        friendlyOperator.GetCombatCapsuleCollider().enabled = false;
    }

    private IEnumerator Attack()
    {
        if (!enemy)
        {
            enemy = friendlyOperator.GetEnemy();
            friendlyOperator.GetCombatCapsuleCollider().enabled = true;
        }
        
        while (enemy.Health.CurrentPoint > 0)
        {
            enemy.Health.TakeDamage(friendlyOperator.Attack.CurrentPoint);
            yield return new WaitForSeconds(2f);
            Debug.Log("attack");
        }
        
        if (enemy.Health.CurrentPoint <= 0)
        {
            enemy = null;
            friendlyOperator.GetCombatCapsuleCollider().enabled = false;
        }
    }
}
