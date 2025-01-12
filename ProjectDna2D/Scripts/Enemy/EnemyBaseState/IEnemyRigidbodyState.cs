using UnityEngine;

public interface IEnemyRigidbodyState
{
    public Enemy Enemy { get; }
    public Animator EnemyAnimator { get; }
    public void EnterState();
    public void FixedExecute();
    public void Execute();
    public void ExitState();
}
