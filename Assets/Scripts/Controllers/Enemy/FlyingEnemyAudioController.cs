using UnityEngine;

public class FlyingEnemyAudioController : MonoBehaviour
{
    private AudioSource source;

    [SerializeField] private AudioClip[] buzzClips;
    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private float buzzVolume = 1f;
    [SerializeField] private float killVolume = 1f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 20f;

    void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.volume = buzzVolume;
        source.spatialBlend = 1.0f;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Linear; // or AudioRolloffMode.Logarithmic
    }

    void OnEnable()
    {
        if (buzzClips.Length == 0) return;
        int index = Random.Range(0, buzzClips.Length);
        source.clip = buzzClips[index];
        source.Play();
    }

    public void PlayDeathSound()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(deathClips, killVolume);
    }
}
