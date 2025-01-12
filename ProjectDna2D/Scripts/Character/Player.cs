using System;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    public Rigidbody2D CharacterRb { get; private set; }
    [SerializeField] private GameObject character;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float climbSpeed;

    public Animator CharacterAnimator { get; private set; }
    public float MovementSpeed => movementSpeed;
    public float ClimbSpeed => climbSpeed;
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public Vector2 HorizontalDirection { get; private set; }
    public Vector2 LookDirection { get; private set; }
    public Vector2 VerticalDirection { get; private set; }
    
    [SerializeField] private float jumpForce;
    public float JumpForce => jumpForce;

    //public bool IsGrounded { get; set; }

    public IRigidbodyState CurrentRigidbodyState { get; private set; }
    public IdleRigidbodyState IdleRigidbodyState { get; private set; }
    public MoveRigidbodyState MoveRigidbodyState { get; private set; }
    public bool CanMoveHorizontal { get; set; }
    public bool CanMoveVertical { get; set; }
    public bool IsMoving { get; set; }
    public bool CanAttack { get; set; }
    public bool IsAttacking { get; set; }
    public JumpStartState JumpStartState { get; private set; }
    public FallingState FallingState { get; private set; }
    public LandingState LandingState { get; private set; }
    public JumpCancelState JumpCancelState { get; private set; }
    public bool IsAirBorne { get; set; }
    public RollingState RollingState { get; private set; }
    public StartAttackState StartAttackState {get; private set;}
    public StartMiddleAttackState StartMiddleAttackState { get; private set; }
    public MiddleEndAttackState MiddleEndAttackState { get; private set; } 
    public EndAttackState EndAttackState { get; private set; } 
    //public InHighestAttackState InHighestAttackState { get; private set; }
    public WallClimbIdleState WallClimbIdleState { get; private set; }
    public WallJumpState WallJumpState { get; private set; }
    public WallClimbingState WallClimbingState { get; private set; }
    public WallHangState WallHangState { get; private set; }
    public WallClimbOverState WallClimbOverState { get; private set; }
    public GroundSlamFlyingState GroundSlamFlyingState { get; private set; }
    public GroundSlamSlammingState GroundSlamSlammingState { get; private set; }
    public PlayerHitState PlayerHitState { get; private set; }
    public HitBoxType HitBoxType { get; set; } = HitBoxType.None;
    
    public PlayerAirborneState PlayerAirborneState { get; private set; }

    public bool AttachWall { get; set; }
    public bool OnWall { get; set; }
    
    public Vector2 OverPosition { get; set; }
    
    //public bool CanClimb { get; set; }
    
    private int _groundLayerMask;
    private int _wallLayerMask;
    
    public GameObject WallObject { get; private set; }
    [NonSerialized] public Vector2 WallOverCheckPosition;
    [NonSerialized] public Vector2 WallOverCheckDirection;
    
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Move = Animator.StringToHash("Move");
    public readonly int Jumping = Animator.StringToHash("Jumping");
    public readonly int Highest = Animator.StringToHash("Highest");
    public readonly int FallingAndCancel = Animator.StringToHash("FallingAndCancel");
    public readonly int Landing = Animator.StringToHash("Landing");
    public readonly int Rolling = Animator.StringToHash("Rolling");
    public readonly int StartAttack = Animator.StringToHash("StartAttack");
    public readonly int StartMiddleAttack = Animator.StringToHash("StartMiddleAttack");
    public readonly int MiddleEndAttack = Animator.StringToHash("MiddleEndAttack");
    public readonly int EndAttack = Animator.StringToHash("EndAttack");
    public readonly int GroundSlamFlying = Animator.StringToHash("GroundSlamFlying");
    public readonly int GroundSlamSlamming = Animator.StringToHash("GroundSlamSlamming");
    public readonly int WallClimbIdle = Animator.StringToHash("WallClimbIdle");
    public readonly int WallJump = Animator.StringToHash("WallJump");
    public readonly int WallClimbing = Animator.StringToHash("WallClimbing");
    public readonly int WallHang = Animator.StringToHash("WallHang");
    public readonly int WallClimbOver = Animator.StringToHash("WallClimbOver");
    public readonly int Stiff = Animator.StringToHash("Stiff");
    public readonly int Stun = Animator.StringToHash("Stun");
    public readonly int Airborne = Animator.StringToHash("Airborne");

    private string currentStateString;

    private void Awake()
    {
        IdleRigidbodyState = new IdleRigidbodyState();
        MoveRigidbodyState = new MoveRigidbodyState();
        JumpStartState = new JumpStartState();
        FallingState = new FallingState();
        JumpCancelState = new JumpCancelState();
        RollingState = new RollingState();
        LandingState = new LandingState();
        StartAttackState = new StartAttackState();
        StartMiddleAttackState = new StartMiddleAttackState();
        MiddleEndAttackState = new MiddleEndAttackState();
        EndAttackState = new EndAttackState();
        //InHighestAttackState = new InHighestAttackState();
        GroundSlamFlyingState = new GroundSlamFlyingState();
        GroundSlamSlammingState = new GroundSlamSlammingState();
        WallClimbIdleState = new WallClimbIdleState();
        WallJumpState = new WallJumpState();
        WallClimbingState = new WallClimbingState();
        WallHangState = new WallHangState();
        WallClimbOverState = new WallClimbOverState();
        PlayerHitState = new PlayerHitState();
        PlayerAirborneState = new PlayerAirborneState();
        CharacterAnimator = character.GetComponent<Animator>();
        _groundLayerMask = LayerMask.GetMask("Ground");
        _wallLayerMask = LayerMask.GetMask("Wall");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterRb = GetComponent<Rigidbody2D>();
        StateChange(IdleRigidbodyState);
        CharacterAnimator.ResetTrigger(Idle);
        OnTheGround(true);
        AttachedWall();

    }

    public void StateChangePublic(IRigidbodyState state)
    {
        StateChange(state);
    }
    
    private void StateChange(IRigidbodyState state)
    {
        if (CurrentRigidbodyState == null)
        {
            CurrentRigidbodyState = state;
            CurrentRigidbodyState.EnterState(this);
            return;
        }
        CurrentRigidbodyState?.ExitState(this);
        CurrentRigidbodyState = state;
        CurrentRigidbodyState.EnterState(this);
        currentStateString = CurrentRigidbodyState.ToString();
        //Debug.Log(currentStateString);//히트박스콜라이더에서 한번, 히트박스스테이트에서 한번
    }

    // Update is called once per frame
    void Update()
    {
        CurrentRigidbodyState?.Execute(this);
        //Debug.Log(CurrentRigidbodyState.ToString());
        
        if (Input.GetButton("Horizontal"))
        {
            //Debug.Log("getbuttondown");
            Horizontal = Input.GetAxis("Horizontal");
            if (CanMoveHorizontal)
            {
                if (Horizontal < 0)
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                HorizontalDirection = new Vector2(Horizontal, 0f).normalized;
                LookDirection = HorizontalDirection;
                IsMoving = true;
            }
        }
        if (Input.GetButton("Vertical"))
        {
            //Debug.Log("getbuttondown");
            Vertical = Input.GetAxis("Vertical");
            if (CanMoveVertical)
            {
                VerticalDirection = new Vector2(0f, Vertical).normalized;
            }
        }
        
        
        /*if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            OnTheGround(false);
            /*IsAirBorne = true;
            CanMoveHorizontal = false;
            CanAttack = false;#1#
        }*/
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (IsGrounded() && CanAttack)
            {
                IsAttacking = true;
            }
        }

        /*if (AttachWall)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                OnWall = true;
            }    
        }*/
        if (Input.GetButtonUp("Horizontal"))
        {
            Horizontal = 0;
            HorizontalDirection = Vector2.zero;
        }
        
        if (Input.GetButtonUp("Vertical"))
        {
            Vertical = 0;
            VerticalDirection = Vector2.zero;
        }

        
    }
    void FixedUpdate()
    {
        CurrentRigidbodyState?.FixedExecute(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer & _groundLayerMask) != 0)
        {
            OnTheGround(IsGrounded());
            return;
        }

        if ((1 << collision.gameObject.layer & _wallLayerMask) != 0)
        {
            AttachedWall(collision.gameObject);
            return;
        }
        
        if ((1 << collision.gameObject.layer & PlayerHitBoxController.EnemyLayerMask) != 0)
        {
            if (CurrentRigidbodyState.Equals(GroundSlamFlyingState))
            {
                collision.rigidbody.AddForce(((Vector2)collision.transform.position) - CharacterRb.position * 12f, ForceMode2D.Impulse);
            }
            return;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer & _groundLayerMask) != 0)
        {
            OnTheGround(false);
            return;
        }
        
        if ((1 << collision.gameObject.layer & _wallLayerMask) != 0)
        {
            AttachedWall();
            return;
        }
    }

    public void CanAct(bool canAct = true)
    {
        if (canAct)
        {
            CanMoveHorizontal = true;
            CanMoveVertical = true;
            CanAttack = true;
            return;
        }
        CanMoveHorizontal = false;
        CanMoveVertical = false;
        CanAttack = false;
    }
    
    public void OnTheGround(bool state)
    {
        //IsGrounded = state; // == canJump
        if (state)
        {
            CharacterRb.gravityScale = 1f;
            CanMoveHorizontal = true;
            CanAttack = true;
            IsAirBorne = false;
        }
        else
        {
            CanMoveHorizontal = false;
            CanAttack = false;
        
            IsAirBorne = true;
        }
        
        //Debug.Log(IsAirBorne);
    }

    private void AttachedWall(GameObject wallObject = null)
    {
        if (wallObject)
        {
            AttachWall = true;
            WallObject = wallObject;
            WallOverCheckPosition = new Vector2(WallObject.transform.position.x, CharacterRb.transform.position.y + 3f);
            WallOverCheckDirection = (WallOverCheckPosition - (Vector2)CharacterRb.transform.position).normalized;
            //Debug.DrawRay(CharacterRb.position, WallOverCheckDirection, Color.green);
        }
        else
        {
            AttachWall = false;
            WallObject = null;
        }
    }

    public void OnTheWall(bool state)
    {
        if (state)
        {
            OnWall = true;
            CanMoveVertical = true;
            CanMoveHorizontal = false;
            CanAttack = false;
            IsAirBorne = false;
        }
        else
        {
            OnWall = false;
            CanMoveVertical = false;
            CanMoveHorizontal = true;
            CanAttack = true;
        }
        
    }
    //얘가 온콜리전엔터2d에서 할것
    public bool IsGrounded()
    {
        Vector2 circlePosition = CharacterRb.position + Vector2.down * 0.65f;
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        return Physics2D.OverlapBox(circlePosition, new Vector3(0.4f,0.05f,0f), groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 cubePosition = gameObject.GetComponent<Rigidbody2D>().position + Vector2.down * 0.65f;
        Gizmos.DrawWireCube(cubePosition, new Vector3(0.4f,0.05f,0f));
    }
}
