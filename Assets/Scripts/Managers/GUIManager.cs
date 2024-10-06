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
    
    private void GameStateChangedGUIHandler(GameState.States state)
    {
        if(state == GameState.States.STARTED)
        {
           ShowRadar(); 
        }

    }

    public void ShowRadar()
    {
        m_diegeticGUIs[0].gameObject.SetActive(false);
        m_diegeticGUIs[1].gameObject.SetActive(true);
    }
     private void OnEnable()
    {
        GameState.OnGameStateChange += GameStateChangedGUIHandler;
    }
    private void OnDisable()
    {
        GameState.OnGameStateChange -= GameStateChangedGUIHandler;
    }
}
