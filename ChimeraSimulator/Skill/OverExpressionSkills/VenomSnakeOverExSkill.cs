using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenomSnakeOverExSkill : DnaMainSkill, IAttackSkill
{
    public override string SkillName { get; } = "독니";
    public override string SkillDescription { get; } = "피해량 증가,\n적 약화";
    public override int NeedInstinctPoint { get; protected set; } = 14;
    public float DamageCoefficient { get; } = 1.2f;
    public bool IsOpponentOnState(IChimeraState lastOpponentState)
    {
        return true;
    }

    public IChimeraState AdditionalEffectToOpponentChimera(Chimera defender)
    {
        defender.SetPoisoned(0.92f);
        return defender.VulnerableState;
    }

    public bool IsDodgeAble()
    {
        return true;
    }
}
