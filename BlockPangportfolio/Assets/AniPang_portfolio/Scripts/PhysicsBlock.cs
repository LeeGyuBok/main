using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using FixedUpdate = Unity.VisualScripting.FixedUpdate;
using Random = UnityEngine.Random;


[RequireComponent(typeof(SphereCollider))]
public class PhysicsBlock : MonoBehaviour
{
    public event Action<PhysicsBlock> OnMouseClickBlockPhysics;
    public event Action<PhysicsBlock> DestroyEvent;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;
    private PhysicsBlockPool blockPool;
    

    public AudioClip disappear;


    //얘네가 주변에 같은 태그 붙은 애들이 들어오는 자리고
    public GameObject connectedUp { get; set; }
    public GameObject connectedDown{ get; set; }
    public GameObject connectedRight { get; set; }
    public GameObject connectedLeft { get; set; }
    

    //private static SphereCollider _sphereCollider;
    public Color Color { get; set; }

    public bool isMatch { get; set; }
    
    private bool selfDestroy;

    private bool EndOfGame;

    private void Awake()
    {
        /*TryGetComponent<MeshRenderer>(out _meshRenderer); // 트라이겟컴포넌트<가지고올컴포넌트타입>(out 해당컴포너트가 있다면 할당한 변수)
         트라이겟컴포넌트(out 이 변수의 컴포넌트타입과 일치하면 그 컴포넌트를 가져와서 할당함.)
         아래에 트라이겟컴포넌트 하나더있음. 꼭 읽어볼것
         Debug.Log(TryGetComponent(out _meshRenderer) ? "Success" : "Fail, Need To Repair");  이렇게도 가능해요*/
        if(TryGetComponent(out _meshRenderer))
        {
            
            Color = _meshRenderer.material.color;
            
            /*
             Debug.Log("Success");
             Debug.Log(Color); // RGBA로 나옴. 근데 A는 뭐죠? A는 투명도
             */
        }
        else
        {
            Debug.Log("Fail, Need To Repair");   
        }

        if (TryGetComponent(out _rigidbody))
        {
            //Debug.Log("success");
        }
        
        /*Debug.Log(TryGetComponent(out _sphereCollider) ? "Success" : "Fail, Need To Repair");*/
        isMatch = false;
        /*destroySide = false;*/
    }

    // Start is called before the first frame update
    void Start()
    {
        blockPool = FindObjectOfType<PhysicsBlockPool>();
        //타임 오버시 이벤트 등록
        TimerScript.Instance.TimerEnd += ViewingScore;
        EndOfGame = false;
    }

    // Update is called once per frame
    //여기서 계속 체크해서 
    void Update()
    {
        if (isMatch/*이게 트루일 때 파괴되도록했어요*/ || selfDestroy/*얘는 이제ㅐ 매칭되면 주변파괴 하려다가 그냥 남은애라 항상*/)
        {
            //Debug.Log($"{gameObject.transform.position}: 날 터뜨려주세요");
            AudioManagerPhysicsScript.Instance.PlaySound(disappear);
            
            //점수계산하고
            DestroyEvent?.Invoke(this);
            //블록 복귀
            
            /*Nullifying();*/
            blockPool.ReturnBlock(this);
            
        }

        if (TimerScript.Instance.Timer <= 0)
        {
            EndOfGame = true;
        }
    }
    
    
    private void FixedUpdate()
    {
        if (!EndOfGame)
        {
             if (_rigidbody.velocity.magnitude > 5)
             {
                 _rigidbody.velocity = Vector3.down;
             }
        }
    }
    
    private void OnMouseDown()
    {
        /*Debug.Log("OnMouseDown");*/
        //Debug.Log(transform.position);
        OnMouseClickBlockPhysics?.Invoke(this);
    }
    
    private void OnTriggerEnter(Collider other)//먼저 호출됨
    {
         Nullifying();
    }
    
    //폐기한 코드 여기 잠들다
    /*private void CompareTagIfEnter(Collider other)
    {
        //충돌시 태그비교
        //이즈트리거로 해놓은 스피어콜라이더에 닿으면 실행. transform. other another
        if (gameObject.CompareTag(other.tag)) //태그비교해서 같으면, 이제 방향을 찾아야함.
        {
            CompareMatchedBlockPosition(other.gameObject);
            //isMatch = true;
        }
        else//태그비교 결과, 태그가 다르면, ,그 방향에 해당하는 나의 connected변수를 null로 초기화하낟.
        {
            //other.gameObject의 방향을 찾아서
            CompareMatchExitBlockPosition(other.gameObject);
        }
    }*/

