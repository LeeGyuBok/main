using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : ImmortalObject<BackgroundMusicManager>
{
    private AudioSource _audioSource;
    
    [SerializeField] private List<AudioClip> backgroundMusicClips;
    
    private Dictionary<int, AudioClip> _backgroundMusicClipsDictionary;
    
    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _backgroundMusicClipsDictionary = new Dictionary<int, AudioClip>();

        for (int i = 0; i < backgroundMusicClips.Count; i++)
        {
            _backgroundMusicClipsDictionary[i] = backgroundMusicClips[i];
        }
    }

    private void Start()
    {
        PlayMusic(11);
    }

    public void PlayMusic(int index)
    {
        if (_audioSource.isPlaying)
        {
            if (_audioSource.clip.Equals(_backgroundMusicClipsDictionary[index]))
            {
                return;
            }
            _audioSource.Stop();
        }

        if (index is 4 or 11 or 9 or 10)
        {
            _audioSource.volume = 0.3f;
        }
        else
        {
            _audioSource.volume = 1f;
        }
        _audioSource.clip = _backgroundMusicClipsDictionary[index];
        _audioSource.Play();
    }
}
