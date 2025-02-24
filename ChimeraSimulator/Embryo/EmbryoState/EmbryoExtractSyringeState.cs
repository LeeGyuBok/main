using UnityEngine;
using DG.Tweening;

public class EmbryoExtractSyringeState : IEmbryoState
{
    public Embryo Embryo { get; }
    public Transform EmbryoObjectTransform { get; }

    public EmbryoExtractSyringeState(Embryo embryo)
    {
        Embryo = embryo;
        EmbryoObjectTransform = embryo.gameObject.transform;
    }
    public void Enter()
    {
        EmbryoObjectTransform.DOMove(Embryo.InitialPosition, 2f)
            .OnComplete(() => Embryo.ChangesStatePublic(Embryo.EmbryoInitializeState)).SetDelay(0.5f);
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        EmbryoObjectTransform.DOKill();
        CreateChimeraUiManager.Instance.SetGeneSelectPanel(true);
    }
}
