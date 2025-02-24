

public interface IDefendSkill : IDnaSkill
{
    public float DefendCoefficient { get; }
    public bool IsOpponentOnState(IChimeraState currentOpponentState);
    
    public IDnaSkill AdditionalEffectToMySelf(Chimera chimera);
    
}
