using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOperator : OperatorBattleStatus
{
    #region EnemyCombatState

    //이 스테이트들은 아군적군으로 구분시켜야함.
    public IState MoveState { get; protected set; }
    public IState AreaHoldState{ get; protected set; }
    public IState AttackState{ get; protected set; }
    public IState TakeCoverState{ get; protected set; }
    public IState DeadState{ get; protected set; }
    
    //아군과 적군으로 구별해야함.
    public FriendlyOperator TargetEnemy { get; protected set; }
    
    protected int friendlyLayerMask { get; private set; }//적군용

    protected override void Awake()
    {
        base.Awake();
        MoveState = new MoveState(this);
        AreaHoldState = new AreaHoldState(this);
        DeadState = new DeadState(this);
        TakeCoverState = new TakeCoverState(this);
        AttackState = new AttackState(this);
    }

    protected override void Start()
    {
        // 처음 상태는 이동 상태로 설정
        Debug.Log("Enemy Start");
        friendlyLayerMask = 1 << LayerMask.NameToLayer("Friendly");//멤버변수로 못들고있음 ㅋㅋ
        
        CombatDetectingCapsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        if (CombatDetectingCapsuleCollider.isTrigger)
        {
            CombatDetectingCapsuleCollider.isTrigger = false;
        }
        if (CombatDetectingCapsuleCollider.enabled)
        {
            CombatDetectingCapsuleCollider.enabled = false;
        }
        
        CharacterRigidbody = gameObject.GetComponent<Rigidbody>();
        if (CharacterRigidbody.constraints != RigidbodyConstraints.FreezeRotation)
        {
            CharacterRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        
        ChangeState(MoveState); 
    }

    protected override void Update()
    {
        base.Update();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Friendly")))//아 어렵네
        {
            Debug.Log(other.gameObject.name);
            TargetEnemy = other.gameObject.GetComponent<FriendlyOperator>();
            NowAttachToTarget = true;
            Debug.Log(other.gameObject.name);
        }
    }
    
    public FriendlyOperator GetEnemy()
    {
        StartCoroutine(AttachToEnemy(TargetEnemy));
        return TargetEnemy;
    }
    
    private IEnumerator AttachToEnemy(OperatorBattleStatus enemy)
    {
        Debug.Log(enemy is null);//이게 널임 ㅋㅋ
        Vector3 targetPosition = enemy.gameObject.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 4f * Time.fixedDeltaTime);
        Vector3 direction = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, // 현재 회전
            targetRotation, // 목표 회전
            4f * 2 * Time.fixedDeltaTime); // 회전 속도
        if (NowAttachToTarget)
        {
            yield break;
        }
        yield return null;
        
    }

    #endregion
    public IEnumerator OnStun()
    {
        //일정시간 움직일 수 없어요
        yield return null;
    }

    public IEnumerator OnPoisoning()
    {
        //체력이 주기적으로 감소해요
        yield return null;
    }

    public IEnumerator OnFear(GameObject fearfulObject)
    {
        //일정시간동안 대상의 위치에서 멀어지는 방향으로 느리게 이동해요
        yield return null;
    }
}
