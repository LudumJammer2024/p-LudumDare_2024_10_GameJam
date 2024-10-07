using System.Collections;
using UnityEngine;

public class PlayerMusicController : MonoBehaviour
{
    private AudioSource source;
    public enum MusicState
    {
        OFF,
        PLAYING,
        STOPPED
    }
    private MusicState musicState = MusicState.OFF;

    [SerializeField] private AudioClip music;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private GameState m_gameState;

    void Awake()
    {
        if (!m_gameState) throw new System.NullReferenceException("The GameState is missing");

        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.volume = musicVolume;

        if (music != null) source.clip = music;
    }

    void Update()
    {
        if (musicState == MusicState.STOPPED) return;

        if (m_gameState.Current == GameState.States.STARTED) SetState(MusicState.PLAYING);
        else if (m_gameState.Current == GameState.States.WIN || m_gameState.Current == GameState.States.LOSE) SetState(MusicState.STOPPED);
    }

    private void Play()
    {
        source.Play();
    }

    private void FadeOut()
    {
        if (source.isPlaying)
        {
            StartCoroutine(FadeOutCoroutine(fadeDuration));
        }
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = source.volume;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0, timeElapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    private void SetState(MusicState newState)
    {
        if (musicState == newState) return;

        musicState = newState;
        switch (newState)
        {
            case MusicState.OFF:
                break;
            case MusicState.PLAYING:
                Play();
                break;
            case MusicState.STOPPED:
                FadeOut();
                break;
        }
    }
}
