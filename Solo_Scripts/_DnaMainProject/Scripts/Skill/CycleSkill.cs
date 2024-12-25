using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleSkill
{
    public string SkillName { get; protected set; }
    public string SkillDescription { get; protected set; }
    protected CycleSkill()
    {
        
    }
    public virtual IEnumerator ActivateCycleSkill()
    {
        yield return null;
    }

    

}
