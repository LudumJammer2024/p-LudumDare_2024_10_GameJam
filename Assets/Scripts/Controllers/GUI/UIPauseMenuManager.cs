using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenuManager : Singleton<UIPauseMenuManager>
{
    [Tooltip("Path to the main menu, must be specified in build settings as well.")]
    [SerializeField] private string mainMenuScene = "Scenes/Game/Game";
    [Header("Canvases")]
    [Tooltip("Contains all other canvases, so we can easily hide the pause menu.")]
    [SerializeField] private GameObject ContainerCanvas;
    private bool isVisible = false;

    void Start()
    {
        if (ContainerCanvas != null) MenuVisibility(false);
    }

    public void GoToMainMenu()
    {
        if (SceneManagerSingleton.Instance != null)
        {
            PlayerPrefs.Save();
            Cursor.lockState = CursorLockMode.None;
            SceneManagerSingleton.Instance.LoadScene(mainMenuScene);
        }
    }

    public void ResetLevel()
    {
        if (SceneManagerSingleton.Instance != null)
        {
            PlayerPrefs.Save();
            SceneManagerSingleton.Instance.ReloadScene();
        }
    }

    public void UnpauseGame()
    {
        if (SceneManagerSingleton.Instance != null)
        {
            SceneManagerSingleton.Instance.UnpauseGame();
            MenuVisibility(false);
        }
    }

    // Toggles the menu on or off
    public void MenuVisibility()
    {
        MenuVisibility(!isVisible);
    }

    public void MenuVisibility(bool visible)
    {
        isVisible = visible;
        if (ContainerCanvas != null) ContainerCanvas.SetActive(visible);

        if (PlayerManager.Instance != null)
        {
            Cursor.lockState = visible ? CursorLockMode.None : (PlayerManager.Instance.equippedGun ? CursorLockMode.Locked: CursorLockMode.None);
        }
        else Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
