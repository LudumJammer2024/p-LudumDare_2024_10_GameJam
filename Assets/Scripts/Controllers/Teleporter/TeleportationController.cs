using System;
using UnityEngine;
/// <summary>
/// Teleports any GameObject that has a "Player" tag to the TeleportDestinationPoint.
/// Passes the Player as a Transform references via the event delegate OnTeleport
/// </summary>
/// 
[RequireComponent(typeof(Collider))]
public class TeleportationController : MonoBehaviour
{
    public static event Action<Transform> OnTeleport; //Event delegate
    [SerializeField] private GameObject m_playerGO;  // Reference to the player GO to pass the Transform ref to the event

    [Header("Settings")]
    [Tooltip("Time in seconds")]
    [SerializeField] private float m_timeToTeleport;
    [SerializeField] private bool m_isEnabled = false;
    [Header("Base Properties")]
    [SerializeField] private MeshRenderer m_baseMeshRenderer;
    [SerializeField] private Material m_baseMaterial;
    private string m_playerTag = "Player";
    private float m_counter = 0.0f;
    private bool m_isPlayerPresent = false; // should be false for default.
    public bool Enable
    {
        get => m_isEnabled;
        set => EnablePortal(value);
    }
    public float Counter { get => m_counter; } // Exposing counter so it can read outside

    private void Start()
    {
        EnablePortal(false);
    }

    private void Update()
    {
        if (!m_isPlayerPresent) return;
        m_counter += 1 * Time.deltaTime;

        if (m_counter >= m_timeToTeleport && m_isPlayerPresent && m_playerGO && m_isEnabled)
        {
            m_counter = 0.0f;
            m_isPlayerPresent = false;
            OnTeleport.Invoke(m_playerGO.transform);
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

    private void EnablePortal(bool enablePortal)
    {
        m_isEnabled = enablePortal;

        if (m_baseMaterial == null || m_baseMeshRenderer == null) return;
        else if (m_isEnabled) m_baseMeshRenderer.material = m_baseMaterial;
    }
}