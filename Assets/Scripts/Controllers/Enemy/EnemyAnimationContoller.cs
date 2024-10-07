using UnityEngine;
using System.Collections;

public class EnemyAnimationContoller : MonoBehaviour
{
    public RuntimeAnimatorController m_animatorController;
    public AnimationClip attack;
    public AnimationClip idle;
    public AnimationClip death;
    public AnimationClip walk;
    public Animator m_animator;
    public ChasingEnemyController chasingEnemyController; //pass the ref to subscribe to the event

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        chasingEnemyController = GetComponent<ChasingEnemyController>();
        //m_animatorController = m_animator.

        //Debug.Log(m_animator);
        m_animator.Play(idle.name, 0);
    }

    private void OnChangeState(ChasingEnemyController.EnemyStates enemyState)
    {
        //Debug.Log(enemyState);
        switch (enemyState)
        {
            case ChasingEnemyController.EnemyStates.ATTACK:
                m_animator.Play(attack.name, 0);
                break;
            case ChasingEnemyController.EnemyStates.WALK:
                m_animator.Play(walk.name, 0);
                break;
            case ChasingEnemyController.EnemyStates.IDLE:
                m_animator.Play(idle.name, 0);
                break;
            case ChasingEnemyController.EnemyStates.DEATH:
                m_animator.Play(death.name, 0);
                break;
        }
    }
    private void OnEnable()
    {
        chasingEnemyController.OnChangeState += OnChangeState;
    }

    private void OnDisable()
    {

        chasingEnemyController.OnChangeState -= OnChangeState;
    }


}
