using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class ChasingEnemyController : MonoBehaviour
{
    [SerializeField] private PlayerState m_playerState;
    [SerializeField] private SphereCollider m_sphereCollider;
    [SerializeField] private float m_MAX_SPEED = 10.0f;
    [SerializeField] private float m_fieldOfView = 0.0f;
    [SerializeField] private float m_fieldOfHearing = 0.0f;
    [SerializeField] private float m_angleOfViewBothSides = 0.0f;

    [SerializeField] private float m_maxChasingDistance = 0.0f;
    [SerializeField] private float m_damageDealingDistance = 5.0f;
    [SerializeField] private NavMeshAgent agent;
    private Vector3 patrolPosition;
    //[SerializeField] 
    private Transform target;
    private ChasingEnemyAudioController audioController;
    private bool m_chase = false;
    private bool m_playedChaseSound = false;
    // Events for the animator
    public enum EnemyStates
    {
        IDLE,
        WALK,
        ATTACK,
        DEATH
    }
    public delegate void EnemyStateMachine(EnemyStates state);
    public static event EnemyStateMachine OnChangeState;

    //
    private void Awake()
    {
        if (!m_playerState)
            throw new System.NullReferenceException("The PlayerState is missing in the ChasingEnemyController");

        if (m_fieldOfView == 0.0f) m_fieldOfView = 10f;
        if (m_fieldOfHearing == 0.0f) m_fieldOfView = 5f;
        if (m_angleOfViewBothSides == 0.0f) m_fieldOfView = 45f;
        if (m_maxChasingDistance == 0.0f || m_maxChasingDistance < m_fieldOfView) m_maxChasingDistance = m_fieldOfView * 1.5f;

        if (m_sphereCollider == null)
            throw new System.NullReferenceException("The f* sphere collider is missing a reference.");

        agent = GetComponent<NavMeshAgent>();

        patrolPosition = transform.position;
        agent.destination = patrolPosition;
        agent.acceleration = m_MAX_SPEED;

        m_sphereCollider.isTrigger = true; //This is a trigger that checks for a player
        m_sphereCollider.radius = m_fieldOfView;

        if (m_sphereCollider.gameObject.TryGetComponent<EnemySensingTrigger>(out EnemySensingTrigger sensingTrigger))
        {
            sensingTrigger.enemyController = this;
        }

        audioController = GetComponent<ChasingEnemyAudioController>();
    }

    private void Update()
    {
        if (!target) return;
        FieldOfViewVectorVisualizer(); //Comment this on final realease
        FieldOfHearing();
        FieldOfView();
        Chase();
    }

    private void Chase()
    {
        if (m_chase)
        {
            Debug.DrawLine(transform.position, target.position, Color.red);
            if ((target.position - transform.position).magnitude < m_maxChasingDistance)
            {
                agent.destination = target.position;
                DealDamage();
            }
            else
            {
                agent.destination = patrolPosition;
                m_chase = false;
                m_playedChaseSound = false;
            }

            if (m_chase && !m_playedChaseSound && audioController != null)
            {
                audioController.PlayNoticeSound();
                m_playedChaseSound = true;
            }

        }
    }

    private void DealDamage()
    {
        if ((target.position - transform.position).magnitude < m_damageDealingDistance)
            m_playerState.DealDamage();//.Health -= (int) 5.0f * Time.deltaTime;
    }
    private void FieldOfView()
    {
        Vector3 targetWithRepectToAgent = (target.position - transform.position).normalized;
        if ((target.position - transform.position).magnitude < m_fieldOfView)
        {
            if (Vector3.Dot(transform.forward, targetWithRepectToAgent) > Mathf.Sin(m_angleOfViewBothSides * Mathf.Deg2Rad))
            {
                m_chase = true;
                OnChangeState(EnemyStates.WALK);
            }

        }
        else
        {
            //agent.destination = patrolPosition;
            Debug.DrawLine(transform.position, target.position, Color.gray);
        }

        //Debug.Log("Mathf.Sin("+m_angleOfViewBothSides+"): " + Mathf.Sin(m_angleOfViewBothSides * Mathf.Deg2Rad));
        //Debug.Log("DOT: "+Vector3.Dot(transform.forward, targetWithRepectToAgent));
    }

    private void FieldOfHearing()
    {
        if ((target.position - transform.position).magnitude < m_fieldOfHearing)
        {
            Debug.DrawLine(transform.position, target.position, Color.green);
            agent.destination = target.position;
            m_chase = true;
        }
        else
        {
            //agent.destination = patrolPosition;
            Debug.DrawLine(transform.position, target.position, Color.gray);
        }
    }
    private void FieldOfViewVectorVisualizer()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * m_fieldOfView, Color.blue);

        Vector3 fovRight = transform.forward;
        Vector3 fovLeft = transform.forward;
        float ang = (m_angleOfViewBothSides) * Mathf.Deg2Rad;
        Matrix4x4 rotateLeft = new Matrix4x4(
                                        new Vector4(Mathf.Cos(ang), 0f, -Mathf.Sin(ang), 0f),
                                        new Vector4(0f, 1f, 0f, 0f),
                                        new Vector4(Mathf.Sin(ang), 0f, Mathf.Cos(ang), 0f),
                                        new Vector4(0f, 0f, 0f, 1f));
        Matrix4x4 rotateRight = new Matrix4x4(
                                        new Vector4(Mathf.Cos(-ang), 0f, -Mathf.Sin(-ang), 0f),
                                        new Vector4(0f, 1f, 0f, 0f),
                                        new Vector4(Mathf.Sin(-ang), 0f, Mathf.Cos(-ang), 0f),
                                        new Vector4(0f, 0f, 0f, 1f));

        Vector3 r = rotateRight * fovRight;
        Vector3 l = rotateLeft * fovLeft;

        Debug.DrawLine(transform.position, transform.position + r * m_fieldOfView, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + l * m_fieldOfView, Color.yellow);

        Debug.DrawLine(target.position, target.position + target.forward * m_fieldOfView, Color.yellow);
    }

    public void OnPlayerDetected(Transform playerTransform)
    {
        target = playerTransform;
    }
}
