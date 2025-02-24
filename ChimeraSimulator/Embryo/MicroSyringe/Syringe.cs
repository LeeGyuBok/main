using System;
using UnityEngine;
using DG.Tweening;

public class Syringe : MonoBehaviour
{
    public Embryo Embryo { get; private set; }
    [SerializeField] private Vector3 targetPosition;
    private Vector3 _startPosition;
    [SerializeField] private float injectTimer;
    private void Awake()
    {
        
    }

    private void Start()
    {
        _startPosition = transform.position;
        
        //테스트코드
        //MoveToEmbryo();
    }

    private void Update()
    {
        
    }

    public void SetEmbryo(Embryo embryo)
    {
        Embryo = embryo;
    }

    public void MoveToEmbryo()
    {
        transform.DOMove(targetPosition, 3f)
            .SetEase(Ease.OutCubic).SetAutoKill(true);
    }
    
    public void MoveAwayFromEmbryo()
    {
        transform.DOMove(_startPosition, 3f)
            .SetEase(Ease.InCubic).SetAutoKill(true);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
