using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SoundController))]
[RequireComponent(typeof(MeshRenderer))]
public class ShootableBase : MonoBehaviour, IHitable {
    [Tooltip("Events that happen when a button is pressed")]
    public UnityEvent onHit;
    private MeshRenderer meshRenderer;
    [SerializeField] private Color defaultColour = Color.blue;
    [SerializeField] private Color activatedColour = Color.green;
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private float hitSoundVolume = 1f;
    private SoundController soundController;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = defaultColour;
        soundController = GetComponent<SoundController>();
    }

    public void OnHit()
    {
        onHit.Invoke();
        soundController.PlayOneShot(hitSounds, hitSoundVolume);
        meshRenderer.material.color = (meshRenderer.material.color == defaultColour) ? activatedColour : defaultColour;
    }
}