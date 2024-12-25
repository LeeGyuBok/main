using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSnakeCycleSkill : CycleSkill
{
    public List<FriendlyOperator> nearFriendlyOperators { get; private set; } = new();
    private WaitForSeconds coolTime = new (10f);
    
    public override IEnumerator ActivateCycleSkill()
    {
        if (nearFriendlyOperators.Count > 0)
        {
            for (int i = 0; i < nearFriendlyOperators.Count; i++)
            {
                nearFriendlyOperators[i].Health.Heal(80, nearFriendlyOperators[i].MaxHealthPoint);
            }
            yield return coolTime;
        }
    }

    public string SkillName { get; protected set; } = "Venomedicine";
    public string SkillDescription { get; protected set; } = "Venomedicine";
}
