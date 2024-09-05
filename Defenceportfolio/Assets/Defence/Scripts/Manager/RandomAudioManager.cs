using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomAudioManager : MonoBehaviour
{
    public List<string> soundNames;
    private float interval = 15f;

    private void Start()
    {
        StartCoroutine(RandomPlay());
    }
    
    private IEnumerator RandomPlay()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            AudioManager.Instance.PlayRandomSound(soundNames);
        }
    }

}
