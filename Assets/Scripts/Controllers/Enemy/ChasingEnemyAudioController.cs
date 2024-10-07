using UnityEngine;

public class ChasingEnemyAudioController : MonoBehaviour
{
    private AudioSource source;

    [SerializeField] private AudioClip[] noticeClips;
    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private float noticeVolume = 1.5f;
    [SerializeField] private float killVolume = 1f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;

    void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.volume = noticeVolume;
        source.spatialBlend = 1.0f;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Linear; // or AudioRolloffMode.Logarithmic
    }

    public void PlayNoticeSound()
    {
        if (noticeClips.Length == 0) return;
        int index = Random.Range(0, noticeClips.Length);
        source.clip = noticeClips[index];
        source.Play();
    }

    public void PlayDeathSound()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(deathClips, killVolume);
    }
}
