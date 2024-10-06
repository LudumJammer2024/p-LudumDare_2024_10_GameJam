using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : Singleton<GameInstance>
{
    [SerializeField] public GameState m_gameState;
    private int m_nodesInLevel;

    private void Awake()
    {
        if (!m_gameState)
            throw new System.NullReferenceException("Missing the Game State");

        m_nodesInLevel = FindObjectsOfType<NodeController>().Length;
        Debug.Log(m_nodesInLevel);
        m_gameState.Init(m_nodesInLevel);
    }

    private void Update() {
        //Debug.Log(m_gameState.disableNodes[0]);
    }
}
