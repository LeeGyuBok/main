using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 클래스는 ImmortalObject<T>라는 제네릭 클래스이며, T에 따라 동작이 달라짐
public class ImmortalObject<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            //컴파일 시점에서 T가 어떤 클래스인지 알 수 없다.
            //이 오브젝트에 붙어있는 스크립트의 T(ImmortalObject<T>)를 가지고온다.
            Instance = GetComponent<T>();
            if (Instance == null)
            {
                Instance = gameObject.AddComponent<T>();
            }
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
        
        //Debug.Log(Instance.gameObject.name);
    }
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
