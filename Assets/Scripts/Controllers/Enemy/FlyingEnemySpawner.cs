using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public UnityEvent OnAllEnemiesDead;

    private GameObject[] enemies;
    private bool isDisabled = true;

    private void Awake()
    {
        enemies = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            enemies[i] = transform.GetChild(i).gameObject;
            enemies[i].SetActive(false);
        }
    }

    public void SpawnEnemies()
    {
        isDisabled = false;
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
        CheckAllEnemiesDead();
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
        OnAllEnemiesDead.Invoke();
    }
}
