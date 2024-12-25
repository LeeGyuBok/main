using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffaloCycleSkill : CycleSkill
{
    public List<EnemyOperator> enemyOperators { get; private set; } = new();
    private WaitForSeconds duration = new (6f);
    private WaitForSeconds coolTime = new (4f);

    public override IEnumerator ActivateCycleSkill()
    {
        if (enemyOperators.Count > 0)
        {
            /*for (int i = 0; i < enemyOperators.Count; i++)
            {
                enemyOperators[i].Speed.SetSpeedPoint(-60);
            }
            yield return duration;
            for (int i = 0; i < enemyOperators.Count; i++)
            {
                enemyOperators[i].Speed.SetSpeedPoint(0);
            }*/

            yield return coolTime;
        }
        
    }

    public string SkillName { get; protected set; } = "Threat of Guardian";
    public string SkillDescription { get; protected set; } = "Threat of Guardian";
}
