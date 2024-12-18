using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerCycleSkill : CycleSkill
{
    public EnemyOperator enemy { get; private set; }
    private FriendlyOperator tigerCharacter;
    
    public override IEnumerator ActivateCycleSkill()
    {
        //상태머신으로부터 enemy를 할당받는 로직
        //
        
        //적에게 달려드는 로직
        GameObject target = new GameObject();
        tigerCharacter.gameObject.transform.position = target.transform.position;
        
        //적에게 기절을 넣는 로직
        enemy = target.GetComponent<EnemyOperator>();
        enemy.StartCoroutine(enemy.OnStun());
        
        //적에게 추가 데미지를 넣는 로직
        yield break;
    }

    public void SetCharacter(FriendlyOperator character)
    {
        if (character.GeneType.Equals(GeneType.Tiger))
        {
            return;
        }
        tigerCharacter = character;
    }


    public override string SkillName { get; protected set; } = "Pounce";
    public override string SkillDescription { get; protected set; } = "Pounce";
}
