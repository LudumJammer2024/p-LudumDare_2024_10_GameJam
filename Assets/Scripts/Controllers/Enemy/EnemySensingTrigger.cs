using UnityEngine;

public class EnemySensingTrigger : MonoBehaviour
{
    public ChasingEnemyController enemyController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyController != null) enemyController.OnPlayerDetected(other.transform);
        }
    }
}
