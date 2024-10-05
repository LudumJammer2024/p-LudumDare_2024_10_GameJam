using UnityEngine;
using UnityEngine.Events;

public class HUDManager : Singleton<HUDManager>
{
    [Header("HUD Manager Events")]
    public UnityEvent onInteractPrompt;
    public UnityEvent onPlayerDeathScreen;
    public UnityEvent onDisplayTooltip;
    public UnityEvent onNotDisplayTooltip;

    private bool interactionPromptActive = false;
    private bool playerDeathScreenActive = false;
    private bool displayingTooltip = false;
    public int displayedTooltipIndex = 0;

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
}