    /*private void OnTriggerExit(Collider other)
    {
        if (destroySide && other.gameObject.TryGetComponent(out PhysicsBlock sideBlock))
        {
            sideBlock.isMatch = true;
        }
    }*/

    /*private void OnMouseOver()
    {
        //이벤트가 구독된 상태라면 해당 함수를 호출한다.
        OnMouseOverBlock?.Invoke(this);
    }*/
    
    private void OnTriggerStay(Collider other)
    {
        //여기서부터 시작이에요 
        if (gameObject.TryGetComponent(out Rigidbody rigidbody) && !EndOfGame)
        {
            //충돌 체크는 충돌한 다른 콜라이더의 속도가 0일때 체크해요
            //사실 그거때문에 이 부분이 있던건데 저도 그게 왜 그러는지 아직도모르겠어요
            //다른 프로젝트에서는 안그랬던거같은데
            //키네마틱 - 아~ 아 이해했어요 네넼ㅋㅋ 알겠습니다. 다음에 테트리스 배열로 만든거 한 번 더 신청할게요 감사합니다.
            if (rigidbody.velocity.magnitude is >= -0.001f or < 0.001f)
            {
                CompareTagIfStay(other);
            }
            //이게 0.001 도 해보고 0.0001도 해봣는데 0이 제일 나은거같아서 0으로했어요
            //원래는 블럭끼리 충돌할 때 안튀어오르게 하고싶었는데 이게 도저히 안되가지고 다른 방법으로 했어요
            
            //어떤매니저요? 게임매니저랑 오디오매니저 둘 게임매니저로가요? 네네
            /*if (bCompareOk)//코드리뷰받은 코드
            {
                CompareTagIfStay(other);
            }*/
        }
    }

    /*IEnumerator WaitFixedUpdate()//코드리뷰받은 코드
    {
        bCompareOk = true;
        yield return new FixedUpdate();
        bCompareOk = false;
    }

    private bool bCompareOk = false;
    
    public void CompareTagIfStay_J()
    {
        StartCoroutine(WaitFixedUpdate());
    }*/

    private void CompareTagIfStay(Collider other)
    {
        //지속적인 태그비교
        if (gameObject.CompareTag(other.tag)) //태그비교해서 같으면, 이제 방향을 찾아야함.
        {
            //여기서 이제 주변 블럭위치 찾고
            CompareMatchedBlockPosition(other.gameObject);
            
            //여기는 가로세로로 3개가 같은 색인지 체크하는 부분
            OverTripleMatch();
            //isMatch = true;
        }
        else//태그비교 결과, 태그가 다르면, ,그 방향에 해당하는 나의 connected변수를 null로 초기화하낟.
        {
            //other.gameObject의 방향을 찾아서
            //멀리 있는애들이라는게 XX0000XX 일때 X끼리 /
            //애초에 레이캐스트가아니고 구콜라이더로 바로 옆에있는 애들만 체크하는거라서 멀리있는애들은 체크를 못해요
            CompareMatchExitBlockPosition(other.gameObject);
        }
    }

    private void CompareMatchedBlockPosition(GameObject other)
    {
        //나랑 콜라이더가 부딪친 아이가 X좌표가 나와 같아.
        //여기서 주변부 애들 체크하고
        //충돌로 해놔가지고 아마 다 돌거에요
        if (((int)other.transform.position.x).Equals((int)transform.position.x))
        {
            CompareMatchedBlockYPosition(other);
        }
        else//나랑 콜라이더가 부딪친아이가 내 왼쪽 또는 오른쪽에 있어. 달라.
        {
            CompareMatchedBlockXPosition(other);
        }
        /*if ((int)Math.Round(other.transform.position.y) > (int)Math.Round(transform.position.y))
        {
            이게 같은 것에 대해서 두번비교해서 두번들어가네..
        }*/
    }
    
    private void CompareMatchedBlockXPosition(GameObject other)
    {
        if ((int)other.transform.position.x > (int)transform.position.x)//오른쪽에있어
        {
            if (connectedRight == null)
            {
                connectedRight = other;
                if (connectedRight.Equals(connectedLeft))
                {
                    connectedLeft = null;
                }
                //Debug.Log($"{transform.position}: right");
                
            }
                   
        }
        else//왼쪽에 있어
        {
            if (connectedLeft == null)
            {
                connectedLeft = other;
                if (connectedLeft.Equals(connectedRight))
                {
                    connectedRight = null;
                }
                //Debug.Log($"{transform.position}: left");
            }
        }
    }

