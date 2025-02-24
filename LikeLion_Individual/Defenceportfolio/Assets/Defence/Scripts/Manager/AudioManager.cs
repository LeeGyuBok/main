using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    //지피티의 오디오매니저지만 형식을 어느정도는 외워놓자
    public static AudioManager Instance { get; private set; }
    
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
    }

    public Sound[] sounds;

    private Dictionary<string, AudioSource> soundDictionary;

    void Awake()
    {
        // 싱글턴 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        soundDictionary = new Dictionary<string, AudioSource>();

        foreach (var sound in sounds)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch;
            audioSource.loop = sound.loop;
            soundDictionary[sound.name] = audioSource;
        }
    }

    public void Play(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            soundDictionary[name].Play();
        }
        else
        {
            Debug.LogWarning($"Sound: {name} not found!");
        }
    }

    public void Stop(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            soundDictionary[name].Stop();
        }
        else
        {
            Debug.LogWarning($"Sound: {name} not found!");
        }
    }
    
    public void PlayRandomSound(List<string> soundNames)
    {
        int randomIndex = Random.Range(0, soundNames.Count);
        Play(soundNames[randomIndex]);
        Debug.Log($"{randomIndex}");
    }
}
