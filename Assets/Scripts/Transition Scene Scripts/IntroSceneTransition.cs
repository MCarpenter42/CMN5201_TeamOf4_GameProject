using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneTransition : Core
{
    private void OnEnable()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
