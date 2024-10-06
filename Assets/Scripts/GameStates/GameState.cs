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
    public States Current = States.IDLE;
    public bool hasWin => Current == States.WIN;
    public bool hasLose => Current == States.LOSE;
    //Events
    public delegate void ChangeGameStateHandler(States state);
    public static event ChangeGameStateHandler OnGameStateChange;  // Subscribe to this event if you want to do something when the game state changes
    // Main variables
    public List<NodeController> disableNodes = new List<NodeController>();
    public List<NodeController> activeNodes = new List<NodeController>();
    public NodeController currentNode = null;
    //Game loop
    public void ActivateNode(NodeController currentNode)
    {
        disableNodes.Remove(currentNode);
        activeNodes.Add(currentNode);
        NextNode();
    }
    public void NodeDestroyed()
    {
        LoseGame();
    }
    private void NextNode()
    {
        if (disableNodes.Count == 0) WinGame();
        int randomNodeIndex = Random.Range(0, disableNodes.Count - 1);
        currentNode = disableNodes[randomNodeIndex];
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
