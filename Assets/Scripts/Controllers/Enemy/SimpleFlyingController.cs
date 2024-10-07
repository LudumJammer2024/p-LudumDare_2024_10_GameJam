using UnityEngine;

public class SimpleFlyingController : MonoBehaviour
{
    private Transform m_flyingBug;
    private float m_flyingSpeed;
    private void Start()
    {
        m_flyingBug = GetComponent<Transform>();
        m_flyingSpeed = Random.Range(1,5);
    }

    private void Update()
    {
        //Up and down
        m_flyingBug.position += Vector3.up * Mathf.Sin(Time.time * m_flyingSpeed) * m_flyingSpeed * Time.deltaTime;

        //Side by side
        m_flyingBug.position += Vector3.right * Mathf.Cos(Time.time * m_flyingSpeed) * m_flyingSpeed * Time.deltaTime;
    }
}
