using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// I wish I could call it "SceneManager" but this is
// already taken by the built-in Unity SceneManager
public class SceneManagerSingleton : Singleton<SceneManagerSingleton>
{

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene(bool asyncLoad = false)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (!asyncLoad) LoadScene(sceneName);
        else LoadSceneAsync(sceneName);
    }

    // Asynchronous scene loading
    // Right now I think the synchronous loading works better
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Time.timeScale = 1.0f;
    }

    /*
    public void PauseGame()
    {
    }

    public void UnpauseGame()
    {
        PlayerPrefs.Save();
    }
    */
}
