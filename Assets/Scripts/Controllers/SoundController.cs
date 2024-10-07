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

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (audioClip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    // Overloaded, plays a random sound from an array
    public void PlayOneShot(AudioClip[] clips, float volume = 1f)
    {
        if (clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[index], volume);
    }
}
