using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(SoundController))]
[RequireComponent(typeof(MeshRenderer))]
public class ButtonController : MonoBehaviour, IInteractable, IInteractableSound
{
    [Tooltip("Events that happen when a button is pressed")]
    public UnityEvent onButtonPressed;
    [Tooltip("Events that happen when a button is unpressed")]
    public UnityEvent onButtonUnpressed;
    public bool isButtonPushed = false;
    [Tooltip("How long between button presses")]
    public float cooldown = 5f;
    public float cooldownTime = 0f;
    private MeshRenderer meshRenderer;

    public Color defaultColour;
    public Color activatedColour;
    public AudioClip pressSound;
    public AudioClip failedSound;
    [SerializeField] private float failedInteractVolume = 0.5f;
    public AudioClip releaseSound;
    private AudioClip currentAudioClip;
    private SoundController soundController;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = defaultColour;
        soundController = GetComponent<SoundController>();
    }

    void Update()
    {
        meshRenderer.material.color = isButtonPushed ? activatedColour : defaultColour;
    }

    public void Interact()
    {
        if (!isButtonPushed && cooldownTime <= 0f)
        {
            StartCoroutine(ButtonCooldown());
        }
        else if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(failedSound, failedInteractVolume);
        }
    }

    private void PushButton()
    {
        // Check that the button gets pushed when we push it
        Debug.Log("Button pushed");
        currentAudioClip = pressSound;
        PlaySound();
        isButtonPushed = true;
        onButtonPressed.Invoke();
    }

    private void UnpushButton()
    {
        // Check that the button gets pushed when we push it
        Debug.Log("Button unpushed");
        currentAudioClip = releaseSound;
        PlaySound();
        isButtonPushed = false;
        onButtonUnpressed.Invoke();
    }

    private IEnumerator ButtonCooldown()
    {
        PushButton();
        cooldownTime = cooldown;

        while (cooldownTime > 0f)
        {
            cooldownTime -= Time.deltaTime;
            yield return null; 
        }

        UnpushButton();
        cooldownTime = 0f;
    }

    public void PlaySound()
    {
        soundController.Play(currentAudioClip);
    }
}