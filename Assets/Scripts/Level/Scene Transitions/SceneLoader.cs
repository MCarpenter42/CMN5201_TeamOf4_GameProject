using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator crossFade;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        crossFade.SetTrigger("Start");
        yield return new WaitForSeconds(1.0f);
        Scene scene = SceneManager.GetActiveScene();
        int nextLevelBuildIndex = 1 - scene.buildIndex;
        SceneManager.LoadScene(nextLevelBuildIndex);
    }
}