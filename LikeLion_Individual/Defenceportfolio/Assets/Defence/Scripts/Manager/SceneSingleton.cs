using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//씬싱글톤은 이 씬에서만 존재하는 싱글톤
//나를 상속받는 애들은 기본적인 싱글톤이 구현돼
public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject prefab = Resources.Load(typeof(T).Name) as GameObject;
                GameObject singleton = Instantiate(prefab);
                _instance = singleton.GetComponent<T>();
            }
            
            return _instance;
        }
    }
    
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
    }
}
