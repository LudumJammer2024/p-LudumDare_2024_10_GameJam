using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(HUDManager))]
public class HUDTeleportPrompt : MonoBehaviour
{
    [SerializeField] private GameObject m_teleportPromptGameObject;
    [SerializeField] private TMP_Text m_teleportPromptText;
    [SerializeField] private Vector2 m_startPosition;
    [SerializeField] private Vector2 m_endPosition;
    [SerializeField] private float m_animationDuration = 1.0f;
    [SerializeField] private float m_teleportTime = 3.0f;
    private Coroutine currentCoroutine;

    void Awake()
    {
        m_teleportPromptGameObject.SetActive(false);
    }

    public void OnDisplayTeleportPrompt()
    {
        // If it is active already do nothing
        if (currentCoroutine != null) return;

        // If it isn't active, set it to be active and animated
        m_teleportPromptGameObject.SetActive(true);
        m_teleportPromptText.text = "Teleporting back in 3,0...";
        currentCoroutine = StartCoroutine(ShowTeleportPrompt());
    }

    public void OnHideTeleportPrompt()
    {
        if (currentCoroutine == null) return;

        StopCoroutine(currentCoroutine);
        m_teleportPromptGameObject.SetActive(false);
        currentCoroutine = null;
    }

    private IEnumerator ShowTeleportPrompt()
    {
        float countdown = m_teleportTime;
        float elapsedTime = 0f;

        while (elapsedTime < m_teleportTime)
        {
            if (elapsedTime < m_animationDuration)
            {
                float t = elapsedTime / m_animationDuration;
                t = Mathf.SmoothStep(0f, 1f, t);
                m_teleportPromptGameObject.transform.localPosition = Vector2.Lerp(m_startPosition, m_endPosition, t);
            }

            m_teleportPromptText.text = $"Teleporting back in {countdown:F1}...";
            countdown -= Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_teleportPromptText.text = "Teleporting back in 0...";
        OnHideTeleportPrompt();
    }
}
