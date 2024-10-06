using System;
using UnityEngine;
/// <summary>
/// Subscribes to the event delegate OnTeleport using the signature TeleportPlayer
/// The TeleportPlayer event handler takes the Transform of the player and asigns it to the spawn Destination
/// </summary>
public class TeleportDestination : Singleton<TeleportDestination>
{
    private Transform playerTransform;

    private void TeleportPlayer(Transform playerPosition)
    {
        if (PlayerManager.Instance != null)
        {
            playerTransform = playerPosition;
        }
    }
    private void OnEnable()
    {
        TeleportationController.OnTeleport += TeleportPlayer;
    }
    private void OnDisable()
    {
        TeleportationController.OnTeleport -= TeleportPlayer;
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            playerTransform.position = transform.position;
            playerTransform = null;
        }
    }
}