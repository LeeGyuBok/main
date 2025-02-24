using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DnaMainSkill: IDnaSkill
{
    public abstract string SkillName { get; }
    public abstract string SkillDescription { get; }

    public abstract int NeedInstinctPoint { get; protected set; }
}
