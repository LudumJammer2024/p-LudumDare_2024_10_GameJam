using UnityEngine;

public class EnvironmentCanvasFacePlayer : MonoBehaviour
{
    [SerializeField] private float downAngle = 0;
    private void Update()
    {
        FacePlayerOnYAxis();
    }

    private void FacePlayerOnYAxis()
    {
        if (Camera.main != null)
        {
            Vector3 direction = (Camera.main.transform.position - transform.position).normalized;
            direction.y = Mathf.Sin(downAngle * Mathf.Deg2Rad); //This helps for legibility
            
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
