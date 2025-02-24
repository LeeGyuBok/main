
public class ElephantDnaSubSkill : DnaSubSkill, IAttackSkill
{
    public ElephantDnaSubSkill(GeneType geneType) : base(geneType)
    {
        GeneType = geneType;
    }

    public override string SkillName { get; } = "충각";
    public override string SkillDescription { get; } = "무력화 상태 적에게,\n피해량 크게 증가";
    public override int NeedInstinctPoint { get; } = 13;
    public sealed override GeneType GeneType { get; protected set; }
    public float DamageCoefficient { get; } = 1.4f;
    public bool IsOpponentOnState(IChimeraState lastOpponentState)
    {
        return lastOpponentState is GroggyState;
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
