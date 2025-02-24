using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepDnaSubSkill : DnaSubSkill, IDefendSkill
{
    public SheepDnaSubSkill(GeneType geneType) : base(geneType)
    {
        GeneType = geneType;
    }

    public override string SkillName { get;} = "두꺼운 털";
    public override string SkillDescription { get;} = "모든 피해 감소";
    public override int NeedInstinctPoint { get; }  = 9;
    public sealed override GeneType GeneType { get; protected set; }
    public float DefendCoefficient { get; } = 0.9f;
    public bool IsOpponentOnState(IChimeraState currentOpponentState)
    {
        return true;
    }

    public IDnaSkill AdditionalEffectToMySelf(Chimera chimera)
    {
        return this as IDnaSkill;
    }
}
