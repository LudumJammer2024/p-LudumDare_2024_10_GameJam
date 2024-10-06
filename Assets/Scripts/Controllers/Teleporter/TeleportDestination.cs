using System;
using UnityEngine;
/// <summary>
/// Subscribes to the event delegate OnTeleport using the signature TeleportPlayer
/// The TeleportPlayer event handler takes the Transform of the player and asigns it to the spawn Destination
/// </summary>
public class TeleportDestination : Singleton<TeleportDestination>
{
    private void TeleportPlayer(Transform playerPosition)
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerCapsuleGameObject != null)
        {
            GameObject playerCapsule = PlayerManager.Instance.PlayerCapsuleGameObject;
            if (playerCapsule.TryGetComponent<CharacterController>(out CharacterController playerController))
            {
                playerController.enabled = false;
                playerPosition.position = transform.position;
                playerController.enabled = true;
            }
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
}