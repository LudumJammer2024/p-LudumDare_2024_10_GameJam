using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class EnemyAnimationContoller : MonoBehaviour
{
    public AnimatorController m_animatorController;
    public AnimationClip attack;
    public AnimationClip idle;
    public AnimationClip death;
    public AnimationClip walk;
    public Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        //m_animatorController = m_animator.

        Debug.Log(m_animator);
        m_animator.Play(idle.name, 0);
    }

    private void OnChangeState(ChasingEnemyController.EnemyStates enemyState)
    {
        Debug.Log(enemyState);
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
        ChasingEnemyController.OnChangeState += OnChangeState;
    }

    private void OnDisable()
    {

        ChasingEnemyController.OnChangeState -= OnChangeState;
    }


}
