using UnityEngine;

public class DodgeState : IChimeraState
{
    public Chimera Chimera { get; }
    public Animator Animator { get; }
    public string StateName { get; } = "회피";

    public DodgeState(Chimera chimera)
    {
        Chimera = chimera;
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
