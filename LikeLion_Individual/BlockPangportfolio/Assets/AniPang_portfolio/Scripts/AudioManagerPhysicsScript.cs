using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerPhysicsScript : MonoBehaviour
{
    public static AudioManagerPhysicsScript Instance { get; private set; } // 싱글톤 인스턴스

    private AudioSource audioSource;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 씬 전환 시에도 오디오 매니저 유지
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스는 파괴
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
