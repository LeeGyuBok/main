using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Character_Controller : MonoBehaviour, IHealthpower, ISkillable
{
    //컴포넌트들
    //플레이어의 입력을 받는 스크립트
    private CharacterInput _characterInput;
    private Rigidbody characterRigidbody;
    
    //애니메이터
    private Animator characterAnimator;
    
    //컴포넌트끝

    
    //플레이어가 입력한 값이 유효한지?
    private bool IsMoveInput
    {
        get { return !Mathf.Approximately(_characterInput.MoveInput.sqrMagnitude, 0f); }
    }
    
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraLocation;

    
    private Vector3 moveDirection;

    private bool isRotate;
    public bool IsRotate
    {
        get { return isRotate;}
        set { isRotate = value; }
    }

    private bool isMove;
    public bool IsMove
    {
        get { return isMove;}
        set { isMove = value; }
    }

    private bool isHang;

    public bool IsHang
    {
        get { return isHang; }
        set { isHang = value; }
    }

    private const float groundDeceleration = 0.9f;

    //트랜지션 파라미터
    private readonly int hashIsWalk = Animator.StringToHash("IsWalk");
    private readonly int hashIsRun = Animator.StringToHash("IsRun");
    private readonly int hashIdleTimer = Animator.StringToHash("IdleTimer");

    private void Awake()
    {
        characterAnimator = characterBody.GetComponent<Animator>();
        characterRigidbody = GetComponent<Rigidbody>();
        characterRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        _characterInput = CharacterInput.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        CalculateMove();
        InputSkill();
    }
    
    // FixedUpdate is called once per end of frame
    void FixedUpdate()
    {
        Translate();
    }

    private void CalculateMove()
    {
        //Debug.Log(IsMoveInput);
        //wasd를 눌러 이동명령을 받는다.
        Vector2 moveInput = _characterInput.MoveInput;
        //카메라기준 이동벡터 형성
        Vector3 lookForward = new Vector3(cameraLocation.forward.x, 0f, cameraLocation.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraLocation.right.x, 0f, cameraLocation.right.z).normalized;
        //이동벡터 계산하고
        Vector3 direction = lookForward * moveInput.y + lookRight * moveInput.x;
        //대각이동시에도 이동속도를 일정하게 하기위해 노멀라이즈드 == 방향벡터만 추출
        moveDirection = direction.normalized;

        if (IsMoveInput)
        {
            //Vector3.RotateTowards 함수는 현재 방향 벡터를 목표 방향 벡터로 일정한 각속도로 회전시키는 기능을 제공
            //회전한다.
            /*characterBody.forward = Vector3.RotateTowards(characterBody.forward, moveDirection,
                rotateSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 1000f);*/
            Vector3 rotateTowards = Vector3.RotateTowards(characterBody.forward, moveDirection,
                rotateSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 1000f);

            characterBody.forward = rotateTowards == Vector3.zero ? Vector3.forward : rotateTowards;


            RotateForward(moveDirection);
            IsMove = true;
            //characterAnimator.SetBool(hashIsWalk, true);
        }
        else
        {
            IsMove = false;
            //characterAnimator.SetBool(hashIsWalk, false);
        }
    }
    
    void RotateForward(Vector3 _moveDir)
    {
        //현재 바라보고 있는 방향과, 이동하려는 방향의 각도차이를 계산한다.
        float angleDifference = Vector3.Angle(characterBody.forward, _moveDir);

        if (angleDifference < 115.0f)//회전끝
        {
            IsRotate = false;
        }
        else//회전중
        {
            //나 회전중이야
            IsRotate = true;
        }
    }

    void Translate()
    {
        if (IsMove)
        {
            //walk
            characterRigidbody.velocity = Vector3.Lerp(characterRigidbody.velocity, moveDirection * moveSpeed, Time.fixedDeltaTime*moveSpeed);
            bool isRun = _characterInput.IsRun;
            //Debug.Log(isRun + "CharController");
            //run, when player press leftShift
            if (isRun)
            {
                //characterAnimator.SetBool(hashIsRun, isRun);
                characterRigidbody.velocity = Vector3.Lerp(characterRigidbody.velocity, moveDirection * (moveSpeed * 2f), Time.fixedDeltaTime*moveSpeed);
            }
            /*else*/
                //characterAnimator.SetBool(hashIsRun, isRun);
        }
        else
        {
            characterRigidbody.velocity *= groundDeceleration;
            if (characterRigidbody.velocity.magnitude < 0.1f)
            {
                characterRigidbody.velocity = Vector3.zero;
            }

            /*if (characterAnimator.GetBool(hashIsRun))
            {
                characterAnimator.SetBool(hashIsRun, false);
            }*/
        }
        if (IsRotate)
        {
            //회전하는 동안느려질게?
            characterRigidbody.velocity = Vector3.Lerp(characterRigidbody.velocity, moveDirection * moveSpeed / 4f, Time.fixedDeltaTime*moveSpeed);    
        }
    }
    
    void Look()
    {
        if (!_characterInput.Input_Block)
        {
            //우리가 보는 화면은 2차원 평면이다. 마우스는 평면안에 있기 때문에, 그 위치를 좌표평면상에 나타낼 수 있다.
            Vector2 mouseDelta = _characterInput.CameraPositionInput;
        
            //쿼터니언을 오일러각으로 변환하여 x,y,z축에 대한 회전으로 분해하여 표현.
            //카메라포지션의 현재 회전을 오일러각으로 변환하여 저장.
            Vector3 camAngle = cameraLocation.rotation.eulerAngles;
        
            //2. 이 코드가 회전각도를 제한
            float x = camAngle.x - mouseDelta.y;
            if (x < 180f) 
            {
                //아래로 볼 때 각도 max 조정
                x = Mathf.Clamp(x, -1f, 75f);
            }
            else
            {
                //위로볼 때 각도
                x = Mathf.Clamp(x, 315f, 361f);
            }
        
            //1. 이 코드만 존재하면 회전각도를 제한하지 않음
            //마우스의 x좌표(mouseDelta.x)가 바뀐다는 것은 Y축(camAngle.y)을 기준으로 바뀐다.
            //마우스의 y좌표(mouseDelta.y)가 바뀐다는 것은 X축(camAngle.x)을 기준으로 바뀐다.
            cameraLocation.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
        }
    }

    /*지형지물 디버그용
     private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.transform.parent != null)
        {
            Debug.Log(other.gameObject.transform.parent.gameObject.name);    
        }
        else
        {
            Debug.Log(other.gameObject.name);
        }
        
    }*/

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }*/
    
    
    //이 이하를 인터페이스화 해서 떼야한다.
    public bool IsGetBarrier { get; private set; }
    //체력
    public int MaxHealthPower { get; } = 100;
    public int CurrentHealthPower { get; private set; } = 100;
    public bool IsDead { get; private set; } = false;
    public void UnderAttack(int dmg)
    {
        if (IsGetBarrier)
        {
            return;
        }
        
        CurrentHealthPower -= dmg;
        if (CurrentHealthPower <= 0) 
        {
            if (!IsDead)
            {
                IsDead = true;
            }
        }
    }

    // 스킬시스템

    public Liminex liminex { get; private set; }
    public void UseSkill(int keycode)
    {
        if (liminex == null)
        {
            foreach (Stack<Item_SO> itemStack in Inventory.Instance.inventory_KeyItem.Values)
            {
                if (itemStack.Peek() is Liminex liminexItem)
                {
                    liminex = liminexItem;
                    if (liminex.IsActivate)
                    {
                        switch (keycode)
                        {
                            case 1:
                                SkillManager.Instance.ActivateSkill(liminex.UseSkill(gameObject, liminex.Blink));
                                return;
                            case 2:
                                SkillManager.Instance.ActivateSkill(liminex.UseSkill(gameObject, liminex.Barrier));
                                return;
                            default:
                                Debug.Log("noSkill");
                                return;
                        }
                    }
                    else
                    {
                        Debug.Log("not activate");
                        return;
                    }
                }
                return;
            }
        }
        else
        {
            if (liminex.IsActivate)
            {
                switch (keycode)
                {
                    case 1:
                        SkillManager.Instance.ActivateSkill(liminex.UseSkill(gameObject, liminex.Blink));
                        return;
                    case 2:
                        SkillManager.Instance.ActivateSkill(liminex.UseSkill(gameObject, liminex.Barrier));
                        return;
                    default:
                        Debug.Log("noSkill");
                        return;
                }
            }
        }
    }

    private void InputSkill()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int keycode = 1;
            Debug.Log(keycode);
            UseSkill(keycode);    
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int keycode = 2;
            Debug.Log(keycode);
            UseSkill(keycode);   
        }
    }

    public void SetBarrier()
    {
        IsGetBarrier = true;
    }

    public void DestroyBarrier()
    {
        IsGetBarrier = false;
    }
}
