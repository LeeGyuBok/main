using UnityEngine;
using DG.Tweening;
public class EmbryoInsertedGeneState : IEmbryoState
{
    public Embryo Embryo { get; }
    public Transform EmbryoObjectTransform { get; }
    private Vector3 _pushedDirection; 
    private bool _inserting;

    public EmbryoInsertedGeneState(Embryo embryo)
    {
        Embryo = embryo;
        EmbryoObjectTransform = embryo.gameObject.transform;
    }
    public void Enter()
    {
        if (_pushedDirection != Vector3.zero)
        {
            EmbryoObjectTransform.DOMove(_pushedDirection * 0.9f, 1.5f)
                .OnComplete(() => Embryo.ChangesStatePublic(Embryo.EmbryoExtractSyringeState));
            return;
        }
        _pushedDirection = new Vector3(-0.5f, -0.5f, 0f);
        EmbryoObjectTransform.DOMove(_pushedDirection * 0.9f, 1.5f)
            .OnComplete(() => Embryo.ChangesStatePublic(Embryo.EmbryoExtractSyringeState));
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        _pushedDirection = Vector3.zero;
    }

    public void SetPushedDirection(Vector3 direction)
    {
        _pushedDirection = direction;
    }
}
