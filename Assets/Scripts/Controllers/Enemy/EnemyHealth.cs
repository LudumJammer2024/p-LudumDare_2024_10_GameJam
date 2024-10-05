using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health Settings")]
    [Tooltip("The amount of health the enemy starts with")]
    [SerializeField] private int startingHealth = 100;

    [Tooltip("Amount of damage the enemy takes when hit")]
    [SerializeField] private int damageAmount = 50;

    private int currentHealth;

    [Tooltip("Actions triggered when the enemy dies.")]
    public UnityEvent OnEnemyDeath;

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage()
    {
        TakeDamage(damageAmount);
    }

    private void Die()
    {
        OnEnemyDeath?.Invoke();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}