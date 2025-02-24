using UnityEngine;

public class ThirdSkillState : IChimeraState
{
    public Chimera Chimera { get; }
    public Animator Animator { get; }
    public DnaSubSkill SubSkill { get; }
    public string StateName { get; } = "";

    public ThirdSkillState(Chimera chimera, DnaSubSkill subSkill)
    {
        Chimera = chimera;
        SubSkill = subSkill;
        StateName = subSkill.SkillName;
        //Animator = chimera.Animator;
    }
    public void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }
    public IDnaSkill Skill { get; }
}
