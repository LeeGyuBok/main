using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Bombird : Bird
{
    private BombirdPool m_bombirdPool;

    //이 새는 위험합니다. 폭발하거든요. 그리고 그 폭발의 범위를 정의하죠
    [SerializeField] private GameObject m_ExplosionField;

    //이 아이는 조금 특별합니다. 중력을 너무나 좋아해요. 그래서 더 빨리, 더 강하게, 땅과 가까워지고싶어합니다.
    private float bombirdGravityScale = 1.4f;
    
    //겟셋
    public BombirdPool m_BombirdPool
    {
        get => m_bombirdPool;
        set => m_bombirdPool = value;
    }
    
    protected override void Release()
    {
        Debug.Log("릴리즈는 읽어");
        m_bombirdPool.ReturnToBomPool(this);
        Debug.Log("풀로 돌아가요");
        m_bombirdPool.SpawnBombird();
        Debug.Log("새가 나타나요");
    }
    

    // Start is called before the first frame update
    void Start()
    {
        ProtectedStart();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //버드의 함수를 갖다써요
    private void Awake()
    {
        ProtectedAwake();
    }

    //버드의 함수를 갖다써요
    private void OnEnable()
    {
        ProtectedOnEnable();
    }

    //버드의 함수를 갖다써요
    private void OnDisable()
    {
        ProtectedOnDisable();
    }

    //버드의 함수를 갖다써요
    private void OnMouseOver()
    {
        ProtectedOnMouseOver();
    }

    //버드의 함수를 갖다써요
    private void OnMouseDown()
    {
        //Debug.Log("봄버드를");
        ProtectedOnMouseDown();
    }

    //버드의 함수를 갖다써요
    private void OnMouseDrag()
    {
        ProtectedOnMouseDrag();
    }

    //버드의 함수를 갖다쓰되, 그라비티 스케일은 이 위험한 새의 그라비티 스케일을 적용해요
    protected override void RenderTrajectory(float _gravityScale)
    {
        //프로텍티드로 선언된 상위클래스의 멤버변수나 함수를 불러올 수 있다. == base. 
        base.RenderTrajectory(bombirdGravityScale);
    }

    //버드의 함수를 갖다써요.
    private void OnMouseUp()
    {
        ProtectedOnMouseUp();
        //Debug.Log(GetComponent<Rigidbody2D>().gravityScale);
    }

    //이 위험한 새의 중력에 대한 특별한 사랑을 적용하기위해 오버라이드해요.
    protected override void ChangeGravity(float _gravity)
    {
     
        base.ChangeGravity(bombirdGravityScale);
    }
    
    //이 아이만의 특별한 충돌시 함수
    private void OnCollisionEnter2D(Collision2D other)
    {
        //발사한 상태면
        if (m_IsLaunch)
        {
            Debug.Log("2D Crash");
            //코루틴을 실행해요
            StartCoroutine(ObjectDisable());
            //발사상태를 꺼요
            m_IsLaunch = false;
            
        }
        if (!m_IsLaunch && !m_IsClicked && m_IsMouseOver)
        {
            ResetState();
            Release();
        }
    }
    
    protected override IEnumerator ObjectDisable()
    {
        //조금 기다려요
        yield return new WaitForSeconds(1.8f);
        
        Debug.Log("폭발한다 펑");
        //폭발하는 로직 구현
        Instantiate(m_ExplosionField, transform.position, quaternion.identity);
        
        //gameObject.SetActive(false);
        //릴리즈해요 == 풀에 반납할거에요
        m_IsMouseOver = false;
        Release();
    }
}
