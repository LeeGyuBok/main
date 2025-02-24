using UnityEngine;

public class MainSkillState : IChimeraState
{
    public Chimera Chimera { get; }
    public Animator Animator { get; }
    public DnaMainSkill MainSkill { get; } 
    public string StateName { get; }

    public MainSkillState(Chimera chimera)
    {
        Chimera = chimera;
        MainSkill = chimera.MainSkill;
        StateName = MainSkill.SkillName;
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
    
}

