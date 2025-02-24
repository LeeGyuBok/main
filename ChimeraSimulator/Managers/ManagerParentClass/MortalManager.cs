using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MortalManager<T> : MonoBehaviour where T : MortalManager<T>
{
    public static T Instance{get; private set;}

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
            if (Instance == null)
            {
                Instance = gameObject.AddComponent<T>();
            }
        }
        //Debug.Log(Instance.gameObject.name);
    }
}
