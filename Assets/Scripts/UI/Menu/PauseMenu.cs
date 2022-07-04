using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class PauseMenu : UI
{
    // Having a cariable local to this script to log whether the game is paused isn't
    // necessary - use GameManager.isPaused instead!
    //public static bool gamePaused = false;
    public GameObject menuFrame;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void PauseMenuResume()
    {
        menuFrame.SetActive(false);
        Resume();
    }

    public void PauseMenuPause()
    {
        Pause();
        menuFrame.SetActive(true);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PauseMenuResume();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
