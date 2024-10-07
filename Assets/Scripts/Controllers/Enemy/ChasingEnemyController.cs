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
    [SerializeField] private float m_damageDealingDistance = 3.0f;
    [SerializeField] private NavMeshAgent agent;
    private Vector3 patrolPosition;
    private Vector3 previousPosition;
    //[SerializeField] 
    private Transform target;
    private ChasingEnemyAudioController audioController;
    private bool m_chase = false;
    private bool m_playedChaseSound = false;
    // CHASING STATES
    public enum ChasingStates
    {
        IDLE,
        CHASE,
        ATTACK,
        RETURN
    }
    public ChasingStates currentChasinState = ChasingStates.IDLE;
    private float m_attackCooldownProgress = 0;
    private const float ATTACK_COOLDOWN_TIME = 5;
    // Events for the animator
    public enum EnemyStates
    {
        IDLE,
        WALK,
        ATTACK,
        DEATH
    }
    public delegate void EnemyStateMachine(EnemyStates state);
    public event EnemyStateMachine OnChangeState;

    //
    private void Awake()
    {
        if (!m_playerState)
            throw new System.NullReferenceException("The PlayerState is missing in the ChasingEnemyController");

        if (m_fieldOfView == 0.0f) m_fieldOfView = 10f;
        if (m_fieldOfHearing == 0.0f) m_fieldOfView = 5f;
        if (m_angleOfViewBothSides == 0.0f) m_fieldOfView = 45f;
        if (m_maxChasingDistance == 0.0f || m_maxChasingDistance < m_fieldOfView) m_maxChasingDistance = m_fieldOfView * 1.5f;
        if (m_damageDealingDistance == 0) m_damageDealingDistance = 3.0f;

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

        m_attackCooldownProgress = ATTACK_COOLDOWN_TIME;
    }

    private void Update()
    {
        //Debug.Log(currentChasinState);
        States(currentChasinState);
        if (!target) return;
        FieldOfViewVectorVisualizer(); //Comment this on final realease
        FieldOfHearing();
        FieldOfView();
        Chase();

        // Checking if the enemy return to the patrol position
        if ((patrolPosition - transform.position).magnitude < 1 && !m_chase) //Good enough
        {
            OnChangeState(EnemyStates.IDLE);
            currentChasinState = ChasingStates.IDLE;
        }
            

        //currentState

    }
    //
    private void States(ChasingStates chasinState)
    {
        //Debug.Log(enemyState);
        switch (chasinState)
        {
            case ChasingStates.ATTACK:
                agent.destination = transform.position;
                break;
            case ChasingStates.CHASE:
                agent.destination = target.position;
                break;
            case ChasingStates.IDLE:
                break;
            case ChasingStates.RETURN:
                agent.destination = patrolPosition;
                break;
        }
    }
    //

    private void Chase()
    {

        if (m_chase)
        {
            //previousPosition = transform.position;
            //Debug.DrawLine(transform.position, target.position, Color.red);
            if ((target.position - transform.position).magnitude < m_maxChasingDistance)
            {
                //currentChasinState = ChasingStates.CHASE;
                //agent.destination = target.position;
                DealDamage();
            }
            else
            {
                //agent.destination = patrolPosition;
                currentChasinState = ChasingStates.RETURN;
                OnChangeState(EnemyStates.WALK);
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

        if ((target.position - transform.position).magnitude <= m_damageDealingDistance && isFacingThePlayer())
        {
            // We make the enemy stop to attack
            currentChasinState = ChasingStates.ATTACK;
            m_playerState.DealDamage();

            if (m_attackCooldownProgress >= ATTACK_COOLDOWN_TIME)
            {
                OnChangeState(EnemyStates.ATTACK);
                Debug.Log("ATTACK sent!");
                m_attackCooldownProgress = 0;
            }
            m_attackCooldownProgress += 1 * Time.deltaTime;
        }
        else
        {
            m_attackCooldownProgress = ATTACK_COOLDOWN_TIME;
            OnChangeState(EnemyStates.WALK);
            currentChasinState = ChasingStates.CHASE;
        }
    }
    private bool isFacingThePlayer()
    {
        // Dot product to check if the player is indeed in front, otherwise the mats will atack the air if the player passes behind
        Vector3 targetWithRepectToAgent = (target.position - transform.position).normalized;
        return (Vector3.Dot(transform.forward, targetWithRepectToAgent) > Mathf.Sin(m_angleOfViewBothSides * Mathf.Deg2Rad));
    }
    private void FieldOfView()
    {
        Vector3 targetWithRepectToAgent = (target.position - transform.position).normalized;
        if ((target.position - transform.position).magnitude < m_fieldOfView)
        {
            if (Vector3.Dot(transform.forward, targetWithRepectToAgent) > Mathf.Sin(m_angleOfViewBothSides * Mathf.Deg2Rad))
            {
                m_chase = true;
                currentChasinState = ChasingStates.CHASE;
                //OnChangeState(EnemyStates.WALK);
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
        if ((target.position - transform.position).magnitude < m_fieldOfHearing && currentChasinState == ChasingStates.IDLE)
        {
            Debug.DrawLine(transform.position, target.position, Color.green);
            //agent.destination = target.position;
            m_chase = true;
            currentChasinState = ChasingStates.CHASE;
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
