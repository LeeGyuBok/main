using UnityEngine;

public interface IChimeraState
{
    public Chimera Chimera { get; }
    public Animator Animator { get; }
    public string StateName { get; }
    
    public void EnterState();
    public void Execute();
    public void ExitState();
}
