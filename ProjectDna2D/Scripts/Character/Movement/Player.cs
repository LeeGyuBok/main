using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public Rigidbody2D CharacterRb { get; private set; }
    
    [SerializeField] private float movementSpeed;
    public float MovementSpeed => movementSpeed;
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public Vector2 Direction { get; private set; }
    
    [SerializeField] private float jumpForce;
    public float JumpForce => jumpForce;

    public bool IsGrounded { get; private set; }

    public IRigidbodyState CurrentRigidbodyState { get; private set; }
    
    public IdleRigidbodyState IdleRigidbodyState { get; private set; }
    public MoveRigidbodyState MoveRigidbodyState { get; private set; }
    public IJumpCancel JumpCancel { get; private set; }
    public bool IsMove { get; set; }
    public IJumpStart JumpStart { get; private set; }
    public IFalling Falling { get; private set; }
    public bool IsAirBorne { get; set; }

    private int _groundLayerMask;

    private void Awake()
    {
        IdleRigidbodyState = new IdleRigidbodyState();
        MoveRigidbodyState = new MoveRigidbodyState();
        JumpStart = new IJumpStart();
        Falling = new IFalling();
        JumpCancel = new IJumpCancel();
        _groundLayerMask = LayerMask.GetMask("Ground");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterRb = GetComponent<Rigidbody2D>();
        StateChange(IdleRigidbodyState);
        
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
            return;
        }
        CurrentRigidbodyState?.ExitState(this);
        Debug.Log(CurrentRigidbodyState.ToString());
        CurrentRigidbodyState = state;
        
        CurrentRigidbodyState.EnterState(this);
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
            Direction = new Vector2(Horizontal, 0f).normalized;
            IsMove = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsAirBorne = true;
        }
        //Vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonUp("Horizontal"))
        {
            IsMove = false;
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
            IsAirBorne = false;
        }
    }
}
