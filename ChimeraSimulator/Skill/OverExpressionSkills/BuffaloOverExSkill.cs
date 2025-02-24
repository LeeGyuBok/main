using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffaloOverExSkill : DnaMainSkill, IDefendSkill
{
    public override string SkillName { get; } = "두꺼운 가죽";
    public override string SkillDescription { get; } = "모든 피해 크게 감소";
    public override int NeedInstinctPoint { get; protected set; } = 14;
    public float DefendCoefficient { get; } = 0.6f;
    public bool IsOpponentOnState(IChimeraState currentOpponentState)
    {
        return true;
    }
    public IDnaSkill AdditionalEffectToMySelf(Chimera chimera)
    {
        return this as IDnaSkill;
    }
}
