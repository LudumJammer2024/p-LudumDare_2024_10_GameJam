using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : Singleton<GUIManager>
{
    [Tooltip("Add here all the diegetic UI on the scene")]
    [SerializeField] private GUIControllers[] m_diegeticGUIs;
    void Start()
    {
        for (int i = 0; i < m_diegeticGUIs.Length; i++)
        {
            Canvas c = m_diegeticGUIs[i].GetComponent<Canvas>();
            c.worldCamera = Camera.main;
        }
    }
}
