using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState", order = 0)]
public class GameState : ScriptableObject
{
    //States
    public enum States
    {
        IDLE,
        STARTED,
        WIN,
        LOSE
    }
    public States Current;
    private int m_nodesInGame;
    public bool hasWin => Current == States.WIN;
    public bool hasLose => Current == States.LOSE;
    //Events
    public delegate void ChangeGameStateHandler(States state);
    public static event ChangeGameStateHandler OnGameStateChange;  // Subscribe to this event if you want to do something when the game state changes
    // Main variables
    public List<NodeController> disableNodes;
    public List<NodeController> activeNodes;
    private NodeController m_currentNode = null;
    //Game loop
    public void Init(int nodesInGame) // The GameInstance will pass how many nodes are in the level
    {
        m_nodesInGame = nodesInGame;
        Current = States.IDLE;
        disableNodes = new List<NodeController>();
        activeNodes = new List<NodeController>();
    }

    public void AddToDisableNodes(NodeController node)
    {
        disableNodes.Add(node);
        // If we get all the nodes, we trigger one of them as Under Attack!
        if (disableNodes.Count == m_nodesInGame)
        {
            int randomNodeIndex = Random.Range(0, disableNodes.Count - 1);
            m_currentNode = disableNodes[randomNodeIndex];
            m_currentNode.SetUnderAttack();
        }

    }

    public void ActivateNode(NodeController node)
    {
        disableNodes.Remove(node);
        activeNodes.Add(node);
        NextNode();
    }
    public void NodeDestroyed()
    {
        LoseGame();
    }
    private void NextNode()
    {
        if (disableNodes.Count == 0)
        {
            WinGame();
        }
        else
        {
            int randomNodeIndex = Random.Range(0, disableNodes.Count - 1);
            m_currentNode = disableNodes[randomNodeIndex];
            // Logic done on the node
            m_currentNode.SetUnderAttack();
            // Display it on the minimap, etc
        }
    }

    //Events

    public void StartGame()
    {
        Current = States.STARTED;
        OnGameStateChange?.Invoke(Current);
    }
    public void LoseGame()
    {
        Current = States.LOSE;
        OnGameStateChange?.Invoke(Current);
    }
    public void WinGame()
    {
        Current = States.WIN;
        OnGameStateChange?.Invoke(Current);
    }
}
