using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private static PlayerInput instance;
    public static PlayerInput Instance
    {
        get {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerInput>();
                
                if (instance == null)
                {
                    GameObject singletonObj = new GameObject("PlayerInput");
                    instance = singletonObj.AddComponent<PlayerInput>();
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

    private Vector2 cameraInput;

    public Vector2 CameraInputInput
    {
        get { return cameraInput; }
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
        cameraInput.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));    
    }
}
