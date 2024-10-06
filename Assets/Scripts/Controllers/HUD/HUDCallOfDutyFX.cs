using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCallOfDutyFX : MonoBehaviour
{
    [SerializeField] PlayerState m_playerState;
    [SerializeField] private RawImage m_redOverlayOfDeath; // Notice the late night variable naming inspiration
    private void Awake()
    {
        if (!m_playerState)
            throw new System.NullReferenceException("Add the F* PlayerState");
    }
    private void Update()
    {
        //Debug.Log(m_playerState.Health);
        float alpha;
        // Wait at least 80% of the health to start showing the red overlay
        if (m_playerState.Health > m_playerState.MaxHealth * 0.8)
            alpha = 0;
        else
            alpha = 1 - m_playerState.Health / m_playerState.MaxHealth;


        m_redOverlayOfDeath.color = new Color(1, 1, 1, alpha);

    }
}
