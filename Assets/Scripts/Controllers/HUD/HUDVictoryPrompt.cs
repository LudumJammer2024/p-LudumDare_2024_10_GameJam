using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HUDManager))]
public class HUDVictoryPrompt : MonoBehaviour
{
    [SerializeField] GameState m_gameState;
    [SerializeField] private GameObject m_victoryPromptGameObject;
    [SerializeField] private Vector2 m_startPosition;
    [SerializeField] private Vector2 m_endPosition;
    [SerializeField] private float m_animationDuration = 1.5f;
    [SerializeField] private AudioClip[] m_victorySounds;
    private bool notDisplayedVictoryPrompt = true;

    void Awake()
    {
        if (!m_gameState) throw new System.NullReferenceException("GameState missing from HUD victory prompt");
        m_victoryPromptGameObject.SetActive(false);
    }

    void Update()
    {
        if (m_gameState.hasWin && notDisplayedVictoryPrompt)
        {
            notDisplayedVictoryPrompt = false;
            DisplayVictoryPrompt();
        }
    }

    public void DisplayVictoryPrompt()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(m_victorySounds);
        m_victoryPromptGameObject.SetActive(true);
        StartCoroutine(ShowVictoryPrompt());
    }

    private IEnumerator ShowVictoryPrompt()
    {
        float elapsedTime = 0f;
        Vector2 initialPosition = m_startPosition;

        while (elapsedTime < m_animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / m_animationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            m_victoryPromptGameObject.transform.localPosition = Vector2.Lerp(initialPosition, m_endPosition, t);
            yield return null;
        }

        m_victoryPromptGameObject.transform.localPosition = m_endPosition;
    }
}
