using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Bird : MonoBehaviour
{
    //앵그리버드의 최초 생성위치
    protected Vector3 m_SpawnPosition;
    
    //앵그리버드의 현재위치 -> 그냥 transform.position 사용
    //protected Vector3 m_CurrentPosition;
    
    //앵그리버드의 발사 위치
    protected Vector3 m_LaunchPosition;
    
    //앵그리버드가 보는 방향
    protected Quaternion m_LookDirection;
    
    //앵그리버드의 2D 물리컴포넌트 
    private Rigidbody2D m_BirdRigidbody;
    //앵그리버드의 애니메이션 컨트롤러 컴포넌트
    private Animator m_BirdAnimator;
    //앵그리버드의 사운드 컴포넌트
    private AudioSource m_BirdAudioSuource;
    
    
    
    //앵그리버드의 발사대 오브젝트
    [SerializeField] protected GameObject m_Launcher;
    //앵그리버드의 발사대 오브젝트의 위치
    protected Vector3 m_LauncherLocation;

    //마우스 클릭 위치
    protected Vector3 m_FirstClickPosition;
    
    //마우스 놓는 위치
    protected Vector3 m_RealesePosition;
    
    //발사각
    protected Vector3 m_LaunchDirection;
    
    //최대 발사 힘
    [SerializeField] protected float m_MaxPower;
    
    //발사힘 계수
    [SerializeField] protected float m_PowerCoeffieciont;

    //현재 발사 힘을 계산할 거리
    protected float m_Distacne;
    
    //최종 발사 힘
    protected float m_LaunchPower;
    
    //최종 발사 벡터
    protected Vector3 m_LaunchForce;
    
    //오프셋은 클릭위치와 버드 오브젝트의 중심간의 거리차이
    protected Vector3 offset;
    
        
    
    //버드 오브젝트에 마우스를 올렸는지?
    protected bool m_IsMouseOver;
    
    //버드 오브젝트를 클릭했는지
    protected bool m_IsClicked;
    
    //버드 오브젝트가 발사중인지
    protected bool m_IsLaunch;
    
    

    //풀 변수 -> 돌아가야할 집
    private BirdPool m_pool;

    //겟 셋
    public BirdPool m_Pool
    {
        get => m_pool;
        set => m_pool = value;
    }

    protected virtual void Release()
    {
        Debug.Log("릴리즈는 읽어");
        m_pool.ReturnToPool(this);
        Debug.Log("풀로 돌아가요");
        m_pool.SpawnBird();
        Debug.Log("새가 나타나요");
    }
    
    
    
    //카메라. 좌표를 확인하고 계산하기 위함.
    protected Camera mainCamera;
    
    //잡아당기는 효과
    protected LineRenderer lineRenderer;
    protected Vector3 m_LineRendererStartPosition1;
    protected Vector3 m_LineRendererStartPosition2;
    
    //새가 날아갈 궤적그리기
    public TrajectoryPointer trajectoryPointer;
    
    //중력 스케일
    private float gravityScale = 1.0f;


    private void Awake()
    {
        //카메라 할당
        mainCamera = Camera.main;
        
        //라인렌더러 컴포넌트 할당
        lineRenderer = GetComponent<LineRenderer>();
        
        //점 4개로 그려요. 앞쪽에 선분하나(점 두개), 뒤쪽에 선분하나(점 두개) == 총 4개
        lineRenderer.positionCount = 4; 
        
        //너비?! 가 중요했다...
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        //색깔 무엇?
        Color brown = new Color(0.6f, 0.3f, 0.1f);
        lineRenderer.startColor = brown;
        lineRenderer.endColor = brown;
        
        //버드의 컴포넌트 할당
        m_BirdRigidbody = GetComponent<Rigidbody2D>();
        m_BirdAnimator = GetComponent<Animator>();
        m_BirdAudioSuource = GetComponent<AudioSource>();

        m_BirdRigidbody.gravityScale = 0;

        //초기에 배치된 위치와 회전을 저장한다.
        m_SpawnPosition = transform.position;
        m_LookDirection = transform.rotation;
        
        //상태를 체크할 불값을 초기화한다.
        m_IsLaunch = false;
        m_IsMouseOver = false;
        m_IsClicked = false;

    }

    protected void ProtectedAwake()
    {
        Awake();
    }

    private void OnEnable()
    {
        //애니메이션 재생을 위해 애니메이터의 스피드를 초기화
        m_BirdAnimator.speed = 1;
    }

    //봄버드를 위한 프로텍트함수
    protected void ProtectedOnEnable()
    {
        OnEnable();
    }

    //버드가 풀로 돌아갈 때, 즉 비활성화될 때.
    private void OnDisable()
    {
        m_BirdRigidbody.gravityScale = 0;

        //초기에 배치된 위치와 회전을 저장한다.
        transform.position = m_SpawnPosition;
        transform.rotation = m_LookDirection;
        
        m_IsLaunch = false;
        m_IsClicked = false;
        
        m_FirstClickPosition = Vector3.zero;
        
        offset = Vector3.zero;
        m_Distacne = 0.0f;
        
        m_RealesePosition = Vector3.zero;
        m_LaunchPosition = Vector3.zero;
    }
    
    //봄버드를 위한 프로텍트함수
    protected void ProtectedOnDisable()
    {
        OnDisable();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*Debug.Log(m_Launcher);//있어요*/
        /*Debug.Log(m_Launcher.transform.position);//있어요*/
        m_LauncherLocation = m_Launcher.transform.position;
        Debug.Log(m_LauncherLocation);//있어요
        
    }

    protected void ProtectedStart()
    {
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //마우스를 올려놓는다 == 이 새를 선택하겠다.
    //업데이트 함수에서 마우스 인풋을 지속적으로 받는 것보다
    //이미 정의되어있는 함수를 쓰는 것이 나을 것 같다고 생각.
    private void OnMouseOver()
    {
        if (!m_IsMouseOver)
        {
            //Debug.Log("나야? 나야? 정말이야? 나야?"); //돼요
            //나는 진짜 개멍청이이다.
            //할당한 오브젝트의 위치를 가지고올 때, 프리팹화한, 가지고올 오브젝트의 위치를 제!대!로! 조정하자.
            transform.position = m_LauncherLocation;
            m_BirdRigidbody.velocity = new Vector2(0, 0);
            m_IsMouseOver = true;
            //Debug.Log("난 이동했음");
        }
    }

    //봄버드를 위한 프로텍트함수
    protected void ProtectedOnMouseOver()
    {
        OnMouseOver();
    }

    //새를 눌러요
    private void OnMouseDown()
    {
        if (!m_IsClicked)
        {
            //Debug.Log("눌러요");//돼요
            //Debug.Log("LeftClick");
            //화면상에 마우스포인터의 위치를 인게임 공간상의 위치로 변환한다.
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mousePosition);
        
            // 꼭 오브젝트의 중심을 선택하지 않더라도 오브젝트가 부드럽게 따라옴.
            offset = transform.position - (Vector3)mousePosition;
                
            //처음 클릭한 위치를 저장. 왜냐하면 이 위치에서 일정거리까지만 이동하게 하기 위함.
            m_FirstClickPosition = mousePosition;
            m_IsClicked = true;
        }
    }
    
    //봄버드를 위한 프로텍트함수
    protected void ProtectedOnMouseDown()
    {
        OnMouseDown();
    }
    
    //새를 움직여요
    private void OnMouseDrag()
    {
        if (m_BirdAnimator != null)
        {
            //애니메이션을 정지한다.
            //Debug.Log(gameObject.GetComponent<Animator>());
            //Debug.Log("현재 위치. 버드의 온마우스드래그");
            m_BirdAnimator.speed = 0;
        }
        //Debug.Log("끌어요");//돼요
        // 마우스가 움직이는 동안 오브젝트를 이동
        // 드래그 동안 화면상에 마우스포인터의 위치를 인게임 공간상의 위치로 계속 변환한다.
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 오브젝트의 위치를 마우스포인터의 위치와 오프셋의 차이를 더해서 설정한다. == 이동한다.
        Vector3 rawPosition = mousePosition + (Vector2)offset;
            
        //일정거리. 디스턴스가 생각보다 매우 크다... 개오바다...
        //아 이것때문에 2일을 날렸네. 디버그 로그는 신이다. <- 숭배하라.
        m_Distacne = Vector3.Distance(m_FirstClickPosition, rawPosition);
        
        //Debug.Log("distance" + m_Distacne);
        
        //만약 처음 누른 위치에서 오브젝트의 위치가 일정거리(MaxPower의 성분)이상 벗어나게되면
        if (m_Distacne > m_MaxPower/10)
        {
            //벡터를 계산해서 방향벡터화한다.
            Vector3 rawDirection = (rawPosition - m_FirstClickPosition).normalized;
                
            //위치를 재 조정한다.
            rawPosition = m_FirstClickPosition + rawDirection * m_MaxPower/10;
        }
        //조정한 위치를 이 오브젝트(버드)의 최근위치로 할당한다.
        //오브젝트를 움직인다.
        transform.position = rawPosition;
        
        m_RealesePosition = transform.position;
        //놓는 위치를 발사 위치로 할당
        m_LaunchPosition = m_RealesePosition;
        //날아갈 방향
        m_LaunchDirection = m_LauncherLocation - m_LaunchPosition;
        //최종 발사힘
        m_LaunchPower = m_Distacne * m_PowerCoeffieciont;
        Debug.Log("Launch: " + m_LaunchPower);
        //발사힘 제한
        if (m_LaunchPower >= m_MaxPower)
        {
            m_LaunchPower = m_MaxPower;
        }
        //최종적 발사벡터 및 힘
        m_LaunchForce = m_LaunchDirection.normalized * m_LaunchPower;
        
        DrawLine(transform.position);
        RenderTrajectory(gravityScale);


    }

    //날아갈 궤적을 그릴 때, 봄버드와 버드의 그라비티스케일이 달라서 궤적을 다르게 그려야하므로
    //봄버드가 자신의 그라비티스케일을 적용할 수 있게(오버라이드 할 수 있게) 버추얼 선언
    protected virtual void RenderTrajectory(float _gravityScale)
    {
        //Debug.Log("그릴게");
        Debug.Log(m_LaunchForce);
        trajectoryPointer.PublicRenderTrajectory(transform.position, m_LaunchForce, _gravityScale);
        //Debug.Log("그렸게어");
    }
    
    //새총효과를 렌더러로 표현
    private void DrawLine(Vector3 _rawPosition)
    {
        //잡아당기는효과부분
        lineRenderer.enabled = true;
        //새총의 위치를 기준으로 그려요
        //여기는 앞쪽
        m_LineRendererStartPosition1 = m_LauncherLocation + new Vector3(0.5f, 0, -0.1f);
        m_LineRendererStartPosition2 = m_LauncherLocation + new Vector3(-0.38f, 0, 0);
        lineRenderer.SetPosition(0, (m_LineRendererStartPosition1));
        lineRenderer.SetPosition(1, _rawPosition);
        //여기는 뒤쪽
        lineRenderer.SetPosition(2, m_LineRendererStartPosition2);
        lineRenderer.SetPosition(3, _rawPosition);
    }
    
    //봄버드를 위한 프로텍트 함수
    protected void ProtectedOnMouseDrag()
    {
        OnMouseDrag();
    }

    //새를 놔줘요
    private void OnMouseUp()
    {
        //잡아당기는효과부분 끄기
        lineRenderer.enabled = false;
        
        //최종적 발사벡터 및 힘
        m_LaunchForce = m_LaunchDirection.normalized * m_LaunchPower;
        
        if (m_LaunchPower >= 8.0f)
        {
            m_BirdAudioSuource.Play();
            Debug.Log(m_LaunchForce);
            ChangeGravity(1.0f);
            m_BirdRigidbody.AddForce(m_LaunchForce, ForceMode2D.Impulse);
            m_IsLaunch = true;
            trajectoryPointer.HideTrajectory();
        }
        else //최소 발사힘에 도달하지 못하면 모두초기화
        {
            lineRenderer.enabled = false;
            trajectoryPointer.HideTrajectory();
            ResetState();
        }
    }

    //봄버드의 그라비티 스케일이 달라요. 그래서, 봄버드가 오버라이드 할 수 있도록 버추얼로 선언
    protected virtual void ChangeGravity(float _gravity)
    {
        m_BirdRigidbody.gravityScale = _gravity;
    }

    //봄버드를 위한 프로텍트함수
    protected void ProtectedOnMouseUp()
    {
        OnMouseUp();
    }

    /*private void OnCollisionEnter(Collision other) //3d 에서 쓰시오
    {
        Debug.Log("Crash");
        
    }*/
    //무언가에 닿았어요
    private void OnCollisionEnter2D(Collision2D other)
    {
        //발사한 상태면
        if (m_IsLaunch)
        {
            //코루틴을 실행해요
            StartCoroutine(ObjectDisable());
            //발사상태를 꺼요
            m_IsLaunch = false;
            
        }

        //현재 상태가 발사하지도 않았고 클릭도 안했다. 그런데 마우스는 올렸다.
        if (!m_IsLaunch && !m_IsClicked && m_IsMouseOver)
        {
            ResetState();
            Release();
        }
    }
    

    protected virtual IEnumerator ObjectDisable()
    {
        //n초를 기다려요
        yield return new WaitForSeconds(2.0f);
        Debug.Log("사라진다 뿅");
        //gameObject.SetActive(false);
        //릴리즈해요 == 풀에 반납할거에요
        m_IsMouseOver = false;
        Release();
    }
    
    protected void ResetState()
    {
        if (m_BirdAnimator != null)
        {
            m_BirdAnimator.speed = 1;
        }
        m_IsClicked = false;
        m_IsMouseOver = false;
        m_IsLaunch = false;
        transform.position = m_SpawnPosition;
        m_FirstClickPosition = Vector3.zero;
        offset = Vector3.zero;
        m_Distacne = 0.0f;
        m_RealesePosition = Vector3.zero;
        m_LaunchPosition = Vector3.zero;
    }
    
    
}
