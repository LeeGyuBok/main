using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class UiSoundManager : ImmortalObject<UiSoundManager>
{
    [SerializeField] private AudioClip sceneDigitalTransitionSound;
    [SerializeField] private AudioClip infoSound;
    [SerializeField] private AudioClip acceptSound;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip inDigitalSound;
    [SerializeField] private AudioClip enterInsideSound;
    [SerializeField] private AudioClip enterOutsideSound;
    [SerializeField] private AudioClip instantiateCagedSound;
    
    [SerializeField] private AudioClip[] randomTypingSounds;
    private int _randomIndex;
    
    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    public void SceneTransitionSound()
    {
        _audioSource.PlayOneShot(sceneDigitalTransitionSound);
    }

    public void InfoSound()
    {
        _audioSource.PlayOneShot(infoSound);
    }

    public void AcceptSound()
    {
        _audioSource.PlayOneShot(acceptSound);
    }

    public void CancelSound()
    {
        _audioSource.PlayOneShot(cancelSound);
    }

    public void InDigitalSound()
    {
        _audioSource.PlayOneShot(inDigitalSound);
    }

    public void EnterOutsideSound()
    {
        _audioSource.PlayOneShot(enterOutsideSound);
    }

    public void EnterInsideSound()
    {
        _audioSource.PlayOneShot(enterInsideSound);
    }
    public void InstantiateCagedSound()
    {
        _audioSource.PlayOneShot(instantiateCagedSound);
    }

    public void RandomTypingSound()
    {
        _randomIndex = Random.Range(0, randomTypingSounds.Length);
        _audioSource.PlayOneShot(randomTypingSounds[_randomIndex]);
    }
}
