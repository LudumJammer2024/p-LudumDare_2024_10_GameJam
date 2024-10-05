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
    private GameObject m_playerGO;  // Reference to the player GO to pass the Transform ref to the event

    [Tooltip("Time in seconds")]
    [SerializeField] private float m_timeToTeleport;
    [SerializeField] private bool m_isEnable = false;
    private string m_playerTag;
    private float m_counter = 0.0f;
    private bool m_isPlayerPresent = true; // should be false for default.
    public bool Enable { get => m_isEnable; set => m_isEnable = value; } // To controll the teleport from the Node
    public float Counter { get => m_counter;} // Exposing counter so it can read outside

    private void Awake()
    {
        m_playerTag = "Player"; //How to get the player tag in the editor?
    }

    private void Update()
    {
        if (!m_isPlayerPresent) return;
        m_counter += 1 * Time.deltaTime;

        if (m_counter >= m_timeToTeleport && m_isPlayerPresent && m_playerGO && m_isEnable)
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
}