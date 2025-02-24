using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenomSnakeDnaSubSkill : DnaSubSkill, IDefendSkill
{
    public VenomSnakeDnaSubSkill(GeneType geneType) : base(geneType)
    {
        GeneType = geneType;
    }
    public override string SkillName { get;} = "기회 포착";
    public override string SkillDescription { get;} = "적 기본공격 시,\n민첩성 증가";
    public override int NeedInstinctPoint { get; } = 9;
    public sealed override GeneType GeneType { get; protected set; }

    public float DefendCoefficient { get; } = 1f;
    
    private float maxAmount { get; } = 120;
    private float amount = 0;

    public bool IsOpponentOnState(IChimeraState currentOpponentState)
    {
        return currentOpponentState is BasicAttackState;
    }

    public IDnaSkill AdditionalEffectToMySelf(Chimera chimera)
    {
        if (amount >= maxAmount)
        {
            return this as IDnaSkill;
        }
        /*float amount = chimera.AgilityPoint * 0.12f;*/
        amount = 60f;
        chimera.SetAgility(amount);
        //Debug.Log("Agility increase");
        return this as IDnaSkill;
    }
    
}
