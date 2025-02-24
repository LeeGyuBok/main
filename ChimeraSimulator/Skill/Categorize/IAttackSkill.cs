using System;

public interface IAttackSkill : IDnaSkill
{
    public float DamageCoefficient { get; }
    public bool IsOpponentOnState(IChimeraState lastOpponentState);
    public IChimeraState AdditionalEffectToOpponentChimera(Chimera chimera);
    public bool IsDodgeAble();
}
