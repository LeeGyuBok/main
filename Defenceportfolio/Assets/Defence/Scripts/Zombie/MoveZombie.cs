using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveZombie : MonoBehaviour, INevigate
{
    //좀비들이 움직일 방향을 갖고있는 부모오브젝트
    public GameObject DirectionFlag_Parent { get; set; }
    //자식오브젝트를 담을 배열(방향표시하는 깃발들)
    public GameObject[] DirectionFlag_Children { get; set; }
    //좀비 바라보고있는 방향
    public Vector3 CurrentDirectionVector3 { get; set; }
    //바라볼 다음 방향
    public Vector3 NextDirectionVector3 { get; set; }
    //방향전환 했어요 변수
    public bool Turn { get; set; }
    
    //방향전환한 횟수
    protected int turnCount;
    
    //좀비 속도
    private float speed;
    
    //리지드바디
    private Rigidbody rigidbody;
    //좀비 능력치
    private ZombieStatus zombieStatus;


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
    }

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out zombieStatus))
        {
            speed = zombieStatus.Speed;
            //Debug.Log("try success");
        }
        else
        {
            speed = 1.0f;
        }
        
        //Debug.Log($"start: {turnCount}");
        TurnForward(turnCount);
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

    /*// Transition is called once per frame
    void Transition()
    {
        
    }*/

    private void FixedUpdate()
    {
        /*transform.Translate((-currentDirectionVector3 + transform.position).normalized * (1.1f * Time.fixedDeltaTime));*/
        if (!zombieStatus.Dead)
        {
            Vector3 movement = (CurrentDirectionVector3 - transform.position).normalized * (speed * Time.fixedDeltaTime);

            transform.LookAt(CurrentDirectionVector3);
            // Rigidbody의 위치를 설정합니다.
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
}
