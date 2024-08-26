using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RunnerScript : MonoBehaviour
{
    public bool GameStart { get; set; }
    
    private Animator _animator;
    private TimerScript _timer;
    private float runnerTimer;
    float initialZ;
    float initialX;
    private bool LastTurn { get; set; }
    
    private static readonly int Start1 = Animator.StringToHash("Start");


    private void Awake()
    {
        GameStart = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out _animator))
        {
            
            //Debug.Log("_animator");
        }
        else
        {
            //Debug.Log("error");
        }
        
        if (FindObjectOfType<TimerScript>())
        {
            _timer = TimerScript.Instance;
           // Debug.Log("_timer");
            runnerTimer = _timer.Timer;
        }
        else
        {
          //  Debug.Log("error");
        }

        initialX = transform.position.z;
        GameStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (runnerTimer > 0)
        {
            runnerTimer -= Time.deltaTime;    
        }

        
        
    }

    private void FixedUpdate()
    {
        if (GameStart)
        {
            
            _animator.SetBool(Start1, GameStart);
            transform.Translate(Vector3.forward * 0.028f);
            if (transform.position.x > 32.0f)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (transform.position.z  > initialX + 11.5f)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            if (transform.position.z < 3.0f)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            if (transform.position.x <-23.0f)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            if (transform.position.z < initialX)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                LastTurn = true;
            }

            if (LastTurn && transform.position.x > 5.9f)
            {
                _animator.SetBool(Start1, !GameStart);
                GameStart = false;
            }
        }

        if (runnerTimer <= 0)
        {
            transform.rotation = new Quaternion(0, 180f, 0, 0);
            if (TryGetComponent(out Rigidbody _rigidbody))
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

            
        }
    }
}
