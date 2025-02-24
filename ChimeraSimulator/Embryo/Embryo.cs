using System;
using DG.Tweening;
using UnityEngine;
//다트윈 찾아보기

[RequireComponent(typeof(SphereCollider))]
public class Embryo : MonoBehaviour
{
    private SphereCollider _embryoCollider;
    //상태1 유전자 삽입 전
    //상태2 유전자 삽입 중
    //상태3 유전자 삽입 후
    //상태4 발생
    private string _currentEmbryoState;
    public IEmbryoState CurrentEmbryoState { get; private set; }
    public EmbryoInitializeState EmbryoInitializeState { get; private set; }
    public EmbryoInsertedGeneState EmbryoInsertedGeneState { get; private set; }
    public EmbryoExtractSyringeState EmbryoExtractSyringeState { get; private set; }

    public Vector3 InitialPosition { get; private set; }
    
    private Vector3 _pushedDirection;
    
    

    private void Awake()
    {
        _embryoCollider = GetComponent<SphereCollider>();
        
        EmbryoInitializeState = new EmbryoInitializeState(this);
        EmbryoInsertedGeneState = new EmbryoInsertedGeneState(this);
        EmbryoExtractSyringeState = new EmbryoExtractSyringeState(this);
    }
    
    void Start()
    {
        InitialPosition = transform.position;
        ChangeState(EmbryoInitializeState);
    }
    
    void Update()
    {
        CurrentEmbryoState?.Execute();
    }

    public void ChangesStatePublic(IEmbryoState newEmbryoState)
    {
        ChangeState(newEmbryoState);
    }

    private void ChangeState(IEmbryoState newEmbryoState)
    {
        CurrentEmbryoState?.Exit();
        CurrentEmbryoState = newEmbryoState;
        CurrentEmbryoState.Enter();
        _currentEmbryoState = CurrentEmbryoState.ToString();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (CurrentEmbryoState == EmbryoInsertedGeneState) return;
        _pushedDirection = (transform.position - other.transform.position).normalized;
        EmbryoInsertedGeneState.SetPushedDirection(_pushedDirection);
        ChangeState(EmbryoInsertedGeneState);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    /*private void OnTriggerExit(Collider other)
    {
        ChangeState(EmbryoExtractSyringeState);
    }*/
}
