using UnityEngine;

public interface IRigidbodyState
{
    public Animator PlayerAnimator { get; }
    public void EnterState(Player player);
    public void FixedExecute(Player player);
    public void Execute(Player player);
    public void ExitState(Player player);
    
}
