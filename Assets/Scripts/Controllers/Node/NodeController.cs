using UnityEngine;
using UnityEngine.Events;

public class NodeController : MonoBehaviour
{
    public enum NodeState
    {
        Idle,
        UnderAttack,
        Safe,
        Active
    }

    [SerializeField] private NodeState currentState = NodeState.Idle;

    // UnityEvents for state transitions
    public UnityEvent onIdle;
    public UnityEvent onUnderAttack;
    public UnityEvent onSafe;
    public UnityEvent onActive;

    // Boolean flags for state transitions
    private bool underAttackFlag = false;
    private bool safeFlag = false;
    private bool activeFlag = false;

    void Awake()
    {
        SetState(NodeState.Idle);
    }

    void Update()
    {
        HandleStateTransitions();
    }

    private void HandleStateTransitions()
    {
        switch (currentState)
        {
            case NodeState.Idle:
                if (underAttackFlag)
                {
                    SetState(NodeState.UnderAttack);
                }
                break;

            case NodeState.UnderAttack:
                if (safeFlag)
                {
                    SetState(NodeState.Safe);
                }
                break;

            case NodeState.Safe:
                if (activeFlag)
                {
                    SetState(NodeState.Active);
                }
                break;

            case NodeState.Active:
                // NO MORE TRANSITION
                break;
        }
    }

    private void SetState(NodeState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case NodeState.Idle:
                // Don't use this, mostly just in case we want to but don't use it.
                onIdle?.Invoke();
                break;
            case NodeState.UnderAttack:
                onUnderAttack?.Invoke();
                break;
            case NodeState.Safe:
                onSafe?.Invoke();
                break;
            case NodeState.Active:
                onActive?.Invoke();
                break;
        }
    }

    public void SetUnderAttack()
    {
        underAttackFlag = true;
    }

    public void SetSafe()
    {
        safeFlag = true;
    }

    public void SetActive()
    {
        activeFlag = true;
    }
}
