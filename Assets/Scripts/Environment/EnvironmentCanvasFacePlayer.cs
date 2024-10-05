using UnityEngine;

public class EnvironmentCanvasFacePlayer : MonoBehaviour
{
    private void Update()
    {
        FacePlayerOnYAxis();
    }

    private void FacePlayerOnYAxis()
    {
        if (Camera.main != null)
        {
            Vector3 direction = Camera.main.transform.position - transform.position;
            direction.y = 0f;
            
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
