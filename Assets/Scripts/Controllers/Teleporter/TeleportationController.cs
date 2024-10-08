using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Teleports any GameObject that has a "Player" tag to the TeleportDestinationPoint.
/// Passes the Player as a Transform references via the event delegate OnTeleport
/// </summary>
/// 
[RequireComponent(typeof(Collider))]
public class TeleportationController : MonoBehaviour
{
    public static event Action<Transform> OnTeleport; //Event delegate
    public enum TeleporterState
    {
        Idle,
        Ready,
        Active
    }

    [SerializeField] private TeleporterState currentState = TeleporterState.Idle;

    [Header("Settings")]
    [Tooltip("Time in seconds")]
    [SerializeField] private float m_timeToTeleport = 3.0f;
    [SerializeField] private GameObject m_playerGO; // Reference to the player GO to pass the Transform ref to the event
    [Header("Base Properties")]
    [SerializeField] private MeshRenderer m_baseMeshRenderer;
    [SerializeField] private Material m_baseReadyMaterial;
    [SerializeField] private Material m_baseActiveMaterial;
    [Header("Unity Events")]
    public UnityEvent onReady;
    public UnityEvent onActive;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private AudioClip deactivationSound;
    [SerializeField] private AudioClip teleportationSound;
    private AudioSource audioSource;

    private string m_playerTag = "Player";
    private float m_counter = 0.0f;
    private bool m_isPlayerPresent = false;
    public float Counter { get => m_counter; }

    private void Start()
    {
        SetState(currentState);

        // Add audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 1.0f;
        audioSource.maxDistance = 20.0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    private void Update()
    {
        if (!m_isPlayerPresent || currentState == TeleporterState.Idle) return;
        m_counter += 1 * Time.deltaTime;

        if (currentState == TeleporterState.Ready) SetActive();

        if (HUDManager.Instance != null) HUDManager.Instance.DisplayingTeleportPrompt = true;

        if (!audioSource.isPlaying && activationSound != null && currentState == TeleporterState.Active)
        {
            audioSource.clip = activationSound;
            audioSource.Play();
        }

        if (m_counter >= m_timeToTeleport && m_isPlayerPresent && m_playerGO && currentState == TeleporterState.Active)
        {
            m_counter = 0.0f;
            m_isPlayerPresent = false;
            OnTeleport?.Invoke(m_playerGO.transform);
            if (audioSource.isPlaying) audioSource.Stop();

            if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(teleportationSound);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(m_playerTag))
        {
            m_playerGO = other.gameObject;
            m_isPlayerPresent = true;
            //Debug.Log("Player on teleport: " + m_playerGO.name);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(m_playerTag))
        {
            // Stop playing the activate sound and play the deactivate sound
            m_isPlayerPresent = false;
            m_playerGO = null;
            m_counter = 0.0f;
            if (HUDManager.Instance != null) HUDManager.Instance.DisplayingTeleportPrompt = false;

            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.PlayOneShot(deactivationSound);
        }
    }

    private void SetState(TeleporterState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case TeleporterState.Idle:
                break;
            case TeleporterState.Ready:
                onReady?.Invoke();
                if (m_baseReadyMaterial == null || m_baseMeshRenderer == null) return;
                m_baseMeshRenderer.material = m_baseReadyMaterial;
                break;
            case TeleporterState.Active:
                onActive?.Invoke();
                if (m_baseActiveMaterial == null || m_baseMeshRenderer == null) return;
                m_baseMeshRenderer.material = m_baseActiveMaterial;
                break;
        }
    }

    public void SetIdle()
    {
        SetState(TeleporterState.Idle);
    }

    public void SetReady()
    {
        SetState(TeleporterState.Ready);
    }

    public void SetActive()
    {
        SetState(TeleporterState.Active);
    }
}