using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class PauseMenu : Menu
{
    // Having a cariable local to this script to log whether the game is paused isn't
    // necessary - use GameManager.isPaused instead!
    //public static bool gamePaused = false;
    public GameObject menuFrame;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void ShowPauseMenu(bool show)
    {
        if (show)
        {
            Pause();
        }
        else
        {
            Resume();
        }
        menuFrame.SetActive(show);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Resume();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
