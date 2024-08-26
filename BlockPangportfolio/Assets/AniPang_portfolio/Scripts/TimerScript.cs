using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private AudioClip bgm;
    public float Timer { get; private set; }
    private AudioSource _audioSource;
    public event Action<TimerScript> TimerEnd;
    
    public static TimerScript Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 씬이 변경되어도 오브젝트가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하는 경우 현재 오브젝트를 파괴
        }

        Timer = bgm.length - 2f ;
        if (TryGetComponent(out _audioSource))
        {
            Instance._audioSource.clip = bgm;
            Instance._audioSource.volume = 0.2f;
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance._audioSource.Play();

    }

    // Update is called once per frame
    void Update()
    {
        Instance.Timer -= Time.deltaTime;
        End();
    }

    void End()
    {
        if (Timer <= 0)
        {
            //Debug.Log("End");
            TimerEnd?.Invoke(this);
        }
    }
}
