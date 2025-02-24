using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepOverExSkill : DnaMainSkill, IDefendSkill
{
    public override string SkillName { get; } = "양털";
    public override string SkillDescription { get; } = "기본 공격 피해 크게 감소";
    public override int NeedInstinctPoint { get; protected set; } = 14;

    public float DefendCoefficient { get; } = 0.5f;

    public bool IsOpponentOnState(IChimeraState currentOpponentState)
    {
        return currentOpponentState is BasicAttackState;
    }
    public IDnaSkill AdditionalEffectToMySelf(Chimera chimera)
    {
        return this as IDnaSkill;
    }
}
