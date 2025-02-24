using UnityEngine;

public class VulnerableState : IChimeraState
{
    public Chimera Chimera { get; }
    public Animator Animator { get; }
    public string StateName { get; } = "취약";

    public VulnerableState(Chimera chimera)
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
