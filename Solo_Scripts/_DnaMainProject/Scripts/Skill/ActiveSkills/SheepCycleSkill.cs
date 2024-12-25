using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepCycleSkill : CycleSkill
{
    public List<FriendlyOperator> nearFriendlyOperators { get; private set; } = new();
    private WaitForSeconds duration = new (5f);
    private WaitForSeconds coolTime = new (5f);
    
    public override IEnumerator ActivateCycleSkill()
    {
        if (nearFriendlyOperators.Count > 0)
        {
            /*for (int i = 0; i < nearFriendlyOperators.Count; i++)
            {
                nearFriendlyOperators[i].SetDefensePointCoefficient(15);
            }
            yield return duration;
            for (int i = 0; i < nearFriendlyOperators.Count; i++)
            {
                nearFriendlyOperators[i].SetDefensePointCoefficient(0);
            }*/
            yield return coolTime;
        }
        
        
    }

    public string SkillName { get; protected set; } = "DefenseStance of Pack";
    public string SkillDescription { get; protected set; } = "DefenseStance of Pack";
}
