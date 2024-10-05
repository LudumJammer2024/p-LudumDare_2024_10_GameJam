using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    // Reference to the player
    public GameObject PlayerRootGameObject { get; private set; }
    public GameObject PlayerCapsuleGameObject;
    public static event Action OnPlayerGunEquip;

    [Header("Spawning")]
    [Tooltip("Player prefab. Spawns the player at the spawnPoint.")]
    [SerializeField] private GameObject playerPrefab;
    [Tooltip("HUD prefab. Instantiated at scene load.")]
    [SerializeField] private GameObject HUDPrefab;
    private GameObject m_HUD;
    /*
    [Tooltip("Pause menu prefab. Instantiated at scene load.")]
    [SerializeField] private GameObject PauseMenuPrefab;
    */
    [Tooltip("Game object where the player is instantiated.")]
    [SerializeField] private GameObject spawnPoint;

    [Header("Setable fields and states")]
    public bool isAlive = true;

    /*NOTE
    On the start the cursor is enable, no crosshair
    Once the player starts the game, the cursor gets locked and the crosshair shows up
    */

    public bool controlEnabled = false; //TODO true
    public float volume = 1.0f;
    public float sensitivity = 1.0f;

    public bool ControlEnable { set => controlEnabled = value; }

    void Awake()
    {

        if (playerPrefab == null)
        {
            Debug.LogError("No Player Prefab selected.");
        }
        else if (spawnPoint == null)
        {
            Debug.LogError("No spawn point attached to the Player Manager.");
        }
        else
        {
            PlayerRootGameObject = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
        if (HUDPrefab == null)
        {
            Debug.LogError("No HUD Prefab selected.");
        }
        else
        {
            m_HUD = Instantiate(HUDPrefab);
        }

        /*
        if (PauseMenuPrefab == null)
        {
            Debug.LogError("No pause menu prefab selected.");
        }
        else
        {
            Instantiate(PauseMenuPrefab);
        }

        // Load volume and sensitivity
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }

        if (PlayerPrefs.HasKey("Volume"))
        {
            volume = PlayerPrefs.GetFloat("Volume");
        }
        */

    }
    private void Start()
    {
        if (m_HUD)
            m_HUD.SetActive(false); // The HUD will get activated once the gun is Equiped
    }

    void Update()
    {
        if (!isAlive)
        {
            if (HUDManager.Instance != null) HUDManager.Instance.PlayerDeathScreen = true;
            controlEnabled = false;
        }
    }

    public void EquipGun() //Call this with the Start button to get the gun
    {
        OnPlayerGunEquip.Invoke();
        m_HUD.SetActive(true);
        Debug.Log(PlayerRootGameObject);
        InputManager input = PlayerRootGameObject.GetComponentInChildren<InputManager>();
        input.SetCursorState(true);
    }
}