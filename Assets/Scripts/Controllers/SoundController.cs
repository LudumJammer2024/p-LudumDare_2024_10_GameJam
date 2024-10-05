using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    public AudioClip audioClip;
    [SerializeField]
    public float playbackTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        if (audioClip == null) return;
        audioSource.clip = audioClip = clip;
        playbackTime = 0;
        audioSource.Play();
    }
}
