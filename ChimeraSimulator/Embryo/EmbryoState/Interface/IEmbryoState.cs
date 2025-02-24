using UnityEngine;

public interface IEmbryoState
{
    public Embryo Embryo { get; }
    public Transform EmbryoObjectTransform { get; }
    
    void Enter();
    void Execute();
    void Exit();
}
