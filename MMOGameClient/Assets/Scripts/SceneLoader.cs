using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject LoadingScreenCanvas;
    public Slider loadingBar;
    public void LoadGameScene(int sceneIndex = 1)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(int sceneIndex = 1)
    {
        LoadingScreenCanvas.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        while (!op.isDone)
        {
            loadingBar.value = Mathf.Clamp01( op.progress/0.9f);
            yield return null;
        }
        LoadingScreenCanvas.SetActive(false);

    }
    //private void OnLevelWasLoaded(int level)
    //{
    //    //FindObjectOfType<ClientLoginManager>().SceneLoaded();
    //}
}
