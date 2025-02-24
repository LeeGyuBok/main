using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfOverExSkill : DnaMainSkill, IAttackSkill
{
    public override string SkillName { get; } = "무리사냥";
    public override string SkillDescription { get; } = "적 취약 시,\n피해량 크게 증가";
    public override int NeedInstinctPoint { get; protected set; } = 14;
    public float DamageCoefficient { get; } = 1.4f;
    public bool IsOpponentOnState(IChimeraState lastOpponentState)
    {
        return lastOpponentState is VulnerableState;
    }

    public IChimeraState AdditionalEffectToOpponentChimera(Chimera defender)
    {
        return defender.VulnerableState;
    }

    public bool IsDodgeAble()
    {
        return true;
    }
}
