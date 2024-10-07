using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerDeath : MonoBehaviour
{
    [Tooltip("Array containing death sounds")]
    [SerializeField] private AudioClip[] deathSounds;
    private InputManager input;
    private bool isDead = false;
    private bool hasKilledPlayer = false;
    void Awake()
    {
        input = GetComponent<InputManager>();
    }

    void Update()
    {
        if (PlayerManager.Instance != null) isDead = !PlayerManager.Instance.isAlive;

        if (isDead && !hasKilledPlayer)
        {
            OnPlayerDeath();
        }

        if (hasKilledPlayer && input.IsPausing()) {
            if (SceneManagerSingleton.Instance != null)
            {
                input.PauseInput(false);
                SceneManagerSingleton.Instance.ReloadScene();
            }
        }
    }

    public void KillPlayer()
    {
        isDead = true;
    }

    private void OnPlayerDeath()
    {
        hasKilledPlayer = true;
        if (PlayerManager.Instance != null) PlayerManager.Instance.isAlive = false;

        // Play death sound if there is an AudioManager in the scene
        if (AudioManager.Instance != null) AudioManager.Instance.PlayOneShot(deathSounds);
    }
}