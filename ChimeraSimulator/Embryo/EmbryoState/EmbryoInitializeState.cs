using UnityEngine;
using DG.Tweening;

public class EmbryoInitializeState : IEmbryoState
{
    public Embryo Embryo { get; }
    public Transform EmbryoObjectTransform { get; }
    private Tween _horizontalLoopTween;
    private Tween _verticalLoopTween;
    
    public EmbryoInitializeState (Embryo embryo)
    {
        Embryo = embryo;
        EmbryoObjectTransform = embryo.gameObject.transform;
    }

    public void Enter()
    {
        /*_horizontalLoopTween = EmbryoObjectTransform.DOMoveX( EmbryoObjectTransform.position.x - 1f, 3f)
            .SetLoops(-1, LoopType.Yoyo) // Yoyo for alternating directions
            .SetEase(Ease.InOutSine)// Smooth easing
            /*.SetAutoKill(true)#1#;*/
        _verticalLoopTween = EmbryoObjectTransform.DOMoveY(EmbryoObjectTransform.position.y - 0.5f, 1.5f)
            .SetLoops(-1, LoopType.Yoyo) // Yoyo for alternating directions
            .SetEase(Ease.InOutSine);
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        //_horizontalLoopTween.Kill(true);
        _verticalLoopTween.Kill(true);
    }
}
