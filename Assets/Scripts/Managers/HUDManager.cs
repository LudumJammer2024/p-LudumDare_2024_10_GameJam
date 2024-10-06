using UnityEngine;
using UnityEngine.Events;

public class HUDManager : Singleton<HUDManager>
{
    [Header("HUD Manager Events")]
    public UnityEvent onInteractPrompt;
    public UnityEvent onPlayerDeathScreen;
    public UnityEvent onDisplayTooltip;
    public UnityEvent onNotDisplayTooltip;
    public UnityEvent onDisplayTeleportPrompt;
    public UnityEvent onNotDisplayTeleportPrompt;
    public UnityEvent onAmmoUpdate;
    public UnityEvent onShoot;

    private bool interactionPromptActive = false;
    private bool playerDeathScreenActive = false;
    private bool displayingTooltip = false;
    private bool displayingTeleportPrompt = false;
    public int displayedTooltipIndex = 0;
    private int ammoAmount = 10;

    public bool InteractionPrompt
    {
        get => interactionPromptActive;
        set
        {
            interactionPromptActive = value;
            onInteractPrompt.Invoke();
        }
    }

    public bool PlayerDeathScreen
    {
        get => playerDeathScreenActive;
        set
        {
            if (playerDeathScreenActive != value)
            {
                playerDeathScreenActive = value;
                onPlayerDeathScreen.Invoke();
            }
        }
    }

    public bool DisplayingTooltip
    {
        get => displayingTooltip;
        set
        {
            displayingTooltip = value;

            if (value) onDisplayTooltip.Invoke();
            else onNotDisplayTooltip.Invoke();
        }
    }

    public bool DisplayingTeleportPrompt
    {
        get => displayingTeleportPrompt;
        set
        {
            displayingTeleportPrompt = value;

            if (value) onDisplayTeleportPrompt.Invoke();
            else onNotDisplayTeleportPrompt.Invoke();
        }
    }
    
    public int Ammo
    {
        get => ammoAmount;
        set
        {
            ammoAmount = value;
            onAmmoUpdate?.Invoke();
        }
    }

    public void UpdateCrosshair() {
        onShoot?.Invoke();
    }
}