using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerOverExSkill : DnaMainSkill, IAttackSkill
{
    public override string SkillName { get; } = "암습";
    public override string SkillDescription { get; } = "조우 시,\n피해량 크게 증가,\n적 무력화";
    public override int NeedInstinctPoint { get; protected set; } = 14;
    public float DamageCoefficient { get; } = 1.4f;
    public bool IsOpponentOnState(IChimeraState lastOpponentState)
    {
        return lastOpponentState is StandingState;
    }

    public IChimeraState AdditionalEffectToOpponentChimera(Chimera defender)
    {
        return defender.GroggyState;
    }

    public bool IsDodgeAble()
    {
        return false;
    }
}
