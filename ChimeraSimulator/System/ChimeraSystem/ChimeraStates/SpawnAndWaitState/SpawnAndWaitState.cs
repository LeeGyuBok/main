using UnityEngine;

public class SpawnAndWaitState : IChimeraState
{
    public Chimera Chimera { get; }
    public Animator Animator { get; }
    public string StateName { get; } = "케이지 대기";
    public SpawnAndWaitState(Chimera chimera)
    {
        Chimera = chimera;
        //Animator = chimera.Animator;
    }
    
    public void EnterState()
    {
        
    }

    public void Execute()
    {
        
    }

    public void FixedExecute()
    {
        
    }

    public void ExitState()
    {
        
    }
}
