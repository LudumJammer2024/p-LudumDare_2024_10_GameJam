using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class FlyingEnemyController : MonoBehaviour
{
    public UnityEvent OnAllEnemiesDead;
    public UnityEvent OnNodeDying;
    [Header("Enemy variables")]
    [Tooltip("Damage per second per enemy")]
    [SerializeField] private float damagePerEnemy = 0.5f;

    [Header("Health Bar Variables")]
    [Tooltip("The maximum health of the spawner node")]
    [SerializeField] private float maxHealth = 100f;
    [Tooltip("The UI Slider representing the health bar")]
    [SerializeField] private Slider healthBar;
    [Tooltip("The Image component of the health bar fill")]
    [SerializeField] private Image healthBarFill;
    [Tooltip("The gradient representing the color transition from green to red")]
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private TMP_Text healthText;

    private GameObject[] enemies;
    private bool isDisabled = true;
    public float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        UpdateHealthBarColor();
        UpdateHealthText();

        enemies = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            enemies[i] = transform.GetChild(i).gameObject;
            enemies[i].SetActive(false);
        }

        healthBar.gameObject.SetActive(false);
        healthText.gameObject.SetActive(false);
    }

    public void SpawnEnemies()
    {
        isDisabled = false;
        healthBar.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (isDisabled) return;
        ApplyDamageOverTime();
        CheckAllEnemiesDead();
    }

    private void ApplyDamageOverTime()
    {
        int aliveEnemies = CountAliveEnemies();
        if (aliveEnemies > 0)
        {
            float scaledDamage = damagePerEnemy * aliveEnemies * Mathf.Log(aliveEnemies + 1);
            currentHealth -= scaledDamage * Time.deltaTime;
            healthBar.value = currentHealth;
            UpdateHealthBarColor();
            UpdateHealthText();

            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                OnNodeDying.Invoke(); // Game over state!
                isDisabled = true;
            }
        }
    }

    private int CountAliveEnemies()
    {
        int count = 0;
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    private void CheckAllEnemiesDead()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.activeSelf)
            {
                return;
            }
        }

        isDisabled = true;
        healthBar.gameObject.SetActive(false);
        healthText.gameObject.SetActive(false);
        OnAllEnemiesDead.Invoke();
    }

    private void UpdateHealthBarColor()
    {
        float healthPercentage = currentHealth / maxHealth;
        Color newColor = healthGradient.Evaluate(healthPercentage);
        healthBarFill.color = newColor;
    }

    private void UpdateHealthText()
    {
        // Calculate the health percentage and format it to 1 decimal point
        float healthPercentage = currentHealth / maxHealth * 100f;

        // Update the text with the formatted percentage
        healthText.text = $"NODE HEALTH: {healthPercentage:F1}%";
    }
}
