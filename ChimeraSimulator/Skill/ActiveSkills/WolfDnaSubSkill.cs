using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfDnaSubSkill : DnaSubSkill, IAttackSkill
{
    public WolfDnaSubSkill(GeneType geneType) : base(geneType)
    {
        GeneType = geneType;
    }

    public override string SkillName { get;} = "집요함";
    public override string SkillDescription { get;} = "피해량 증가";
    public override int NeedInstinctPoint { get; } = 11;
    public sealed override GeneType GeneType { get; protected set; }
    
    public float DamageCoefficient { get; } = 1.2f;
    public bool IsOpponentOnState(IChimeraState lastOpponentState)
    {
        return true;
    }

    public IChimeraState AdditionalEffectToOpponentChimera(Chimera chimera)
    {
        return chimera.VulnerableState;
    }

    public bool IsDodgeAble()
    {
        return true;
    }
}
