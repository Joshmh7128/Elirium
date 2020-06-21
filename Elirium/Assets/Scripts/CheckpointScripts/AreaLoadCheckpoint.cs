using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AreaLoadCheckpoint : Checkpoint
{
    public string loadedScene;
    public string unloadedScene;

    private bool isLoaded = false;

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (!isLoaded && other.CompareTag("Player"))
        {
            isLoaded = true;
            //SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            //SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            StartCoroutine("LoadScene");
        }
    }

    private IEnumerator LoadScene()
    {
        if (unloadedScene != "")
        {
            var unloadedLevel = SceneManager.UnloadSceneAsync(unloadedScene);
            yield return unloadedLevel;
        }

        if (loadedScene != "")
        {
            var loadedLevel = SceneManager.LoadSceneAsync(loadedScene, LoadSceneMode.Additive);
            yield return loadedLevel;
        }

        manager.UpdateOrbs();
        manager.SavePlayer();
    }
}
