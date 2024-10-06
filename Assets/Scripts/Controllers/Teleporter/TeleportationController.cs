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
    [SerializeField] private float m_timeToTeleport;
    [SerializeField] private GameObject m_playerGO; // Reference to the player GO to pass the Transform ref to the event
    [Header("Base Properties")]
    [SerializeField] private MeshRenderer m_baseMeshRenderer;
    [SerializeField] private Material m_baseReadyMaterial;
    [SerializeField] private Material m_baseActiveMaterial;
    [Header("Unity Events")]
    public UnityEvent onReady;
    public UnityEvent onActive;

    private string m_playerTag = "Player";
    private float m_counter = 0.0f;
    private bool m_isPlayerPresent = false;
    public float Counter { get => m_counter; }

    private void Start()
    {
        SetState(currentState);
    }

    private void Update()
    {
        if (!m_isPlayerPresent || currentState == TeleporterState.Idle) return;
        m_counter += 1 * Time.deltaTime;

        if (currentState == TeleporterState.Ready) SetActive();

        if (m_counter >= m_timeToTeleport && m_isPlayerPresent && m_playerGO && currentState == TeleporterState.Active)
        {
            m_counter = 0.0f;
            m_isPlayerPresent = false;
            OnTeleport?.Invoke(m_playerGO.transform);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(m_playerTag))
        {
            m_playerGO = other.gameObject;
            m_isPlayerPresent = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(m_playerTag))
        {
            m_isPlayerPresent = false;
            m_playerGO = null;
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