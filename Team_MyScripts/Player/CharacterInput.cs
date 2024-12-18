using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    private static CharacterInput instance;
    public static CharacterInput Instance
    {
        get {
            if (instance == null)
            {
                instance = FindFirstObjectByType<CharacterInput>();
                
                if (instance == null)
                {
                    GameObject singletonObj = new GameObject("CharacterInput");
                    instance = singletonObj.AddComponent<CharacterInput>();
                }
                
                DontDestroyOnLoad(instance);
            }

            return instance;
        }
    }

    private Vector2 move;
    public Vector2 MoveInput
    {
        get {return move;} 
    }
    
    public bool IsRun => Input.GetKey(KeyCode.LeftShift) ? true : false;

    private Vector2 cameraPosition;

    public Vector2 CameraPositionInput
    {
        get { return cameraPosition; }
    }
    
    //플레이어의 인풋을 막아요
    public bool Input_Block { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    

    /*// Start is called before the first frame update
    void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        move.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        cameraPosition.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));    
    }
}
