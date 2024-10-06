using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState", menuName = "PlayerState", order = 0)]
public class PlayerState : ScriptableObject
{
    public delegate void PlayerHealthDelegate();
    public static event PlayerHealthDelegate OnPlayerDeath;
    [SerializeField] private const float MAX_HEALTH = 100;
    [SerializeField] private const float RECOVERY_COOLDOWN_TIME = 1;
    [SerializeField] private float m_health;
    [SerializeField] private float m_lastHealthRecord;
    [SerializeField] private float m_damageContribution = 5.0f;
    [SerializeField] private float m_recoveryContribution = 10.0f;
    [SerializeField] private bool m_isAlive = true;
    [SerializeField] private float m_recoveryCooldownProgress = 0;
    public float Health
    {
        get => m_health;
        // set
        // {
        //     if (m_health <= 0)
        //     {
        //         m_health = 0;
        //         if (OnPlayerDeath != null)
        //             OnPlayerDeath?.Invoke();
        //     }
        //     else
        //     {
        //         m_health = value;
        //     }
        // }
    }

    // Events

    // Methods
    public void Init()
    {
        m_health = MAX_HEALTH;
        m_isAlive = true;
        m_lastHealthRecord = m_health;
    }
    public void DealDamage()
    {
        if (!m_isAlive) return;

        m_lastHealthRecord = m_health;
        m_health -= m_damageContribution * Time.deltaTime;
        m_recoveryCooldownProgress = 0;

        if (m_health <= 0)
        {
            m_health = 0;
            m_isAlive = false;
            if (OnPlayerDeath != null)
                OnPlayerDeath?.Invoke();
        }
    }

    public void AutomaticHealthing()
    {
        if (m_lastHealthRecord == m_health) return;
        m_recoveryCooldownProgress += 1 * Time.deltaTime;

        if (m_recoveryCooldownProgress >= RECOVERY_COOLDOWN_TIME && m_health < MAX_HEALTH)
        {
            m_recoveryCooldownProgress = 10;
            m_lastHealthRecord = m_health;
            m_health += m_recoveryContribution * Time.deltaTime;

            if(m_health > MAX_HEALTH)
            {
                m_health = MAX_HEALTH;
                m_lastHealthRecord = m_health;
            }
        }

    }

}