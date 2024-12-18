using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearCycleSkill : CycleSkill
{
    public List<FriendlyOperator> nearFriendlyOperators { get; private set; } = new();
    private bool IsAlone = false;

    private FriendlyOperator bearCharacter;
    
    public override IEnumerator ActivateCycleSkill()
    {
        if (nearFriendlyOperators.Count < 1 && bearCharacter != null)
        {
            IsAlone = true;
        }

        while (IsAlone /*&& !MissionEnd*/)
        {
            //같이 출발한 아군이 없다면 '자신'의 모든 능력치 증가, 체력 자연회복
            bearCharacter.Health.Heal(10, bearCharacter.MaxHealthPoint);
            yield return new WaitForSeconds(1f);
        }
    }

    public void SetCharacter(FriendlyOperator character)
    {
        if (character.GeneType.Equals(GeneType.Bear))
        {
            return;
        }
        bearCharacter = character;
    }

    public override string SkillName { get; protected set; } = "Tyrant";   
    public override string SkillDescription { get; protected set; } = "Tyrant";
}
