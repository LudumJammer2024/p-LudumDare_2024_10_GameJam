using System;
using UnityEngine;
/// <summary>
/// Subscribes to the event delegate OnTeleport using the signature TeleportPlayer
/// The TeleportPlayer event handler takes the Transform of the player and asigns it to the spawn Destination
/// </summary>
public class TeleportDestination : MonoBehaviour
{
    public static TeleportDestination Instance { get; private set;}
    [SerializeField] private Transform spawnDestination;
    private void Awake()
    {
        // We only need ONE teleportation destination, so a duplicate will get detroyed
        if(!Instance || Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        if(!spawnDestination)
        {
            throw new Exception("Add the Spawn destination");
        }
    }
    private void TeleportPlayer(Transform playerPosition)
    {
        playerPosition.position = spawnDestination.position;
    }
    private void OnEnable()
    {
        TeleportationController.OnTeleport += TeleportPlayer;
    }
    private void OnDisable()
    {
        TeleportationController.OnTeleport -= TeleportPlayer;
    }

    
}