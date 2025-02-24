using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DnaSubSkill : IDnaSkill
{
    private Image _mainSkillImage;
    public abstract string SkillName { get; }
    public abstract string SkillDescription { get; }

    public abstract int NeedInstinctPoint { get; }
    public abstract GeneType GeneType { get; protected set; }

    protected DnaSubSkill(GeneType geneType)
    {
        
    }

    public virtual IEnumerator ActivateCycleSkill()
    {
        yield return null;
    }
}