    private void CompareMatchedBlockYPosition(GameObject other)
    {
        if ((int)Math.Round(other.transform.position.y) > (int)Math.Round(transform.position.y)) //내 위에 있어
        {
            if (connectedUp == null)
            {
                connectedUp = other;
                if (connectedUp.Equals(connectedDown))
                {
                    connectedDown = null;
                }
                //Debug.Log($"{transform.position}: up");
            }
        }
        else //내 밑에 있어
        {
            if (connectedDown == null)
            {
                connectedDown = other;
                if (connectedDown.Equals(connectedUp))
                {
                    connectedUp = null;
                }
                //Debug.Log($"{transform.position}: down");    
            }
        }
    }
    
    private void CompareMatchExitBlockPosition(GameObject other)
    {
        if (((int)other.transform.position.x).Equals((int)transform.position.x))
        {
            CompareMissMatchedBlockYPosition(other);
        }

        if ((int)Math.Round(other.transform.position.y) > (int)Math.Round(transform.position.y))
        {
            CompareMissMatchedBlockXPosition(other);
        }
    }
    
    private void CompareMissMatchedBlockXPosition(GameObject other)
    {
        if ((int)other.transform.position.x > (int)transform.position.x)//오른쪽에있어
        {
            if (connectedRight != null)
            {
                connectedRight = null;
               // Debug.Log($"{transform.position}: right");
            }
                   
        }
        else//왼쪽에 있어
        {
            if (connectedLeft != null)
            {
                connectedLeft = null;
               // Debug.Log($"{transform.position}: left");
            }
        }
    }

    private void CompareMissMatchedBlockYPosition(GameObject other)
    {
        if ((int)other.transform.position.y > (int)transform.position.y) //내 위에 있어
        {
            if (connectedUp != null)
            {
                connectedUp = null;
                //Debug.Log($"{transform.position}: up");
            }
        }
        else //내 밑에 있어
        {
            if (connectedDown != null)
            {
                connectedDown = null;
                //Debug.Log($"{transform.position}: down");    
            }
        }
    }

    private void OverTripleMatch()
    {
        //여기서 위아래
        if (connectedUp && connectedDown)
        {
            isMatch = true;
            connectedUp.TryGetComponent(out PhysicsBlock upBlock);
            connectedDown.TryGetComponent(out PhysicsBlock downBlock);
            upBlock.isMatch = true;
            downBlock.isMatch = true;
            
            return;
        }
        //좌우로 같은 블럭 체크되면 
        if (connectedRight && connectedLeft)
        {
            //체크해주고
            isMatch = true;
            //좌우에있는애들 가지고와서
            //그 저번에 다른분들이 알려주셨던 사이트 보고 저거 적용해봤어요
            //아아 항상 붙어있을 애들이라 안했는데 사용목적이 조금 어긋나긴하네요 네네
            //배열로 하면 좀 더 나을 것 같긴해요
            //그렇..죠 배열로 해보려다가 말씀하신대로 복잡하고 그래가지고 그냥 노선틀었어요 넵넵
            connectedRight.TryGetComponent(out PhysicsBlock rightBlock);
            connectedLeft.TryGetComponent(out PhysicsBlock leftBlock);
            //걔네들도 체크됐다고 해줘요
            
            rightBlock.isMatch = true;
            leftBlock.isMatch = true;
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
        if (isMatch)
        {
            Debug.Log(isMatch);
            if (other.TryGetComponent(out PhysicsBlock sideBlock))
            {
                Debug.Log(sideBlock.selfDestroy);
                sideBlock.selfDestroy = true;
            }
        }

    }*/

    private void ViewingScore(TimerScript timerScript)
    {
        if (EndOfGame)
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
            Vector3 randomDirection = new Vector3(
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f)
            ).normalized;
            _rigidbody.AddForce(randomDirection * 5f, ForceMode.Impulse);
        }
    }

    public void Nullifying()
    {
        selfDestroy = false;
        connectedUp = null;
        connectedDown = null;
        connectedRight = null;
        connectedLeft = null;
        isMatch = false;
    }

    /*public float GetSpeed()//코드리뷰받은 코드
    {
        return _rigidbody.velocity.magnitude;
    }*/
}
