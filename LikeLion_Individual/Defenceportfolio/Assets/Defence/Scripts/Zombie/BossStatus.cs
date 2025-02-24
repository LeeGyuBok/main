using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStatus : MonoBehaviour, IHealthPower, INevigate
{
    public bool Dead { get; private set; }
    public bool Move { get; private set; }
    //방향전환 했어요 변수
    public GameObject DirectionFlag_Parent { get; set; }
    public GameObject[] DirectionFlag_Children { get; set; }
    public Vector3 CurrentDirectionVector3 { get; set; }
    public Vector3 NextDirectionVector3 { get; set; }
    public bool Turn { get; set; }
    
    //리지드바디
    private Rigidbody rigidbody;
    
    //방향전환한 횟수
    private int turnCount;
    
    //속도
    private float speed = 3.2f;
    
    //애니메이터
    private Animator animator;
    
    //물리용 콜라이더
    private CapsuleCollider capsuleCollider;
    
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    public event Action<BossStatus> BossClear;

    private void Awake()
    {
        DirectionFlag_Parent = GameObject.Find("Flags");
        int size = FindObjectsOfType<Flag_ZombieRoute>().Length;
        DirectionFlag_Children = new GameObject[size];
        size -= size;
        foreach (Transform child in DirectionFlag_Parent.transform)
        {
            
            DirectionFlag_Children[size] = child.gameObject;
            //Debug.Log(directionFlag_Children[size]);
            size++;

        }
        //Debug.Log(size);
        Turn = false;
        turnCount = 0;
        
        //Debug.Log($"Awake: {turnCount}");

        if (!TryGetComponent(out rigidbody))
        {
            Debug.Log("error");
        }
        if (!TryGetComponent(out capsuleCollider))
        {
            Debug.Log("error");
        }
        if (!TryGetComponent(out animator))
        {
            Debug.Log("error");
        }

        Hp = MaxHp;
    }


    // Start is called before the first frame update
    void Start()
    {
        Move = false;
        TurnForward(turnCount);
        StartCoroutine(RoarDelay());
    }
    
    private void TurnForward(int turncount)
    {
        if (turncount < DirectionFlag_Children.Length)
        {
            //방향을 전환해요
            CurrentDirectionVector3 = DirectionFlag_Children[turncount].transform.position;
            /*Debug.Log(currentDirectionVector3.normalized);*/
            if (!(turncount + 1).Equals(DirectionFlag_Children.Length))
            {
                NextDirectionVector3 = DirectionFlag_Children[turncount + 1].transform.position;
                /*Debug.Log(nextDirectionVector3.normalized);*/
                transform.LookAt(CurrentDirectionVector3); 
                
            }
            else
            {
                transform.LookAt(CurrentDirectionVector3); 
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void FixedUpdate()
    {
        /*transform.Translate((-currentDirectionVector3 + transform.position).normalized * (1.1f * Time.fixedDeltaTime));*/
        if (!Dead && Move)
        {
            Vector3 movement = (CurrentDirectionVector3 - transform.position).normalized * (speed * Time.fixedDeltaTime);

            // Rigidbody의 위치를 설정합니다.
            transform.LookAt(CurrentDirectionVector3); 
            rigidbody.MovePosition(transform.position + movement);
            //Debug.Log(currentDirectionVector3.normalized);    
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //방향지시자와 부딪치는 순간
        if (other.transform.position.Equals(DirectionFlag_Children[turnCount].transform.position))
        {
            Turn = true;
            turnCount++;
            if (Turn)
            {
                TurnForward(turnCount);
                Turn = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int MaxHp { get; set; } = 500;
    
    //보스몬스터에서는 체력재생으로 사용
    public int HpIncrement { get; set; } = 3;
    
    
    
    public int Hp { get; private set; }
    public void TakeDmg(int dmg)
    {
        Hp -= dmg;
        if (Hp <= 0 && !Dead)
        {
            Die();
            Debug.Log(Hp);
        }
    }

    private void Die()
    {
        Dead = true;
        Move = false;
        rigidbody.freezeRotation = true;
        animator.SetBool(Run, Move);
        animator.SetBool(IsDead, Dead);
        AnimatorStateInfo dead = animator.GetCurrentAnimatorStateInfo(0);
        Destroy(gameObject, dead.length + 1f);
    }

    private IEnumerator RoarDelay()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * 1.05f);
        Move = true;
        animator.SetBool(Run, Move);
    }
}
