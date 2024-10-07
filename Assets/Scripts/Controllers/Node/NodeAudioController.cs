using UnityEngine;

public class NodeAudioController : MonoBehaviour
{
    private AudioSource loopAudioSource;
    private AudioSource loopAudioSource2;
    private AudioSource oneShotAudioSource;

    [SerializeField] private AudioClip idleLoopClip;
    [SerializeField] private AudioClip underAttackLoopClip;
    [SerializeField] private AudioClip activeLoopClip;
    [SerializeField] private AudioClip activeLoopClip2;

    [SerializeField] private AudioClip[] safeOneShotClips;
    [SerializeField] private AudioClip[] activeOneShotClips;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;

    void Awake()
    {
        if (loopAudioSource == null) CreateAudioSources();
    }

    public void PlayIdle()
    {
        if (loopAudioSource == null) CreateAudioSources();
        StopAllLoopingSounds();
        loopAudioSource.clip = idleLoopClip;
        loopAudioSource.Play();
    }

    public void PlayUnderAttack()
    {
        StopAllLoopingSounds();
        loopAudioSource.clip = underAttackLoopClip;
        loopAudioSource.Play();
    }

    public void PlaySafe()
    {
        PlayOneShot(safeOneShotClips);
    }

    public void PlayActive()
    {
        StopAllLoopingSounds();
        PlayOneShot(activeOneShotClips);

        loopAudioSource.clip = activeLoopClip;
        loopAudioSource.Play();

        loopAudioSource2.clip = activeLoopClip2;
        loopAudioSource2.Play();
    }

    private void PlayOneShot(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        oneShotAudioSource.PlayOneShot(clips[index], volume);
    }

    private void StopAllLoopingSounds()
    {
        if (loopAudioSource == null) return;
        if (loopAudioSource.isPlaying)
        {
            loopAudioSource.Stop();
        }
        if (loopAudioSource2.isPlaying)
        {
            loopAudioSource2.Stop();
        }
    }

    private void CreateAudioSources()
    {
        loopAudioSource = gameObject.AddComponent<AudioSource>();
        loopAudioSource2 = gameObject.AddComponent<AudioSource>();
        oneShotAudioSource = gameObject.AddComponent<AudioSource>();

        ConfigureAudioSource(loopAudioSource);
        ConfigureAudioSource(loopAudioSource2);
        ConfigureAudioSource(oneShotAudioSource, isLooping: false);
    }

    private void ConfigureAudioSource(AudioSource source, bool isLooping = true)
    {
        source.loop = isLooping;
        source.volume = volume;

        if (isLooping)
        {
            source.spatialBlend = 1.0f;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.rolloffMode = AudioRolloffMode.Linear; // or AudioRolloffMode.Logarithmic
        }
    }
}
