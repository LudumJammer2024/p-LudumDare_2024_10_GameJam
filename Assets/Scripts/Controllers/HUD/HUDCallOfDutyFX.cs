using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCallOfDutyFX : MonoBehaviour
{
    [SerializeField] PlayerState m_playerState;
    [SerializeField] private AudioSource m_heartbeatAudio;
    [SerializeField] private RawImage m_redOverlayOfDeath; // Notice the late night variable naming inspiration
    [SerializeField] private float m_pulseSpeed = 1.5f;
    [SerializeField] private float m_healthThresholdStart = 0.8f;
    [SerializeField] private float m_healthThresholdEnd = 0.1f;

    private void Awake()
    {
        if (!m_playerState)
            throw new System.NullReferenceException("Add the F* PlayerState");

        if (!m_heartbeatAudio)
            throw new System.NullReferenceException("Add an AudioSource component for the heartbeat sound");

        m_heartbeatAudio.loop = true;
    }
    private void Update()
    {
        // Wait at least 80% of the health to start showing the red overlay
        float alpha = 0f;
        float normalisedHealth = Mathf.Clamp(m_playerState.Health / m_playerState.MaxHealth, 0f, 1f);

        if (normalisedHealth > m_healthThresholdStart)
        {
            m_heartbeatAudio.volume = alpha = 0f;
        }

        else
        {
            if (normalisedHealth <= m_healthThresholdStart && normalisedHealth > m_healthThresholdEnd)
            {
                float t = Mathf.InverseLerp(m_healthThresholdEnd, m_healthThresholdStart, normalisedHealth);
                m_heartbeatAudio.volume = alpha = Mathf.Lerp(1f, 0f, t);
            }
            else
            {
                m_heartbeatAudio.volume = alpha = 1f;
            }

            float pulse = Mathf.Sin(Time.time * m_pulseSpeed) * 0.1f;
            alpha = Mathf.Clamp(alpha + pulse, 0f, 1f);
        }

        m_redOverlayOfDeath.color = new Color(1, 1, 1, alpha);

        if (!m_heartbeatAudio.isPlaying && normalisedHealth <= m_healthThresholdStart)
        {
            m_heartbeatAudio.Play();
        }
        else if (normalisedHealth > m_healthThresholdStart && m_heartbeatAudio.isPlaying)
        {
            m_heartbeatAudio.Stop();
        }

    }
}
